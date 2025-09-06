using System;
using System.Collections;
using System.Collections.Generic;
using GlobalSettings;
using TeamCherry.NestedFadeGroup;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Serialization;

public class BouncePod : ActivatingBase, IHitResponder, IHitResponderOverride
{
	[SerializeField]
	private string deactivateAnim;

	[SerializeField]
	private string activateAnim;

	[SerializeField]
	private string reactivateAnim;

	[SerializeField]
	private ParticleSystem activePt;

	[SerializeField]
	private NestedFadeGroupCurveAnimator pulseFadeChild;

	[SerializeField]
	private ParticleSystem endTickPt;

	[SerializeField]
	private NestedFadeGroupCurveAnimator tickFadeParent;

	[SerializeField]
	private JitterSelf deactivateWarningJitter;

	[Space]
	[SerializeField]
	private bool disableReposition;

	[SerializeField]
	private string bounceAnim;

	[SerializeField]
	private string hitAnim;

	[SerializeField]
	private string hookAnim;

	[SerializeField]
	private string ringAnim;

	[Space]
	[SerializeField]
	private float hitInertTime;

	[SerializeField]
	private string inertRecoverAnim;

	[Space]
	[SerializeField]
	private int hitsToBreak;

	[SerializeField]
	private string breakAnim;

	[Space]
	[SerializeField]
	private CameraShakeTarget impactShake;

	[SerializeField]
	private GameObject impactEffectPrefab;

	[SerializeField]
	private GameObject bounceSpawnPrefab;

	[SerializeField]
	[FormerlySerializedAs("animator")]
	private tk2dSpriteAnimator tk2dAnimator;

	[SerializeField]
	private Animator mecanimAnimator;

	[SerializeField]
	private AudioEventRandom bounceSounds;

	[SerializeField]
	private HarpoonHook harpoonHook;

	[SerializeField]
	private bool ignoreHitWhileHooked;

	[Space]
	[SerializeField]
	private UnityEvent onInertHit;

	[SerializeField]
	private UnityEvent onBounceHit;

	[SerializeField]
	private UnityEvent onRecover;

	[SerializeField]
	private UnityEvent onBreak;

	[SerializeField]
	private UnityEvent onAppear;

	[SerializeField]
	private UnityEvent onEndWarning;

	[SerializeField]
	private UnityEvent onEnd;

	[Space]
	[SerializeField]
	private Rigidbody2D bounceReactBody;

	[SerializeField]
	private Vector2 bounceReactVelocity;

	[SerializeField]
	private Vector2 hitReactVelocity;

	private Transform heroMarker;

	private bool hasStarted;

	private Coroutine cullWaitRoutine;

	private Collider2D collider;

	private static int _lastAttackCount = -1;

	private readonly Stack<Coroutine> bouncePullRoutines = new Stack<Coroutine>();

	private static BouncePod _currentBouncer;

	private float inertTimeLeft;

	private HitInstance lastHit;

	private int hitCount;

	private bool isBroken;

	private DamageHero[] heroDamagers;

	private TinkEffect tinkEffect;

	private Collider2D lastHitCollider;

	private bool isHarpoonHooked;

	private HitInstance hookQueuedHit;

	private bool doingBouncePull;

	private static readonly List<(BouncePod pod, HitInstance hit)> _queuedBounceHits = new List<(BouncePod, HitInstance)>();

	private static readonly List<(BouncePod pod, HitInstance hit)> _queuedInertHits = new List<(BouncePod, HitInstance)>();

	private static readonly int _deactivateAnticProp = Animator.StringToHash("In Deactivate Antic");

	private bool allowHitWhileHooked;

	private bool hasHeroMarker;

	public override bool IsPaused => isHarpoonHooked;

	public event Action BounceHit;

	public event Action InertHit;

	protected override void Awake()
	{
		base.Awake();
		collider = GetComponent<Collider2D>();
		heroDamagers = GetComponentsInChildren<DamageHero>(includeInactive: true);
		GameObject obj = base.gameObject;
		obj.AddComponentIfNotPresent<NonBouncer>();
		bool flag = harpoonHook != null;
		if (!flag)
		{
			harpoonHook = GetComponent<HarpoonHook>();
			flag = (allowHitWhileHooked = harpoonHook != null);
		}
		if (flag)
		{
			harpoonHook.OnHookStart.AddListener(delegate
			{
				PlayAnim(hookAnim);
				isHarpoonHooked = true;
			});
			harpoonHook.OnHookEnd.AddListener(delegate
			{
				if (isHarpoonHooked)
				{
					isHarpoonHooked = false;
					if (!(hookQueuedHit.Source == null))
					{
						HitInstance damageInstance = hookQueuedHit;
						hookQueuedHit = default(HitInstance);
						damageInstance.IsHarpoon = true;
						Hit(damageInstance);
					}
				}
			});
			harpoonHook.OnHookCancel.AddListener(delegate
			{
				isHarpoonHooked = false;
				hookQueuedHit = default(HitInstance);
			});
		}
		EventRegister.GetRegisterGuaranteed(obj, "HERO DAMAGED").ReceivedEvent += delegate
		{
			if (!(_currentBouncer != this))
			{
				if (bouncePullRoutines.Count > 0)
				{
					while (bouncePullRoutines.Count > 0)
					{
						StopCoroutine(bouncePullRoutines.Pop());
					}
				}
				CancelBouncePull();
			}
		};
	}

	protected override void Start()
	{
		base.Start();
		if (!disableReposition)
		{
			heroMarker = base.transform.Find("Hero Y");
		}
		hasHeroMarker = heroMarker != null;
		hasStarted = true;
		tinkEffect = GetComponent<TinkEffect>();
		if ((bool)tinkEffect)
		{
			tinkEffect.enabled = false;
			tinkEffect.OverrideResponder = this;
		}
	}

	private void OnDestroy()
	{
		if (_currentBouncer == this)
		{
			_currentBouncer = null;
		}
		ClearQueuedHits();
	}

	private void Update()
	{
		if (inertTimeLeft <= 0f)
		{
			return;
		}
		inertTimeLeft -= Time.deltaTime;
		if (!(inertTimeLeft > 0f))
		{
			if ((bool)collider)
			{
				collider.enabled = true;
			}
			PlayAnim(inertRecoverAnim);
			onRecover.Invoke();
		}
	}

	private void LateUpdate()
	{
		List<(BouncePod, HitInstance)> list;
		bool flag;
		if (_queuedBounceHits.Count > 0)
		{
			list = _queuedBounceHits;
			flag = true;
		}
		else
		{
			if (_queuedInertHits.Count <= 0)
			{
				return;
			}
			list = _queuedInertHits;
			flag = false;
		}
		HeroController instance = HeroController.instance;
		Vector2 a = instance.transform.position;
		a.y -= 1f;
		bool isHarpoon = false;
		BouncePod bouncePod = null;
		float num = float.MaxValue;
		HitInstance hitInstance = default(HitInstance);
		foreach (var item3 in list)
		{
			BouncePod item = item3.Item1;
			HitInstance item2 = item3.Item2;
			Vector2 b = item.transform.position;
			float num2 = Vector2.Distance(a, b);
			if (item2.IsHarpoon)
			{
				isHarpoon = true;
			}
			if (!(num2 > num))
			{
				num = num2;
				bouncePod = item;
				hitInstance = item2;
			}
		}
		hitInstance.IsHarpoon = isHarpoon;
		if (bouncePod != null)
		{
			if (flag)
			{
				bouncePod.lastHit = hitInstance;
				bouncePod.DoBounce();
				NailSlashTravel component = hitInstance.Source.GetComponent<NailSlashTravel>();
				if ((bool)component)
				{
					component.DoBounceEffect(bouncePod.transform.position);
				}
			}
			else
			{
				bouncePod.DoInertHit(instance.cState.facingRight);
			}
		}
		_queuedBounceHits.Clear();
		_queuedInertHits.Clear();
	}

	private void ClearQueuedHits()
	{
		for (int num = _queuedBounceHits.Count - 1; num >= 0; num--)
		{
			(BouncePod, HitInstance) tuple = _queuedBounceHits[num];
			if (!tuple.Item1 || tuple.Item1 == this)
			{
				_queuedBounceHits.RemoveAt(num);
			}
		}
		for (int num2 = _queuedInertHits.Count - 1; num2 >= 0; num2--)
		{
			(BouncePod, HitInstance) tuple2 = _queuedInertHits[num2];
			if (!tuple2.Item1 || tuple2.Item1 == this)
			{
				_queuedInertHits.RemoveAt(num2);
			}
		}
	}

	public bool WillRespond(HitInstance damageInstance)
	{
		if (!base.gameObject.activeInHierarchy)
		{
			return false;
		}
		if (isBroken || !base.IsActive)
		{
			return false;
		}
		if (!damageInstance.IsFirstHit)
		{
			return false;
		}
		if (inertTimeLeft > 0f)
		{
			return false;
		}
		HeroController instance = HeroController.instance;
		GameObject source = damageInstance.Source;
		DamageEnemies damageEnemies = (source ? source.GetComponent<DamageEnemies>() : null);
		if (damageEnemies == null)
		{
			_ = 1;
		}
		else
			_ = !damageEnemies.DidHitEnemy;
		HitInstance.HitDirection hitDirection = damageInstance.GetHitDirection(HitInstance.TargetType.BouncePod);
		bool isEquipped = Gameplay.WarriorCrest.IsEquipped;
		float num = (isEquipped ? (-1.35f) : (-1.25f));
		if (instance.transform.position.y < base.transform.position.y + num && hitDirection != HitInstance.HitDirection.Up && isEquipped)
		{
			return false;
		}
		if ((bool)source)
		{
			if (damageInstance.AttackType == AttackTypes.Nail && instance.cState.attackCount == _lastAttackCount)
			{
				return false;
			}
			if ((bool)source.GetComponent<BreakItemsOnContact>())
			{
				return false;
			}
		}
		return true;
	}

	public IHitResponder.HitResponse Hit(HitInstance damageInstance)
	{
		if (!base.gameObject.activeInHierarchy)
		{
			return IHitResponder.Response.None;
		}
		if (isBroken || !base.IsActive)
		{
			return IHitResponder.Response.None;
		}
		if (!damageInstance.IsFirstHit)
		{
			return IHitResponder.Response.None;
		}
		if (inertTimeLeft > 0f)
		{
			return IHitResponder.Response.None;
		}
		if (damageInstance.AttackType == AttackTypes.Spikes)
		{
			int cardinalDirection = DirectionUtils.GetCardinalDirection(damageInstance.UseBouncePodDirection ? damageInstance.BouncePodDirection : damageInstance.Direction);
			float hitSign = 1f;
			if (cardinalDirection == 2)
			{
				hitSign = -1f;
			}
			if (!isBroken)
			{
				PlayAnim(hitAnim);
			}
			SendInertHit(hitSign);
			SpawnHitEffects();
			return IHitResponder.Response.GenericHit;
		}
		HeroController instance = HeroController.instance;
		GameObject source = damageInstance.Source;
		DamageEnemies damageEnemies = (source ? source.GetComponent<DamageEnemies>() : null);
		bool flag = damageEnemies == null || !damageEnemies.DidHitEnemy;
		HitInstance.HitDirection hitDirection = damageInstance.GetHitDirection(HitInstance.TargetType.BouncePod);
		bool isEquipped = Gameplay.WarriorCrest.IsEquipped;
		float num = (isEquipped ? (-1.35f) : (-1.25f));
		if (instance.transform.position.y < base.transform.position.y + num && hitDirection != HitInstance.HitDirection.Up)
		{
			if (isEquipped)
			{
				return IHitResponder.Response.None;
			}
			flag = false;
		}
		if ((bool)source)
		{
			if (damageInstance.AttackType == AttackTypes.Nail && instance.cState.attackCount == _lastAttackCount)
			{
				return IHitResponder.Response.None;
			}
			if ((bool)source.GetComponent<BreakItemsOnContact>())
			{
				return IHitResponder.Response.None;
			}
		}
		if (isHarpoonHooked)
		{
			damageInstance.IsHarpoon = true;
			hookQueuedHit = damageInstance;
			if (ignoreHitWhileHooked || !allowHitWhileHooked || (flag && hasHeroMarker && hitDirection == HitInstance.HitDirection.Down))
			{
				return IHitResponder.Response.None;
			}
		}
		lastHit = damageInstance;
		hitCount++;
		if (hitsToBreak > 0 && hitCount >= hitsToBreak)
		{
			SetActive(value: false, isInstant: true);
			isBroken = true;
			PlayAnim(breakAnim);
			onBreak.Invoke();
		}
		lastHitCollider = (source ? source.GetComponent<Collider2D>() : null);
		switch (hitDirection)
		{
		case HitInstance.HitDirection.Left:
			if (!isBroken)
			{
				PlayAnim(hitAnim);
			}
			SendInertHit(-1f);
			if (flag && damageInstance.AttackType == AttackTypes.Nail)
			{
				instance.RecoilRight();
			}
			SpawnHitEffects();
			return IHitResponder.Response.GenericHit;
		case HitInstance.HitDirection.Right:
			if (!isBroken)
			{
				PlayAnim(hitAnim);
			}
			SendInertHit(1f);
			if (flag && damageInstance.AttackType == AttackTypes.Nail)
			{
				instance.RecoilLeft();
			}
			SpawnHitEffects();
			return IHitResponder.Response.GenericHit;
		case HitInstance.HitDirection.Up:
			if (!isBroken)
			{
				PlayAnim(hitAnim);
			}
			SendInertHit(instance.cState.facingRight ? 1 : (-1));
			if (flag && damageInstance.AttackType == AttackTypes.Nail)
			{
				instance.RecoilDown();
			}
			SpawnHitEffects();
			return IHitResponder.Response.GenericHit;
		case HitInstance.HitDirection.Down:
			if (flag && damageInstance.AttackType == AttackTypes.Nail)
			{
				_queuedBounceHits.Add((this, damageInstance));
			}
			else
			{
				_queuedInertHits.Add((this, damageInstance));
			}
			return IHitResponder.Response.GenericHit;
		default:
			return IHitResponder.Response.GenericHit;
		}
	}

	private void CancelBouncePull()
	{
		if (doingBouncePull)
		{
			doingBouncePull = false;
			BounceShared.OnBouncePullInterrupted();
		}
	}

	private void DoBounce()
	{
		DamageHero[] array = heroDamagers;
		for (int i = 0; i < array.Length; i++)
		{
			array[i].SetCooldown(0.5f);
		}
		PlayAnim(bounceAnim);
		SpawnHitEffects();
		SendBounceHit();
		HeroController instance = HeroController.instance;
		_lastAttackCount = instance.cState.attackCount;
		instance.crestAttacksFSM.SendEvent("BOUNCE CANCEL");
		instance.sprintFSM.SendEvent("BOUNCE CANCEL");
		instance.FinishDownspike();
		instance.StartAnimationControl();
		EventRegister.SendEvent(EventRegisterEvents.FsmCancel);
		while (bouncePullRoutines.Count > 0)
		{
			StopCoroutine(bouncePullRoutines.Pop());
		}
		CancelBouncePull();
		if (!disableReposition && (bool)heroMarker)
		{
			_currentBouncer = this;
			bouncePullRoutines.PushIfNotNull(StartCoroutine(BouncePull(instance)));
		}
		else
		{
			DoBounceOff(instance);
		}
	}

	private void DoInertHit(bool facingRight)
	{
		if (!isBroken)
		{
			PlayAnim(hitAnim);
		}
		SendInertHit(facingRight ? 1 : (-1));
		SpawnHitEffects();
	}

	private IEnumerator BouncePull(HeroController hc)
	{
		GameCameras.instance.cameraTarget.ShorterDetach();
		CancelBouncePull();
		if (isHarpoonHooked)
		{
			lastHit.IsHarpoon = true;
		}
		doingBouncePull = true;
		yield return bouncePullRoutines.PushIfNotNullReturn(StartCoroutine(BounceShared.BouncePull(base.transform, heroMarker.position, hc, lastHit)));
		doingBouncePull = false;
		if (!hc.cState.dashing && !hc.controlReqlinquished)
		{
			DoBounceOff(hc);
		}
		_currentBouncer = null;
	}

	private void DoBounceOff(HeroController hc)
	{
		hc.DownspikeBounce(harpoonRecoil: false);
		if ((bool)bounceSpawnPrefab)
		{
			bounceSpawnPrefab.Spawn(base.transform.position);
		}
		if ((bool)bounceReactBody)
		{
			Vector2 linearVelocity = bounceReactVelocity;
			if (hc.cState.facingRight)
			{
				linearVelocity.x *= -1f;
			}
			bounceReactBody.linearVelocity = linearVelocity;
		}
	}

	private void SpawnHitEffects()
	{
		bounceSounds.SpawnAndPlayOneShot(base.transform.position);
		bool flag = impactShake.TryShake(this);
		if ((!lastHitCollider || !tinkEffect || !tinkEffect.TryDoTinkReaction(lastHitCollider, !flag, !bounceSounds.HasClips())) && (bool)impactEffectPrefab)
		{
			float overriddenDirection = lastHit.GetOverriddenDirection(base.transform, HitInstance.TargetType.Regular);
			impactEffectPrefab.Spawn(base.transform.position).transform.SetRotation2D(Helper.GetReflectedAngle(overriddenDirection, reflectHorizontal: true, reflectVertical: false) + 180f);
		}
	}

	private void SendBounceHit()
	{
		if (hitInertTime > 0f)
		{
			inertTimeLeft = hitInertTime;
			if ((bool)collider)
			{
				collider.enabled = false;
			}
		}
		onBounceHit.Invoke();
		if (this.BounceHit != null)
		{
			this.BounceHit();
		}
	}

	private void SendInertHit(float hitSign)
	{
		if (hitInertTime > 0f)
		{
			inertTimeLeft = hitInertTime;
			if ((bool)collider)
			{
				collider.enabled = false;
			}
		}
		onInertHit.Invoke();
		if (this.InertHit != null)
		{
			this.InertHit();
		}
		if ((bool)bounceReactBody)
		{
			Vector2 linearVelocity = hitReactVelocity;
			linearVelocity.x *= hitSign;
			bounceReactBody.linearVelocity = linearVelocity;
		}
	}

	private void PlayAnim(string animName, bool fromEnd = false)
	{
		if (!string.IsNullOrEmpty(animName))
		{
			if ((bool)tk2dAnimator)
			{
				tk2dSpriteAnimationClip clipByName = tk2dAnimator.GetClipByName(animName);
				tk2dAnimator.PlayFromFrame(clipByName, fromEnd ? (clipByName.frames.Length - 1) : 0);
			}
			if ((bool)mecanimAnimator)
			{
				ActivatingBase.PlayAnim(this, mecanimAnimator, animName, fromEnd);
			}
		}
	}

	protected override void OnActiveStateUpdate(bool value, bool isInstant)
	{
		if ((bool)collider)
		{
			collider.enabled = value;
		}
		if ((bool)mecanimAnimator)
		{
			mecanimAnimator.SetBoolIfExists(_deactivateAnticProp, value: false);
		}
		isInstant = isInstant || !hasStarted;
		if (value)
		{
			if (!base.IsActive)
			{
				if (!string.IsNullOrEmpty(activateAnim))
				{
					PlayAnim(activateAnim, isInstant);
				}
			}
			else if (!string.IsNullOrEmpty(reactivateAnim))
			{
				PlayAnim(reactivateAnim, isInstant);
			}
			hitCount = 0;
			isBroken = false;
		}
		else
		{
			if (base.IsActive && !string.IsNullOrEmpty(deactivateAnim))
			{
				PlayAnim(deactivateAnim, isInstant);
			}
			ClearQueuedHits();
		}
	}

	protected override void OnActivate()
	{
		if ((bool)tickFadeParent)
		{
			tickFadeParent.ForceStop();
			tickFadeParent.Group.AlphaSelf = 1f;
		}
		if ((bool)endTickPt)
		{
			endTickPt.Stop(withChildren: true);
		}
		if ((bool)activePt)
		{
			activePt.Play(withChildren: true);
		}
		if ((bool)pulseFadeChild && pulseFadeChild.gameObject.activeInHierarchy)
		{
			pulseFadeChild.StartAnimation();
		}
		onAppear.Invoke();
	}

	protected override void OnDeactivateWarning()
	{
		if ((bool)tickFadeParent)
		{
			tickFadeParent.StartAnimation();
		}
		if ((bool)endTickPt)
		{
			endTickPt.Play(withChildren: true);
		}
		if ((bool)mecanimAnimator)
		{
			mecanimAnimator.SetBoolIfExists(_deactivateAnticProp, value: true);
		}
		if ((bool)activePt)
		{
			activePt.Stop(withChildren: true);
		}
		if ((bool)pulseFadeChild)
		{
			pulseFadeChild.StopAtCurrentPoint();
			pulseFadeChild.Group.FadeTo(1f, 0.5f);
		}
		if ((bool)deactivateWarningJitter)
		{
			deactivateWarningJitter.StartJitter();
		}
		onEndWarning.Invoke();
	}

	protected override void OnDeactivate()
	{
		if ((bool)tickFadeParent)
		{
			tickFadeParent.StopAtCurrentPoint();
		}
		if ((bool)endTickPt)
		{
			endTickPt.Stop(withChildren: true);
		}
		if ((bool)deactivateWarningJitter)
		{
			deactivateWarningJitter.StopJitter();
		}
		onEnd.Invoke();
	}

	public void Ring(bool playSound = true)
	{
		if (base.isActiveAndEnabled)
		{
			PlayAnim(ringAnim);
			if (playSound)
			{
				bounceSounds.SpawnAndPlayOneShot(base.transform.position);
			}
		}
	}

	public void SetDisableReposition(bool set)
	{
		disableReposition = set;
	}
}

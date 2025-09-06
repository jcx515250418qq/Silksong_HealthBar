using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class BounceBalloon : UnlockablePropBase, IHitResponder, ICancellable
{
	private static readonly int _bounceAnim = Animator.StringToHash("Bounce");

	private static readonly int _deflatedAnim = Animator.StringToHash("Deflated");

	private static readonly int _inflateAnim = Animator.StringToHash("Inflate");

	private static readonly int _inflatedAnim = Animator.StringToHash("Inflated");

	[SerializeField]
	private Animator animator;

	[SerializeField]
	private CameraShakeTarget sideHitShake;

	[SerializeField]
	private CameraShakeTarget impactShake;

	[SerializeField]
	private GameObject impactEffectPrefab;

	[SerializeField]
	private GameObject slashEffectPrefab;

	[SerializeField]
	private ParticleSystemPool hitParticles;

	[SerializeField]
	private Vector2 heroBouncePos;

	[SerializeField]
	private CameraShakeTarget bounceShake;

	[SerializeField]
	private AudioEventRandom inflateSound;

	[SerializeField]
	private AudioEventRandom bounceSound;

	[Space]
	[SerializeField]
	private GameObject bounceEffects;

	public UnityEvent OnBounce;

	[Space]
	[SerializeField]
	private float raiseVelocity = 18f;

	[SerializeField]
	private float raiseDuration = 0.5f;

	[SerializeField]
	private float xAccelerationFactor = 1f;

	[SerializeField]
	private float xDecelerationFactor = 2f;

	[SerializeField]
	private float xMaxSpeed = 4f;

	[Space]
	[SerializeField]
	private bool isInflated;

	[Space]
	[SerializeField]
	private VibrationDataAsset downBounceVibration;

	private bool attackCancelled;

	private int cullFramesLeft = 2;

	private Coroutine inflateRoutine;

	private InputHandler inputHandler;

	private AmbientFloat ambientFloat;

	private Collider2D collider;

	private HarpoonHook harpoonHook;

	private bool isHarpoonHooked;

	private HitInstance hookQueuedHit;

	private readonly Stack<Coroutine> bounceRoutines = new Stack<Coroutine>();

	private Action onBounceEnd;

	private static BounceBalloon activeBalloon;

	private bool bouncePullStarted;

	private void Awake()
	{
		ambientFloat = GetComponentInChildren<AmbientFloat>();
		collider = GetComponent<Collider2D>();
		harpoonHook = GetComponent<HarpoonHook>();
		if (!harpoonHook)
		{
			return;
		}
		harpoonHook.OnHookStart.AddListener(delegate
		{
			isHarpoonHooked = true;
		});
		harpoonHook.OnHookEnd.AddListener(delegate
		{
			isHarpoonHooked = false;
			if (!(hookQueuedHit.Source == null))
			{
				HitInstance damageInstance = hookQueuedHit;
				hookQueuedHit = default(HitInstance);
				damageInstance.Direction = 270f;
				damageInstance.IsHarpoon = true;
				Hit(damageInstance);
			}
		});
		harpoonHook.OnHookCancel.AddListener(delegate
		{
			isHarpoonHooked = false;
			hookQueuedHit = default(HitInstance);
		});
	}

	private void Start()
	{
		GameManager instance = GameManager.instance;
		inputHandler = instance.GetComponent<InputHandler>();
		if (isInflated)
		{
			Opened();
		}
		else
		{
			SetDeflated();
		}
	}

	private void OnDestroy()
	{
		if (activeBalloon == this)
		{
			activeBalloon = null;
		}
	}

	private void Update()
	{
		if (cullFramesLeft > 0)
		{
			cullFramesLeft--;
			if (cullFramesLeft == 0 && (bool)animator)
			{
				animator.cullingMode = AnimatorCullingMode.CullCompletely;
			}
		}
		if (inputHandler.inputActions.Attack.WasPressed)
		{
			attackCancelled = true;
		}
	}

	private void SpawnImpactEffect(HitInstance damageInstance)
	{
		if ((bool)impactEffectPrefab)
		{
			float overriddenDirection = damageInstance.GetOverriddenDirection(base.transform, HitInstance.TargetType.Regular);
			impactEffectPrefab.Spawn(base.transform.position).transform.SetRotation2D(Helper.GetReflectedAngle(overriddenDirection, reflectHorizontal: true, reflectVertical: false) + 180f);
		}
	}

	private void SpawnSlashEffect()
	{
		if ((bool)slashEffectPrefab)
		{
			slashEffectPrefab.Spawn(base.transform.position);
		}
	}

	public IHitResponder.HitResponse Hit(HitInstance damageInstance)
	{
		if (!isInflated)
		{
			return IHitResponder.Response.None;
		}
		if ((bool)animator)
		{
			animator.Play(_bounceAnim, 0, 0f);
		}
		bounceSound.SpawnAndPlayOneShot(base.transform.position);
		if ((bool)hitParticles)
		{
			hitParticles.PlayParticles();
		}
		if (damageInstance.AttackType != 0)
		{
			SpawnImpactEffect(damageInstance);
			return IHitResponder.Response.GenericHit;
		}
		if (isHarpoonHooked)
		{
			hookQueuedHit = damageInstance;
			return IHitResponder.Response.None;
		}
		GameObject source = damageInstance.Source;
		DamageEnemies damageEnemies = (source ? source.GetComponent<DamageEnemies>() : null);
		bool flag = damageEnemies == null || !damageEnemies.DidHitEnemy;
		HeroController instance = HeroController.instance;
		switch (damageInstance.GetHitDirection(HitInstance.TargetType.Regular))
		{
		case HitInstance.HitDirection.Left:
			if (flag)
			{
				instance.RecoilRightLong();
			}
			sideHitShake.DoShake(this);
			SpawnSlashEffect();
			return IHitResponder.Response.GenericHit;
		case HitInstance.HitDirection.Right:
			if (flag)
			{
				instance.RecoilLeftLong();
			}
			sideHitShake.DoShake(this);
			SpawnSlashEffect();
			return IHitResponder.Response.GenericHit;
		case HitInstance.HitDirection.Up:
			if (flag)
			{
				instance.RecoilDown();
			}
			sideHitShake.DoShake(this);
			SpawnSlashEffect();
			return IHitResponder.Response.GenericHit;
		default:
			impactShake.DoShake(this);
			if (flag)
			{
				SpawnImpactEffect(damageInstance);
				VibrationManager.PlayVibrationClipOneShot(downBounceVibration, null);
				CancelBounce();
				if (activeBalloon != null)
				{
					activeBalloon.CancelBounce();
				}
				HeroUtility.AddCancellable(this);
				bounceRoutines.PushIfNotNull(StartCoroutine(Bounce(damageInstance)));
				return IHitResponder.Response.GenericHit;
			}
			SpawnSlashEffect();
			return IHitResponder.Response.GenericHit;
		}
	}

	public void CancelBounce()
	{
		if (activeBalloon == this)
		{
			activeBalloon = null;
		}
		HeroUtility.RemoveCancellable(this);
		while (bounceRoutines.Count > 0)
		{
			StopCoroutine(bounceRoutines.Pop());
		}
		if (bouncePullStarted)
		{
			BounceShared.OnBouncePullInterrupted();
			bouncePullStarted = false;
		}
		if (onBounceEnd != null)
		{
			onBounceEnd();
			onBounceEnd = null;
		}
	}

	private IEnumerator Bounce(HitInstance hit)
	{
		Transform transform = base.transform;
		HeroController hc = HeroController.instance;
		hc.crestAttacksFSM.SendEvent("BOUNCE CANCEL");
		hc.sprintFSM.SendEvent("BOUNCE CANCEL");
		hc.FinishDownspike();
		EventRegister.SendEvent(EventRegisterEvents.FsmCancel);
		hc.RelinquishControl();
		bouncePullStarted = true;
		yield return bounceRoutines.PushIfNotNullReturn(StartCoroutine(BounceShared.BouncePull(transform, transform.TransformPoint(heroBouncePos), hc, hit)));
		bouncePullStarted = false;
		hc.RelinquishControl();
		int animationVersion = hc.StopAnimationControlVersioned();
		hc.AffectedByGravity(gravityApplies: false);
		int controlVersion = HeroController.ControlVersion;
		bounceShake.DoShake(this);
		OnBounce.Invoke();
		Transform obj = bounceEffects.transform;
		Vector3 position = obj.position;
		obj.position = new Vector3(hc.transform.position.x, position.y, position.z);
		tk2dSpriteAnimationClip clip = hc.AnimCtrl.GetClip("Updraft Rise");
		hc.AnimCtrl.animator.PlayFromFrame(clip, 0);
		CameraTarget camTarget = GameCameras.instance.cameraTarget;
		camTarget.SetUpdraft(active: true);
		Rigidbody2D body = hc.GetComponent<Rigidbody2D>();
		body.linearVelocity = Vector2.zero;
		onBounceEnd = delegate
		{
			hc.OnTakenDamage -= onBounceEnd;
			onBounceEnd = null;
			CancelBounce();
			camTarget.SetUpdraft(active: false);
			hc.ResetHardLandingTimer();
			hc.StartAnimationControl(animationVersion);
			hc.ResetAirMoves();
		};
		hc.OnTakenDamage += onBounceEnd;
		attackCancelled = false;
		float elapsed = 0f;
		WaitForFixedUpdate wait = new WaitForFixedUpdate();
		for (; elapsed <= raiseDuration; elapsed += Time.fixedDeltaTime)
		{
			if (attackCancelled)
			{
				break;
			}
			RaiseMovement(body);
			yield return wait;
		}
		if (attackCancelled)
		{
			hc.cState.downSpikeBouncing = false;
			hc.cState.downSpikeRecovery = false;
			hc.SetStartWithAttack();
		}
		else
		{
			hc.SetStartWithBalloonBounce();
		}
		onBounceEnd();
		if (controlVersion == HeroController.ControlVersion)
		{
			hc.RegainControl();
		}
		if (activeBalloon == this)
		{
			activeBalloon = null;
		}
		HeroUtility.RemoveCancellable(this);
	}

	private void RaiseMovement(Rigidbody2D body)
	{
		Vector2 linearVelocity = body.linearVelocity;
		bool flag = false;
		if (inputHandler.inputActions.Right.IsPressed)
		{
			linearVelocity.x += xAccelerationFactor * Time.deltaTime;
			flag = true;
		}
		if (inputHandler.inputActions.Left.IsPressed)
		{
			linearVelocity.x -= xAccelerationFactor * Time.deltaTime;
			flag = true;
		}
		if (!flag)
		{
			if (linearVelocity.x > 0f)
			{
				linearVelocity.x -= xDecelerationFactor * Time.deltaTime;
				if (linearVelocity.x < 0f)
				{
					linearVelocity.x = 0f;
				}
			}
			else if (linearVelocity.x < 0f)
			{
				linearVelocity.x += xDecelerationFactor * Time.deltaTime;
				if (linearVelocity.x > 0f)
				{
					linearVelocity.x = 0f;
				}
			}
		}
		linearVelocity.x = Mathf.Clamp(linearVelocity.x, 0f - xMaxSpeed, xMaxSpeed);
		linearVelocity.y = raiseVelocity;
		body.linearVelocity = linearVelocity;
	}

	private IEnumerator Inflate()
	{
		inflateSound.SpawnAndPlayOneShot(base.transform.position);
		float duration;
		if ((bool)animator)
		{
			animator.cullingMode = AnimatorCullingMode.AlwaysAnimate;
			animator.Play(_inflateAnim);
			yield return new WaitUntil(() => animator.GetCurrentAnimatorStateInfo(0).shortNameHash == _inflateAnim);
			duration = animator.GetCurrentAnimatorStateInfo(0).length;
		}
		else
		{
			duration = 0f;
		}
		if ((bool)ambientFloat)
		{
			for (float elapsed = 0f; elapsed < duration; elapsed += Time.deltaTime)
			{
				if ((bool)ambientFloat)
				{
					ambientFloat.SpeedMultiplier = elapsed / duration;
				}
				yield return null;
			}
		}
		else
		{
			yield return new WaitForSeconds(duration);
		}
		isInflated = true;
		SetActive(value: true);
		if ((bool)animator)
		{
			animator.Play(_inflatedAnim);
			animator.cullingMode = AnimatorCullingMode.CullCompletely;
		}
	}

	public override void Open()
	{
		if (inflateRoutine == null && !isInflated)
		{
			inflateRoutine = StartCoroutine(Inflate());
		}
	}

	public override void Opened()
	{
		isInflated = true;
		if ((bool)animator)
		{
			animator.Play(_inflatedAnim);
		}
		SetActive(value: true);
	}

	private void SetActive(bool value)
	{
		if ((bool)ambientFloat)
		{
			ambientFloat.SpeedMultiplier = (value ? 1f : 0f);
		}
		if ((bool)collider)
		{
			collider.enabled = value;
		}
	}

	public void SetDeflated()
	{
		isInflated = false;
		inflateRoutine = null;
		if ((bool)animator)
		{
			animator.Play(_deflatedAnim);
		}
		SetActive(value: false);
	}

	public void DoCancellation()
	{
		CancelBounce();
	}
}

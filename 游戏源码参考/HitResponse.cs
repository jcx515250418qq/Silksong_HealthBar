using System;
using HutongGames.PlayMaker;
using TeamCherry.SharedUtils;
using UnityEngine;
using UnityEngine.Events;

public class HitResponse : HitResponseBase, IHitResponder, IBreakerBreakable
{
	[Serializable]
	private struct SimpleFling
	{
		public GameObject Prefab;

		public MinMaxFloat FlingSpeed;
	}

	[SerializeField]
	private bool ignoreLeft;

	[SerializeField]
	private bool ignoreRight;

	[SerializeField]
	private bool ignoreUp;

	[SerializeField]
	private bool ignoreDown;

	[SerializeField]
	private bool ignoreMultiHit;

	[SerializeField]
	private bool onlyTakeHeroDamage;

	[SerializeField]
	private bool firstHitOnly;

	[SerializeField]
	private bool blockHitRecurseUpwards;

	[SerializeField]
	private int hitPriority;

	[Space]
	[SerializeField]
	private SpecialTypes unconditionallyAllowedTypes;

	[SerializeField]
	[EnumPickerBitmask(typeof(AttackTypes))]
	private int allowedAttackTypes = -1;

	[Space]
	[SerializeField]
	private CameraShakeTarget hitShake;

	[SerializeField]
	private GameObject hitEffect;

	[SerializeField]
	private Vector3 hitEffectOffset;

	[SerializeField]
	private TimerGroup hitEffectTimer;

	[SerializeField]
	private bool positionHitX;

	[SerializeField]
	private bool positionHitY;

	[SerializeField]
	private bool angleEffectForDownSpike;

	[SerializeField]
	private PlayMakerFSM positionSetFSM;

	[SerializeField]
	private EnemyHitEffectsRegular passHit;

	[SerializeField]
	private bool setOriginOnPassHit;

	[SerializeField]
	private Breakable passHitToBreakable;

	[SerializeField]
	private Recoil passRecoil;

	[SerializeField]
	private bool bounceRecoil;

	[SerializeField]
	private HarpoonHook harpoonHook;

	[Space]
	[SerializeField]
	private SimpleFling[] spawnFling;

	[Space]
	[SerializeField]
	private MinMaxInt hitCombo;

	[SerializeField]
	private float hitComboCoolown;

	[Space]
	[SerializeField]
	private PlayMakerFSM fsmTarget;

	[SerializeField]
	private bool enableFsmOnSend;

	[SerializeField]
	private bool addFsmEventDir;

	[SerializeField]
	private bool flipEventWithScale;

	[Space]
	[SerializeField]
	[ModifiableProperty]
	[Conditional("fsmTarget", true, false, false)]
	[InspectorValidation("IsFsmEventValid")]
	private string fsmEvent;

	public UnityEvent OnHit;

	[Space]
	[SerializeField]
	[ModifiableProperty]
	[Conditional("fsmTarget", true, false, false)]
	[InspectorValidation("IsFsmEventValid")]
	private string comboFsmEvent;

	public UnityEvent OnCombo;

	private bool isHarpoonHooked;

	private HitInstance hookQueuedHit;

	private int targetHitCombo;

	private int currentHitCombo;

	private float hitComboTimer;

	private double lastHitTime;

	public int HitPriority => hitPriority;

	public bool HitRecurseUpwards => !blockHitRecurseUpwards;

	public override bool IsActive
	{
		get
		{
			return base.enabled;
		}
		set
		{
			base.enabled = value;
		}
	}

	public BreakableBreaker.BreakableTypes BreakableType
	{
		get
		{
			if (!passHitToBreakable)
			{
				return BreakableBreaker.BreakableTypes.Basic;
			}
			return passHitToBreakable.BreakableType;
		}
	}

	GameObject IBreakerBreakable.gameObject => base.gameObject;

	public event Action<HitInstance.HitDirection> WasHitDirectional;

	private bool? IsFsmEventValid(string eventName)
	{
		if (addFsmEventDir)
		{
			if (!fsmTarget)
			{
				return null;
			}
			if (string.IsNullOrEmpty(eventName))
			{
				return null;
			}
			return fsmTarget.IsEventValid(eventName + "LEFT") || fsmTarget.IsEventValid(eventName + "RIGHT") || fsmTarget.IsEventValid(eventName + "UP") || fsmTarget.IsEventValid(eventName + "DOWN");
		}
		return fsmTarget.IsEventValid(eventName, isRequired: false);
	}

	private void OnDrawGizmosSelected()
	{
		Gizmos.matrix = base.transform.localToWorldMatrix;
		Gizmos.DrawWireSphere(hitEffectOffset, 0.2f);
	}

	protected override void Awake()
	{
		base.Awake();
		if (!base.transform.IsOnHeroPlane())
		{
			base.enabled = false;
		}
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
				if (damageInstance.MagnitudeMultiplier < 1f)
				{
					damageInstance.MagnitudeMultiplier = 1f;
				}
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
		ResetCombo();
	}

	private void Update()
	{
		if (currentHitCombo > 0)
		{
			hitComboTimer -= Time.deltaTime;
			if (hitComboTimer < 0f)
			{
				ResetCombo();
			}
		}
	}

	public IHitResponder.HitResponse Hit(HitInstance damageInstance)
	{
		if (!base.enabled)
		{
			return IHitResponder.Response.None;
		}
		if (onlyTakeHeroDamage && !damageInstance.IsHeroDamage)
		{
			return IHitResponder.Response.None;
		}
		if (firstHitOnly && !damageInstance.IsFirstHit)
		{
			return IHitResponder.Response.None;
		}
		if (!IsDamageInstanceAllowed(damageInstance))
		{
			return IHitResponder.Response.None;
		}
		if (ignoreMultiHit && damageInstance.HitEffectsType != 0)
		{
			return IHitResponder.Response.None;
		}
		if (Time.timeAsDouble - lastHitTime < 0.10000000149011612)
		{
			return IHitResponder.Response.None;
		}
		if (isHarpoonHooked)
		{
			hookQueuedHit = damageInstance;
			return IHitResponder.Response.None;
		}
		lastHitTime = Time.timeAsDouble;
		float num = damageInstance.MagnitudeMultiplier;
		HitInstance.HitDirection hitDirection = damageInstance.GetHitDirection(HitInstance.TargetType.Regular);
		Vector3 lossyScale = base.transform.lossyScale;
		string text = fsmEvent;
		bool flag = false;
		float minInclusive;
		float maxInclusive;
		switch (hitDirection)
		{
		case HitInstance.HitDirection.Left:
			if (ignoreLeft)
			{
				return IHitResponder.Response.None;
			}
			if (addFsmEventDir)
			{
				text = ((!flipEventWithScale || !(lossyScale.x < 0f)) ? (text + "LEFT") : (text + "RIGHT"));
			}
			if (bounceRecoil && damageInstance.AttackType == AttackTypes.Nail)
			{
				HeroController.instance.RecoilRight();
			}
			minInclusive = 120f;
			maxInclusive = 160f;
			break;
		case HitInstance.HitDirection.Right:
			if (ignoreRight)
			{
				return IHitResponder.Response.None;
			}
			if (addFsmEventDir)
			{
				text = ((!flipEventWithScale || !(lossyScale.x < 0f)) ? (text + "RIGHT") : (text + "LEFT"));
			}
			if (bounceRecoil && damageInstance.AttackType == AttackTypes.Nail)
			{
				HeroController.instance.RecoilLeft();
			}
			minInclusive = 30f;
			maxInclusive = 70f;
			break;
		case HitInstance.HitDirection.Up:
			if (ignoreUp)
			{
				return IHitResponder.Response.None;
			}
			if (addFsmEventDir)
			{
				text = ((!flipEventWithScale || !(lossyScale.y < 0f)) ? (text + "UP") : (text + "DOWN"));
			}
			if (bounceRecoil && damageInstance.AttackType == AttackTypes.Nail)
			{
				HeroController.instance.RecoilDown();
			}
			minInclusive = 70f;
			maxInclusive = 110f;
			num *= 1.5f;
			break;
		case HitInstance.HitDirection.Down:
			if (ignoreDown)
			{
				return IHitResponder.Response.None;
			}
			flag = true;
			if (addFsmEventDir)
			{
				text = ((!flipEventWithScale || !(lossyScale.y < 0f)) ? (text + "DOWN") : (text + "UP"));
			}
			minInclusive = 160f;
			maxInclusive = 380f;
			break;
		default:
			throw new ArgumentOutOfRangeException();
		}
		Vector3 position = damageInstance.Source.transform.position;
		SimpleFling[] array = spawnFling;
		for (int i = 0; i < array.Length; i++)
		{
			SimpleFling simpleFling = array[i];
			if ((bool)simpleFling.Prefab)
			{
				Rigidbody2D component = simpleFling.Prefab.Spawn(base.transform.TransformPoint(hitEffectOffset)).GetComponent<Rigidbody2D>();
				if (!(component == null))
				{
					float num2 = UnityEngine.Random.Range(minInclusive, maxInclusive);
					Vector2 vector = new Vector2(Mathf.Cos(num2 * (MathF.PI / 180f)), Mathf.Sin(num2 * (MathF.PI / 180f)));
					MinMaxFloat flingSpeed = simpleFling.FlingSpeed;
					float num3 = flingSpeed.GetRandomValue() * num;
					component.linearVelocity = vector * num3;
				}
			}
		}
		if ((bool)positionSetFSM)
		{
			FsmVector2 fsmVector = positionSetFSM.FsmVariables.FindFsmVector2("Hit Source Position");
			if (fsmVector != null)
			{
				fsmVector.Value = position;
			}
		}
		hitShake.DoShake(this);
		if ((bool)hitEffect && (!hitEffectTimer || hitEffectTimer.HasEnded))
		{
			Vector3 position2 = base.transform.position;
			Vector3 position3 = new Vector3(positionHitX ? position.x : position2.x, positionHitY ? position.y : position2.y, hitEffect.transform.position.z);
			if (hitEffectOffset != Vector3.zero)
			{
				position3 += base.transform.TransformVector(hitEffectOffset);
			}
			GameObject gameObject = hitEffect.Spawn(position3);
			if (angleEffectForDownSpike && flag)
			{
				gameObject.transform.SetRotation2D(180f - damageInstance.GetOverriddenDirection(base.transform, HitInstance.TargetType.Regular));
			}
			if ((bool)hitEffectTimer)
			{
				hitEffectTimer.ResetTimer();
			}
		}
		SendHitInDirection(damageInstance.Source, hitDirection);
		if (hitCombo.End > 0)
		{
			hitComboTimer = hitComboCoolown;
			currentHitCombo++;
			if (currentHitCombo >= targetHitCombo)
			{
				ResetCombo();
				ComboResponse();
				return IHitResponder.Response.GenericHit;
			}
		}
		if ((bool)fsmTarget && !string.IsNullOrEmpty(text))
		{
			if (enableFsmOnSend)
			{
				fsmTarget.enabled = true;
			}
			fsmTarget.SendEvent(text);
		}
		if (OnHit != null)
		{
			OnHit.Invoke();
		}
		if (this.WasHitDirectional != null)
		{
			this.WasHitDirectional(hitDirection);
		}
		if ((bool)passHit)
		{
			if (setOriginOnPassHit)
			{
				passHit.ReceiveHitEffect(damageInstance, passHit.transform.InverseTransformPoint(position));
			}
			else
			{
				passHit.ReceiveHitEffect(damageInstance);
			}
		}
		if ((bool)passHitToBreakable)
		{
			passHitToBreakable.Hit(damageInstance);
		}
		if ((bool)passRecoil)
		{
			passRecoil.RecoilByDamage(damageInstance);
		}
		return IHitResponder.Response.GenericHit;
	}

	private bool IsDamageInstanceAllowed(HitInstance damageInstance)
	{
		if ((unconditionallyAllowedTypes & damageInstance.SpecialType) != 0)
		{
			return true;
		}
		return allowedAttackTypes.IsBitSet((int)damageInstance.AttackType);
	}

	public void ResetCombo()
	{
		targetHitCombo = hitCombo.GetRandomValue();
		currentHitCombo = 0;
	}

	private void ComboResponse()
	{
		if ((bool)fsmTarget && !string.IsNullOrEmpty(comboFsmEvent))
		{
			if (enableFsmOnSend)
			{
				fsmTarget.enabled = true;
			}
			fsmTarget.SendEvent(comboFsmEvent);
		}
		if (OnCombo != null)
		{
			OnCombo.Invoke();
		}
	}

	public void HitFromBreaker(BreakableBreaker breaker)
	{
		if ((bool)passHitToBreakable)
		{
			passHitToBreakable.HitFromBreaker(breaker);
		}
	}

	public void BreakFromBreaker(BreakableBreaker breaker)
	{
		if ((bool)passHitToBreakable)
		{
			passHitToBreakable.BreakFromBreaker(breaker);
		}
	}
}

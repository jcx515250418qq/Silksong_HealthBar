using System;
using System.Linq;
using UnityEngine;
using UnityEngine.Events;

public class HitRigidbody2D : MonoBehaviour, IHitResponder
{
	[SerializeField]
	private Rigidbody2D body;

	[SerializeField]
	private Transform groupRoot;

	[SerializeField]
	private bool alwaysRespondToHit;

	[SerializeField]
	private Vector3 origin;

	[SerializeField]
	private Vector2 force;

	[SerializeField]
	private bool recoilHero;

	[SerializeField]
	private bool resetVelocity;

	[SerializeField]
	private bool useHitDirectionX;

	[SerializeField]
	private bool useHitDirectionY;

	[SerializeField]
	private AnimationCurve magnitudeMultiplierCurve = AnimationCurve.Constant(0f, 1f, 1f);

	[SerializeField]
	private CameraShakeTarget cameraShake;

	[SerializeField]
	private GameObject hitEffect;

	[SerializeField]
	private float hitEffectScale = 1f;

	[SerializeField]
	private AudioEventRandom hitAudio;

	[SerializeField]
	private bool dontPlayHitOnBreak;

	[Space]
	[SerializeField]
	private Breakable passHitToBreakable;

	[SerializeField]
	private Rigidbody2D breakReactBody;

	[SerializeField]
	private Vector2 breakReactForce;

	[Space]
	[SerializeField]
	private string hitEventRegister;

	private double enabledTime;

	public UnityEvent OnHit;

	private HitRigidbody2D[] groupBodies;

	private IHitEffectReciever[] hitEffects;

	public bool WasHit { get; private set; }

	public event Action<HitInstance> WasHitBy;

	private void OnDrawGizmosSelected()
	{
		Gizmos.matrix = base.transform.localToWorldMatrix;
		Gizmos.DrawWireSphere(origin, 0.2f);
	}

	private void Awake()
	{
		if (!(groupRoot ? groupRoot : base.transform).IsOnHeroPlane())
		{
			Collider2D component = GetComponent<Collider2D>();
			if ((bool)component)
			{
				component.enabled = false;
			}
			return;
		}
		if ((bool)groupRoot)
		{
			groupBodies = (from b in groupRoot.GetComponentsInChildren<HitRigidbody2D>(includeInactive: true)
				where b != this
				select b).ToArray();
		}
		hitEffects = GetComponents<IHitEffectReciever>();
	}

	private void OnEnable()
	{
		enabledTime = Time.timeAsDouble;
	}

	private void OnDisable()
	{
		WasHit = false;
	}

	private void FixedUpdate()
	{
		WasHit = false;
	}

	public IHitResponder.HitResponse Hit(HitInstance damageInstance)
	{
		if (!base.enabled)
		{
			return IHitResponder.Response.None;
		}
		if (damageInstance.AttackType == AttackTypes.Spikes)
		{
			return IHitResponder.Response.None;
		}
		if ((float)(Time.timeAsDouble - enabledTime) < 0.3f)
		{
			return IHitResponder.Response.GenericHit;
		}
		if (!body)
		{
			return IHitResponder.Response.None;
		}
		if (!alwaysRespondToHit && groupBodies != null)
		{
			HitRigidbody2D[] array = groupBodies;
			for (int i = 0; i < array.Length; i++)
			{
				if (array[i].WasHit)
				{
					return IHitResponder.Response.None;
				}
			}
		}
		WasHit = true;
		Vector2 vector = base.transform.TransformPoint(origin);
		Vector2 zero = Vector2.zero;
		Rigidbody2D rigidbody2D;
		Vector2 vector2;
		if (body.gameObject.activeInHierarchy)
		{
			rigidbody2D = body;
			vector2 = force;
		}
		else
		{
			rigidbody2D = breakReactBody;
			vector2 = breakReactForce;
		}
		switch (damageInstance.GetActualHitDirection(base.transform, HitInstance.TargetType.Regular))
		{
		case HitInstance.HitDirection.Left:
			zero.x = -1f;
			if (recoilHero && damageInstance.AttackType == AttackTypes.Nail)
			{
				HeroController.instance.RecoilRight();
			}
			break;
		case HitInstance.HitDirection.Right:
			zero.x = 1f;
			if (recoilHero && damageInstance.AttackType == AttackTypes.Nail)
			{
				HeroController.instance.RecoilLeft();
			}
			break;
		case HitInstance.HitDirection.Up:
			if (recoilHero && damageInstance.AttackType == AttackTypes.Nail)
			{
				HeroController.instance.RecoilDown();
			}
			goto case HitInstance.HitDirection.Down;
		case HitInstance.HitDirection.Down:
			zero.x = (HeroController.instance.cState.facingRight ? 1 : (-1));
			zero.y = 1f;
			break;
		}
		if (useHitDirectionX)
		{
			vector2.x *= zero.x;
		}
		if (useHitDirectionY)
		{
			vector2.y *= zero.y;
		}
		cameraShake.DoShake(this);
		if ((bool)hitEffect)
		{
			GameObject obj = hitEffect.Spawn();
			obj.transform.SetPosition2D(vector);
			obj.transform.localScale *= hitEffectScale;
		}
		IHitEffectReciever[] array2 = hitEffects;
		for (int i = 0; i < array2.Length; i++)
		{
			array2[i].ReceiveHitEffect(damageInstance);
		}
		if ((bool)passHitToBreakable)
		{
			passHitToBreakable.Hit(damageInstance);
		}
		if (!dontPlayHitOnBreak || !passHitToBreakable || !passHitToBreakable.IsBroken)
		{
			hitAudio.SpawnAndPlayOneShot(vector);
		}
		if (!string.IsNullOrEmpty(hitEventRegister))
		{
			EventRegister.SendEvent(hitEventRegister);
		}
		OnHit.Invoke();
		if (this.WasHitBy != null)
		{
			this.WasHitBy(damageInstance);
		}
		vector2 *= magnitudeMultiplierCurve.Evaluate(damageInstance.MagnitudeMultiplier);
		if ((bool)rigidbody2D)
		{
			if (resetVelocity)
			{
				rigidbody2D.linearVelocity = Vector2.zero;
			}
			rigidbody2D.AddForceAtPosition(vector2, vector, ForceMode2D.Impulse);
		}
		return IHitResponder.Response.GenericHit;
	}
}

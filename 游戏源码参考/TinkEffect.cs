using System;
using System.Collections.Generic;
using System.Linq;
using GlobalEnums;
using GlobalSettings;
using HutongGames.PlayMaker;
using TeamCherry.SharedUtils;
using UnityEngine;
using UnityEngine.Events;

public class TinkEffect : HitResponseBase, IHitResponder
{
	public delegate void TinkEffectSpawnEvent(Vector3 position, Quaternion rotation);

	[SerializeField]
	private bool isActive = true;

	[SerializeField]
	private int hitPriority;

	[Space]
	public GameObject blockEffect;

	[SerializeField]
	private bool overrideCamShake;

	[SerializeField]
	[ModifiableProperty]
	[Conditional("overrideCamShake", true, false, false)]
	private CameraShakeTarget camShakeOverride;

	public bool useNailPosition;

	public bool useOwnYPosition;

	public bool sendFSMEvent;

	public PlayMakerFSM fsm;

	[ModifiableProperty]
	[Conditional("sendFSMEvent", true, false, false)]
	[InspectorValidation("IsFsmEventValid")]
	public string FSMEvent;

	public bool sendDirectionalFSMEvents;

	public bool RecoilHero = true;

	public bool onlyReactToNail;

	public bool noHarpoonHook;

	[SerializeField]
	private ITinkResponder.TinkFlags tinkProperties;

	[SerializeField]
	private ITinkResponder.TinkFlags ignoreResponders;

	[Obsolete]
	[HideInInspector]
	public bool onlyReactToDown;

	[SerializeField]
	[EnumPickerBitmask(typeof(HitInstance.HitDirection))]
	private int directionMask = -1;

	[SerializeField]
	private bool checkSlashPosition;

	[SerializeField]
	private bool activeOnAnyPlane;

	[SerializeField]
	[EnumPickerBitmask(typeof(HitInstance.HitDirection))]
	private int preventDamageDirection;

	[SerializeField]
	private List<HealthManager> preventDamageHealthManagers = new List<HealthManager>();

	[Space]
	public UnityEvent OnTinked;

	[Space]
	public UnityEvent OnTinkedHeavy;

	[Space]
	public UnityEvent OnTinkedUp;

	public UnityEvent OnTinkedDown;

	public UnityEvent OnTinkedLeft;

	public UnityEvent OnTinkedRight;

	private Collider2D collider;

	private bool hasCollider;

	private DamageHero heroDamager;

	private GameCameras gameCam;

	private IHitResponderOverride overrideResponder;

	private const float REPEAT_DELAY = 0.01f;

	private static double _nextTinkTime;

	public int HitPriority => hitPriority;

	public IHitResponderOverride OverrideResponder
	{
		get
		{
			return overrideResponder;
		}
		set
		{
			overrideResponder = value;
		}
	}

	public bool HitRecurseUpwards => !IsActive;

	public override bool IsActive
	{
		get
		{
			return isActive;
		}
		set
		{
			isActive = value;
		}
	}

	public event TinkEffectSpawnEvent OnSpawnedTink;

	private bool? IsFsmEventValid(string eventName)
	{
		if (!fsm || string.IsNullOrEmpty(eventName))
		{
			return null;
		}
		return fsm.FsmEvents.Any((FsmEvent fsmEvent) => fsmEvent.Name.Equals(eventName));
	}

	private void OnValidate()
	{
		if (onlyReactToDown)
		{
			directionMask = 0.SetBitAtIndex(3);
			onlyReactToDown = false;
		}
	}

	protected override void Awake()
	{
		base.Awake();
		OnValidate();
		collider = GetComponent<Collider2D>();
		heroDamager = GetComponent<DamageHero>();
		if (!base.transform.IsOnHeroPlane() && !activeOnAnyPlane)
		{
			base.enabled = false;
		}
		preventDamageHealthManagers.RemoveAll((HealthManager o) => o == null);
	}

	private void OnEnable()
	{
		AddDebugDrawComponent();
	}

	public IHitResponder.HitResponse Hit(HitInstance damageInstance)
	{
		if (overrideResponder != null && overrideResponder.WillRespond(damageInstance))
		{
			return IHitResponder.Response.None;
		}
		if (!damageInstance.Source)
		{
			return IHitResponder.Response.None;
		}
		DamageEnemies component = damageInstance.Source.GetComponent<DamageEnemies>();
		if (!component)
		{
			return IHitResponder.Response.None;
		}
		return TryDoTinkReaction(component, doCamShake: true, doSound: true) ? IHitResponder.Response.GenericHit : IHitResponder.Response.None;
	}

	public bool TryDoTinkReaction(Collider2D collision, bool doCamShake, bool doSound)
	{
		if (!isActive)
		{
			return false;
		}
		GameObject gameObject = collision.gameObject;
		if (gameObject.layer != 17)
		{
			return false;
		}
		DamageEnemies component = gameObject.GetComponent<DamageEnemies>();
		if (!component)
		{
			return false;
		}
		return TryDoTinkReaction(component, doCamShake, doSound);
	}

	private bool CanTink(DamageEnemies damager)
	{
		NailSlashTerrainThunk componentInChildren = damager.GetComponentInChildren<NailSlashTerrainThunk>();
		if ((bool)componentInChildren && componentInChildren.WillHandleTink(collider))
		{
			return false;
		}
		if (!damager.doesNotTink)
		{
			return true;
		}
		return (tinkProperties & damager.AllowedTinkFlags) != 0;
	}

	private bool TryDoTinkReaction(DamageEnemies damager, bool doCamShake, bool doSound)
	{
		if (damager.attackType == AttackTypes.Coal)
		{
			return false;
		}
		if (!CanTink(damager))
		{
			return false;
		}
		GameObject gameObject = damager.gameObject;
		bool flag = gameObject.CompareTag("Nail Attack");
		if (damager.doesNotTinkThroughWalls && (bool)collider)
		{
			HeroController instance = HeroController.instance;
			Vector3 position = gameObject.transform.position;
			Vector3 vector = (flag ? instance.transform.position : position);
			Vector2 to = collider.ClosestPoint(vector);
			if (Helper.IsLineHittingNoTriggers(vector, to, 256, null, out var _))
			{
				return false;
			}
		}
		Vector2 tinkPosition;
		return TryDoTinkReactionNoDamager(gameObject, doCamShake, doSound, flag, out tinkPosition);
	}

	public bool TryDoTinkReactionNoDamager(GameObject obj, bool doCamShake, bool doSound, bool isNailAttack, out Vector2 tinkPosition)
	{
		bool flag = true;
		bool flag2 = Time.timeAsDouble >= _nextTinkTime;
		NailAttackBase component = obj.GetComponent<NailAttackBase>();
		bool flag3 = isNailAttack && (bool)component && !component.CanHitSpikes;
		HeroController instance = HeroController.instance;
		Vector3 position = obj.transform.position;
		tinkPosition = position;
		Vector3 vector = (isNailAttack ? instance.transform.position : position);
		if (useOwnYPosition)
		{
			position.y = (vector.y = base.transform.position.y);
		}
		DamageEnemies component2 = obj.GetComponent<DamageEnemies>();
		bool flag4 = component2 != null;
		if (!flag4)
		{
			component2 = obj.transform.parent.gameObject.GetComponent<DamageEnemies>();
			flag4 = component2 != null;
			if (!flag4)
			{
				return false;
			}
		}
		float actualHitDirection;
		if (isNailAttack)
		{
			if ((bool)heroDamager && heroDamager.hazardType == HazardType.SPIKES && flag3)
			{
				instance.TakeDamage(heroDamager.gameObject, CollisionSide.top, heroDamager.damageDealt, HazardType.SPIKES, heroDamager.damagePropertyFlags);
				return false;
			}
			actualHitDirection = GetActualHitDirection(component2);
		}
		else
		{
			actualHitDirection = GetActualHitDirection(component2);
			ITinkResponder component3 = obj.GetComponent<ITinkResponder>();
			bool flag5 = component3 != null;
			if (flag5)
			{
				if ((ignoreResponders & component3.ResponderType) != 0)
				{
					return false;
				}
				component3.Tinked();
			}
			if (onlyReactToNail)
			{
				if (!flag5)
				{
					return false;
				}
				flag = false;
			}
		}
		int cardinalDirection = DirectionUtils.GetCardinalDirection(actualHitDirection);
		switch (cardinalDirection)
		{
		case 3:
			if (!directionMask.IsBitSet(3))
			{
				return false;
			}
			TryPreventDamage(component2, 3);
			break;
		case 1:
			if (!directionMask.IsBitSet(2))
			{
				return false;
			}
			TryPreventDamage(component2, 2);
			break;
		case 2:
			if (!directionMask.IsBitSet(0))
			{
				return false;
			}
			if (isNailAttack && checkSlashPosition && obj.transform.position.x < base.transform.position.x)
			{
				return false;
			}
			TryPreventDamage(component2, 0);
			break;
		case 0:
			if (!directionMask.IsBitSet(1))
			{
				return false;
			}
			if (isNailAttack && checkSlashPosition && obj.transform.position.x > base.transform.position.x)
			{
				return false;
			}
			TryPreventDamage(component2, 1);
			break;
		}
		if (flag2 && RecoilHero && flag4)
		{
			component2.OnTinkEffectTink();
		}
		bool flag6 = base.gameObject.GetComponent<NonBouncer>();
		if (flag4)
		{
			int layer = base.gameObject.layer;
			if ((layer == 11 || layer == 17 || layer == 19) && !flag6)
			{
				component2.OnBounceableTink();
				switch (cardinalDirection)
				{
				case 3:
					component2.OnBounceableTinkDown();
					break;
				case 1:
					component2.OnBounceableTinkUp();
					break;
				case 2:
					component2.OnBounceableTinkLeft();
					break;
				case 0:
					component2.OnBounceableTinkRight();
					break;
				}
			}
		}
		if (isNailAttack)
		{
			if (flag2)
			{
				_nextTinkTime = Time.timeAsDouble + 0.009999999776482582;
			}
			if (!gameCam)
			{
				gameCam = GameCameras.instance;
			}
			if (doCamShake && flag2 && (bool)gameCam)
			{
				if (overrideCamShake)
				{
					camShakeOverride.DoShake(this);
				}
				else
				{
					gameCam.cameraShakeFSM.SendEvent("EnemyKillShake");
				}
			}
		}
		Vector3 euler = new Vector3(0f, 0f, 0f);
		bool flag7 = collider != null;
		if (useNailPosition && (!flag4 || !component2.IgnoreNailPosition))
		{
			flag7 = false;
		}
		Vector2 vector2 = Vector2.zero;
		float num = 0f;
		float num2 = 0f;
		if (flag7)
		{
			Bounds bounds = collider.bounds;
			vector2 = base.transform.TransformPoint(collider.offset);
			num = bounds.size.x * 0.5f;
			num2 = bounds.size.y * 0.5f;
		}
		Vector3 vector3;
		switch (cardinalDirection)
		{
		case 0:
			if (isNailAttack && flag2 && RecoilHero)
			{
				instance.RecoilLeft();
			}
			if (flag)
			{
				if (sendDirectionalFSMEvents && (bool)fsm)
				{
					fsm.SendEvent("TINK RIGHT");
				}
				SendHitInDirection(obj, HitInstance.HitDirection.Right);
			}
			if (flag7)
			{
				float num5 = Mathf.Max(0f, num2 - 1.5f);
				position.y = Mathf.Clamp(position.y, vector2.y - num5, vector2.y + num5);
				vector3 = new Vector3(vector2.x - num, position.y, 0.002f);
			}
			else
			{
				vector3 = ((!isNailAttack) ? new Vector3(vector.x, vector.y, 0.002f) : new Vector3(vector.x + 2f, vector.y, 0.002f));
			}
			break;
		case 1:
			if (isNailAttack && flag2 && RecoilHero)
			{
				instance.RecoilDown();
			}
			if (flag)
			{
				if (sendDirectionalFSMEvents && (bool)fsm)
				{
					fsm.SendEvent("TINK UP");
				}
				SendHitInDirection(obj, HitInstance.HitDirection.Up);
			}
			vector3 = (flag7 ? new Vector3(position.x, Mathf.Max(vector2.y - num2, position.y), 0.002f) : ((!isNailAttack) ? new Vector3(vector.x, vector.y, 0.002f) : new Vector3(vector.x, vector.y + 2f, 0.002f)));
			euler = new Vector3(0f, 0f, 90f);
			break;
		case 2:
			if (isNailAttack && flag2 && RecoilHero)
			{
				instance.RecoilRight();
			}
			if (flag)
			{
				if (sendDirectionalFSMEvents && (bool)fsm)
				{
					fsm.SendEvent("TINK LEFT");
				}
				SendHitInDirection(obj, HitInstance.HitDirection.Left);
			}
			if (!flag7)
			{
				vector3 = ((!isNailAttack) ? new Vector3(vector.x, vector.y, 0.002f) : new Vector3(vector.x - 2f, vector.y, 0.002f));
			}
			else
			{
				float num4 = Mathf.Max(0f, num2 - 1.5f);
				position.y = Mathf.Clamp(position.y, vector2.y - num4, vector2.y + num4);
				vector3 = new Vector3(vector2.x + num, position.y, 0.002f);
			}
			euler = new Vector3(0f, 0f, 180f);
			break;
		default:
			if (flag)
			{
				if (sendDirectionalFSMEvents && (bool)fsm)
				{
					fsm.SendEvent("TINK DOWN");
					if (isNailAttack)
					{
						fsm.SendEvent(instance.cState.facingRight ? "TINK DOWN R" : "TINK DOWN L");
					}
				}
				SendHitInDirection(obj, HitInstance.HitDirection.Down);
			}
			if (!flag7)
			{
				vector3 = ((!isNailAttack) ? new Vector3(vector.x, vector.y, 0.002f) : new Vector3(vector.x, vector.y - 2f, 0.002f));
			}
			else
			{
				float num3 = position.x;
				if (num3 < vector2.x - num)
				{
					num3 = vector2.x - num;
				}
				if (num3 > vector2.x + num)
				{
					num3 = vector2.x + num;
				}
				vector3 = new Vector3(num3, Mathf.Min(vector2.y + num2, position.y), 0.002f);
			}
			euler = new Vector3(0f, 0f, 270f);
			break;
		}
		GameObject gameObject = (flag3 ? Effects.TinkEffectDullPrefab : blockEffect);
		if (flag2)
		{
			Quaternion rotation = Quaternion.Euler(euler);
			if ((bool)gameObject)
			{
				AudioSource component4 = gameObject.Spawn(vector3, rotation).GetComponent<AudioSource>();
				if ((bool)component4)
				{
					component4.pitch = UnityEngine.Random.Range(0.85f, 1.15f);
					component4.volume = (doSound ? 1f : 0f);
				}
			}
			this.OnSpawnedTink?.Invoke(vector3, rotation);
		}
		tinkPosition = vector3;
		if (!flag)
		{
			return true;
		}
		if (sendFSMEvent && (bool)fsm)
		{
			fsm.SendEvent(FSMEvent);
		}
		OnTinked.Invoke();
		if (flag4 && component2.attackType == AttackTypes.Heavy)
		{
			OnTinkedHeavy.Invoke();
		}
		switch (cardinalDirection)
		{
		case 3:
			OnTinkedDown.Invoke();
			break;
		case 1:
			OnTinkedUp.Invoke();
			break;
		case 2:
			OnTinkedLeft.Invoke();
			break;
		case 0:
			OnTinkedRight.Invoke();
			break;
		}
		return true;
	}

	private void TryPreventDamage(DamageEnemies damager, int bit)
	{
		if (!damager || !preventDamageDirection.IsBitSet(bit))
		{
			return;
		}
		foreach (HealthManager preventDamageHealthManager in preventDamageHealthManagers)
		{
			damager.PreventDamage(preventDamageHealthManager);
		}
	}

	private float GetActualHitDirection(DamageEnemies damager)
	{
		if (!damager)
		{
			return 0f;
		}
		if (!damager.CircleDirection)
		{
			return damager.GetDirection();
		}
		Vector2 vector = (Vector2)base.transform.position - (Vector2)damager.transform.position;
		return Mathf.Atan2(vector.y, vector.x) * 57.29578f;
	}

	public void SetFsmEvent(string eventName)
	{
		sendFSMEvent = true;
		fsm = GetComponent<PlayMakerFSM>();
		FSMEvent = eventName;
	}

	public void SetSendDirectionalFSMEvents(bool set)
	{
		sendDirectionalFSMEvents = set;
	}
}

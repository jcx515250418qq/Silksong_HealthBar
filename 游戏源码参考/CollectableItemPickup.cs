using System.Collections;
using TeamCherry.NestedFadeGroup;
using TeamCherry.SharedUtils;
using UnityEngine;
using UnityEngine.Events;

public class CollectableItemPickup : MonoBehaviour
{
	public enum PickupAnimations
	{
		Normal = 0,
		Stand = 1
	}

	private enum FlingDirection
	{
		Either = 0,
		Left = 1,
		Right = 2,
		Drop = 3,
		AwayFromHero = 4
	}

	private static readonly Vector2 KNEELING_PICKUP_OFFSET = new Vector2(0f, -0.76f);

	[SerializeField]
	private InteractEvents interactEvents;

	[SerializeField]
	[ModifiableProperty]
	[Conditional("interactEvents", false, false, false)]
	private TrackTriggerObjects pickupTrigger;

	[SerializeField]
	private bool waitForStoppedMoving;

	[SerializeField]
	private float canPickupDelay;

	[SerializeField]
	private bool ignoreCanExist;

	[SerializeField]
	[ModifiableProperty]
	[Conditional("IsPersistenceHandled", false, true, false)]
	private PersistentBoolItem persistent;

	[SerializeField]
	private SpriteRenderer spriteRenderer;

	[SerializeField]
	private ParticleSystem flingTrail;

	[Space]
	public UnityEvent OnPickup;

	public UnityEvent OnPickupEnd;

	public UnityEvent OnPickedUp;

	public UnityEvent OnPreviouslyPickedUp;

	[Space]
	[SerializeField]
	private SavedItem item;

	[SerializeField]
	private bool showPopup = true;

	[SerializeField]
	[PlayerDataField(typeof(bool), false)]
	private string playerDataBool;

	[SerializeField]
	[ModifiableProperty]
	[Conditional("interactEvents", true, false, false)]
	private PickupAnimations pickupAnim;

	[SerializeField]
	private bool fling;

	[SerializeField]
	[ModifiableProperty]
	[Conditional("fling", true, false, false)]
	private FlingDirection flingDirection;

	[SerializeField]
	private bool smallGetEffect;

	[SerializeField]
	private GameObject extraEffects;

	[SerializeField]
	private GameObject pickupEffect;

	private bool activated;

	private double canPickupTime;

	private bool hasStarted;

	private HeroController subscribedHc;

	private Coroutine pickupRoutine;

	private bool wasInsidePickupTrigger;

	private Rigidbody2D body;

	private bool didCancelGravity;

	private float previousGravity;

	private RigidbodyType2D bodyType;

	public static bool IsPickupPaused { get; set; }

	private bool IsWaitingForStoppedMoving
	{
		get
		{
			if (waitForStoppedMoving && (bool)body)
			{
				return interactEvents;
			}
			return false;
		}
	}

	public SavedItem Item => item;

	public PickupAnimations PickupAnim
	{
		get
		{
			return pickupAnim;
		}
		set
		{
			pickupAnim = value;
		}
	}

	public bool IsPersistenceHandled()
	{
		if (!item || !item.IsUnique)
		{
			return !string.IsNullOrEmpty(playerDataBool);
		}
		return true;
	}

	private void Awake()
	{
		if ((bool)persistent)
		{
			if ((bool)item && item.IsUnique)
			{
				Object.Destroy(persistent);
				persistent = null;
			}
			else
			{
				persistent.OnGetSaveState += OnGetSaveState;
				persistent.OnSetSaveState += OnSetSaveState;
			}
		}
		if ((bool)interactEvents)
		{
			interactEvents.Interacted += DoPickup;
		}
		PickupAnim = PickupAnim;
		SetPlayerDataBool(playerDataBool);
		body = GetComponent<Rigidbody2D>();
		if ((bool)body && pickupAnim == PickupAnimations.Stand)
		{
			body.bodyType = RigidbodyType2D.Kinematic;
		}
	}

	private void OnEnable()
	{
		if (hasStarted)
		{
			Setup();
			if (didCancelGravity && (bool)body)
			{
				body.gravityScale = previousGravity;
				didCancelGravity = false;
				body.bodyType = bodyType;
			}
		}
	}

	private void OnDisable()
	{
		CancelPickup();
	}

	private void Start()
	{
		if (!ignoreCanExist && !GameManager.instance.CanPickupsExist())
		{
			base.gameObject.Recycle();
			return;
		}
		if (!body)
		{
			body = GetComponentInParent<Rigidbody2D>();
		}
		Setup();
		hasStarted = true;
		if (fling)
		{
			FlingSelf();
		}
	}

	private void Update()
	{
		if ((bool)pickupTrigger)
		{
			bool isInside = pickupTrigger.IsInside;
			if (isInside && !wasInsidePickupTrigger)
			{
				DoPickupInstant();
			}
			wasInsidePickupTrigger = isInside;
		}
		if (!IsWaitingForStoppedMoving || activated)
		{
			return;
		}
		if (body.linearVelocity.magnitude < 0.1f)
		{
			if (Time.timeAsDouble >= canPickupTime && interactEvents.IsDisabled)
			{
				interactEvents.Activate();
			}
		}
		else if (pickupRoutine != null)
		{
			CancelPickup();
		}
		else
		{
			ResetPickupDelay();
		}
	}

	private void Setup()
	{
		CheckActivation();
		ResetPickupDelay();
		if (!activated && (bool)spriteRenderer)
		{
			NestedFadeGroup component = spriteRenderer.GetComponent<NestedFadeGroup>();
			if ((bool)component)
			{
				component.AlphaSelf = 1f;
			}
			else
			{
				spriteRenderer.enabled = true;
			}
		}
	}

	private void ResetPickupDelay()
	{
		canPickupTime = Time.timeAsDouble + (double)canPickupDelay;
		if (IsWaitingForStoppedMoving && !interactEvents.IsDisabled)
		{
			interactEvents.Deactivate(allowQueueing: false);
		}
	}

	private void DoPickup()
	{
		if (activated || Time.timeAsDouble < canPickupTime)
		{
			EndInteraction(didPickup: false);
		}
		else
		{
			pickupRoutine = StartCoroutine(Pickup());
		}
	}

	private void DoPickupInstant()
	{
		if (!activated && !(Time.timeAsDouble < canPickupTime))
		{
			if (DoPickupAction(breakIfAtMax: true))
			{
				activated = true;
				return;
			}
			Debug.LogErrorFormat(this, "Couldn't pickup item {0}", item ? item.name : "null");
		}
	}

	private bool DoPickupAction(bool breakIfAtMax)
	{
		bool flag = true;
		if (!string.IsNullOrEmpty(playerDataBool))
		{
			PlayerData.instance.SetVariable(playerDataBool, value: true);
			flag = false;
		}
		bool flag2 = false;
		if ((bool)item)
		{
			flag2 = !item.CanGetMore();
			if (!item.TryGet(breakIfAtMax, showPopup))
			{
				return false;
			}
		}
		else if (flag)
		{
			Debug.LogError("No collectable item assigned!", this);
			return false;
		}
		if (OnPickup != null)
		{
			OnPickup.Invoke();
		}
		if (OnPickedUp != null)
		{
			OnPickedUp.Invoke();
		}
		if ((bool)spriteRenderer)
		{
			NestedFadeGroup component = spriteRenderer.GetComponent<NestedFadeGroup>();
			if ((bool)component)
			{
				component.AlphaSelf = 0f;
			}
			else
			{
				spriteRenderer.enabled = false;
			}
		}
		if ((bool)extraEffects)
		{
			extraEffects.SetActive(value: false);
		}
		if ((bool)pickupEffect)
		{
			pickupEffect.SetActive(value: true);
		}
		if (!showPopup)
		{
			CollectableItemHeroReaction.DoReaction(new Vector2(0f, -0.76f), smallGetEffect);
		}
		else if (flag2)
		{
			CollectableItemHeroReaction.DoReaction();
		}
		StopRBMovement();
		return true;
	}

	private void StopRBMovement()
	{
		if ((bool)body)
		{
			if (!didCancelGravity)
			{
				previousGravity = body.gravityScale;
				body.gravityScale = 0f;
				didCancelGravity = true;
				bodyType = body.bodyType;
				body.bodyType = RigidbodyType2D.Static;
			}
			body.linearVelocity = Vector2.zero;
		}
	}

	public void CancelPickup()
	{
		if (pickupRoutine != null)
		{
			StopCoroutine(pickupRoutine);
			pickupRoutine = null;
			EndInteraction(activated);
			ResetPickupDelay();
			HeroController.instance.StartAnimationControl();
		}
	}

	private IEnumerator Pickup()
	{
		while (HeroTalkAnimation.IsEndingHurtAnim)
		{
			yield return null;
		}
		HeroController instance = HeroController.instance;
		instance.OnTakenDamage += CancelPickup;
		subscribedHc = instance;
		instance.StopAnimationControl();
		tk2dSpriteAnimator animator = instance.GetComponent<tk2dSpriteAnimator>();
		HeroAnimationController heroAnim = instance.GetComponent<HeroAnimationController>();
		animator.Play((pickupAnim == PickupAnimations.Normal) ? heroAnim.GetClip("Collect Normal 1") : heroAnim.GetClip("Collect Stand 1"));
		yield return new WaitForSeconds(0.75f);
		if (pickupAnim == PickupAnimations.Normal)
		{
			CollectableItemHeroReaction.NextEffectOffset = KNEELING_PICKUP_OFFSET;
		}
		bool didPickup = DoPickupAction(breakIfAtMax: false);
		animator.Play((pickupAnim == PickupAnimations.Normal) ? heroAnim.GetClip("Collect Normal 2") : heroAnim.GetClip("Collect Stand 2"));
		if (didPickup)
		{
			FSMUtility.SendEventUpwards(base.gameObject, "COLLECTED");
			activated = true;
			yield return new WaitForSeconds(0.5f);
		}
		if (pickupAnim == PickupAnimations.Normal)
		{
			CollectableItemHeroReaction.NextEffectOffset = Vector2.zero;
		}
		if (didPickup && pickupAnim != PickupAnimations.Stand)
		{
			while (IsPickupPaused)
			{
				yield return null;
			}
		}
		yield return StartCoroutine(animator.PlayAnimWait((pickupAnim == PickupAnimations.Normal) ? heroAnim.GetClip("Collect Normal 3") : heroAnim.GetClip("Collect Stand 3")));
		HeroController.instance.StartAnimationControl();
		if (didPickup && pickupAnim == PickupAnimations.Stand)
		{
			while (IsPickupPaused)
			{
				yield return null;
			}
		}
		EndInteraction(didPickup);
		pickupRoutine = null;
		OnPickupEnd.Invoke();
	}

	private void EndInteraction(bool didPickup)
	{
		if ((bool)subscribedHc)
		{
			subscribedHc.OnTakenDamage -= CancelPickup;
			subscribedHc = null;
		}
		if ((bool)interactEvents)
		{
			interactEvents.EndInteraction();
			if (didPickup)
			{
				interactEvents.Deactivate(allowQueueing: false);
			}
		}
	}

	public void SetItem(SavedItem newItem, bool keepPersistence = false)
	{
		item = newItem;
		if (!keepPersistence && Application.isPlaying && (bool)persistent)
		{
			Object.Destroy(persistent);
		}
	}

	public void SetPlayerDataBool(string boolName)
	{
		playerDataBool = boolName;
		if (string.IsNullOrEmpty(playerDataBool))
		{
			return;
		}
		if ((bool)persistent)
		{
			persistent.OnGetSaveState -= OnGetSaveState;
			persistent.OnSetSaveState -= OnSetSaveState;
			persistent = null;
		}
		if (PlayerData.instance.GetVariable<bool>(playerDataBool))
		{
			if (OnPickedUp != null)
			{
				OnPickedUp.Invoke();
			}
			if (OnPreviouslyPickedUp != null)
			{
				OnPreviouslyPickedUp.Invoke();
			}
			base.gameObject.SetActive(value: false);
		}
	}

	private void OnGetSaveState(out bool value)
	{
		value = activated;
	}

	private void OnSetSaveState(bool value)
	{
		activated = value;
		CheckActivation();
	}

	private void CheckActivation()
	{
		if (activated)
		{
			if (string.IsNullOrEmpty(playerDataBool) && persistent == null && (item == null || (!item.IsUnique && item.CanGetMore())))
			{
				activated = false;
				return;
			}
		}
		else
		{
			activated = (bool)item && !item.CanGetMore();
		}
		if (activated)
		{
			if (OnPickedUp != null)
			{
				OnPickedUp.Invoke();
			}
			if (OnPreviouslyPickedUp != null)
			{
				OnPreviouslyPickedUp.Invoke();
			}
			base.gameObject.SetActive(value: false);
		}
	}

	public void SetFling(bool setFling)
	{
		fling = setFling;
	}

	public void SetFlingLeft()
	{
		flingDirection = FlingDirection.Left;
	}

	public void SetFlingRight()
	{
		flingDirection = FlingDirection.Right;
	}

	public void FlingSelf()
	{
		if (flingDirection == FlingDirection.Drop)
		{
			FlingSelf(new MinMaxFloat(0f, 0f), new MinMaxFloat(0f, 0f));
			return;
		}
		if (flingDirection == FlingDirection.Either)
		{
			flingDirection = ((Random.Range(0, 2) <= 0) ? FlingDirection.Left : FlingDirection.Right);
		}
		if (flingDirection == FlingDirection.AwayFromHero)
		{
			flingDirection = ((base.transform.position.x < HeroController.instance.transform.position.x) ? FlingDirection.Left : FlingDirection.Right);
		}
		float num = ((flingDirection == FlingDirection.Right) ? 80f : 100f);
		FlingSelf(new MinMaxFloat(22f, 22f), new MinMaxFloat(num, num));
	}

	public void FlingSelf(MinMaxFloat speed, MinMaxFloat angle)
	{
		FlingSelf obj = GetComponent<FlingSelf>() ?? base.gameObject.AddComponent<FlingSelf>();
		obj.speedMin = speed.Start;
		obj.speedMax = speed.End;
		obj.angleMin = angle.Start;
		obj.angleMax = angle.End;
		ObjectBounce component = GetComponent<ObjectBounce>();
		if (!component)
		{
			return;
		}
		if ((bool)interactEvents)
		{
			interactEvents.Deactivate(allowQueueing: false);
			component.StartedMoving += delegate
			{
				interactEvents.Deactivate(allowQueueing: false);
			};
			component.StoppedMoving += delegate
			{
				interactEvents.Activate();
			};
		}
		if ((bool)flingTrail)
		{
			flingTrail.Stop(withChildren: true, ParticleSystemStopBehavior.StopEmittingAndClear);
			component.StartedMoving += delegate
			{
				flingTrail.Play(withChildren: true);
			};
			component.StoppedMoving += delegate
			{
				flingTrail.Stop(withChildren: true, ParticleSystemStopBehavior.StopEmitting);
			};
		}
	}

	public void SetActivation(bool value)
	{
		activated = value;
	}
}

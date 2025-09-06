using System;
using System.Collections;
using TeamCherry.SharedUtils;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Serialization;

public class Lever : MonoBehaviour, IHitResponder
{
	private readonly int hitDirectionAnimParam = Animator.StringToHash("Hit Direction");

	private readonly int hitAnimParam = Animator.StringToHash("Hit");

	private readonly int activateAnimParam = Animator.StringToHash("Activate");

	private readonly int retractedAnimParam = Animator.StringToHash("Retracted");

	private readonly int retractAnimId = Animator.StringToHash("Retract");

	[SerializeField]
	private PersistentBoolItem persistent;

	[SerializeField]
	[PlayerDataField(typeof(bool), false)]
	private string playerDataBool;

	[SerializeField]
	private Animator animator;

	[Space]
	[SerializeField]
	private GameObject hitEffect;

	[SerializeField]
	private Transform hitEffectPoint;

	[SerializeField]
	private CameraShakeTarget hitCameraShake;

	[SerializeField]
	private AudioEvent hitSound;

	[SerializeField]
	private GameObject activatedTinker;

	[SerializeField]
	private float openGateDelay = 1f;

	[SerializeField]
	private bool doesNotActivate;

	[SerializeField]
	[ModifiableProperty]
	[Conditional("doesNotActivate", true, false, false)]
	private bool setHitBlocked;

	[SerializeField]
	private bool retractAfterHit;

	[SerializeField]
	private TrackTriggerObjects canHitTrigger;

	[SerializeField]
	private NonBouncer nonBouncer;

	[SerializeField]
	private bool recoilHero;

	[SerializeField]
	private string sendEventToRegister;

	[Space]
	[SerializeField]
	[FormerlySerializedAs("connectedGates")]
	private UnlockablePropBase[] unlockables;

	[SerializeField]
	private PlayMakerFSM[] fsmGates;

	[Space]
	public UnityEvent OnHit;

	public UnityEvent OnHitDelayed;

	public UnityEvent OnActivated;

	public UnityEvent OnStartActivated;

	private bool hitBlocked;

	private bool activated;

	public bool HitBlocked
	{
		get
		{
			return hitBlocked;
		}
		set
		{
			hitBlocked = value;
			if ((bool)nonBouncer)
			{
				nonBouncer.active = value;
			}
		}
	}

	private void Awake()
	{
		if (persistent != null)
		{
			if (!string.IsNullOrEmpty(playerDataBool))
			{
				UnityEngine.Object.Destroy(persistent);
				persistent = null;
			}
			else
			{
				persistent.OnGetSaveState += delegate(out bool value)
				{
					value = activated;
				};
				persistent.OnSetSaveState += delegate(bool value)
				{
					if (value)
					{
						SetActivated(fromStart: true);
					}
				};
			}
		}
		if ((bool)activatedTinker)
		{
			activatedTinker.SetActive(value: false);
		}
		HitBlocked = HitBlocked;
	}

	private void Start()
	{
		if (!string.IsNullOrEmpty(playerDataBool) && PlayerData.instance.GetVariable<bool>(playerDataBool))
		{
			SetActivated();
		}
	}

	public IHitResponder.HitResponse Hit(HitInstance damageInstance)
	{
		if (!damageInstance.IsFirstHit)
		{
			return IHitResponder.Response.None;
		}
		if (activated || HitBlocked)
		{
			return IHitResponder.Response.None;
		}
		if ((bool)canHitTrigger && !canHitTrigger.IsInside)
		{
			return IHitResponder.Response.None;
		}
		if (!damageInstance.IsNailDamage)
		{
			return IHitResponder.Response.None;
		}
		OnHit.Invoke();
		if (!doesNotActivate)
		{
			activated = true;
			HitBlocked = true;
			if (!string.IsNullOrEmpty(playerDataBool))
			{
				PlayerData.instance.SetVariable(playerDataBool, value: true);
			}
		}
		else if (setHitBlocked)
		{
			HitBlocked = true;
		}
		HeroController instance = HeroController.instance;
		float num = 0f;
		bool flag = false;
		switch (damageInstance.GetHitDirection(HitInstance.TargetType.Regular))
		{
		case HitInstance.HitDirection.Left:
			num = -1f;
			flag = true;
			if (recoilHero && damageInstance.AttackType == AttackTypes.Nail)
			{
				instance.RecoilRight();
			}
			break;
		case HitInstance.HitDirection.Right:
			num = 1f;
			flag = true;
			if (recoilHero && damageInstance.AttackType == AttackTypes.Nail)
			{
				instance.RecoilLeft();
			}
			break;
		case HitInstance.HitDirection.Up:
			num = 1f;
			if (recoilHero && damageInstance.IsNailDamage)
			{
				instance.RecoilDown();
			}
			break;
		case HitInstance.HitDirection.Down:
			num = -1f;
			break;
		default:
			throw new ArgumentOutOfRangeException();
		}
		if ((bool)animator)
		{
			animator.cullingMode = AnimatorCullingMode.AlwaysAnimate;
			if (animator.HasParameter(hitDirectionAnimParam, null))
			{
				float z = base.transform.eulerAngles.z;
				if (flag)
				{
					if (z > 90f && z < 270f)
					{
						num *= -1f;
					}
				}
				else if (!(z > 10f) || !(z < 350f))
				{
					num = (instance.cState.facingRight ? 1 : (-1));
				}
				else if (z > 170f && z < 190f)
				{
					num = ((!instance.cState.facingRight) ? 1 : (-1));
				}
				else if (z > 180f)
				{
					num *= -1f;
				}
				if (base.transform.lossyScale.x < 0f)
				{
					num *= -1f;
				}
				animator.SetFloat(hitDirectionAnimParam, num);
			}
			if (animator.HasParameter(hitAnimParam, null))
			{
				animator.SetTrigger(hitAnimParam);
			}
			if (retractAfterHit)
			{
				animator.SetBool(retractedAnimParam, value: true);
			}
		}
		StartCoroutine(Execute());
		if ((bool)hitEffect)
		{
			hitEffect.Spawn(hitEffectPoint ? hitEffectPoint.position : base.transform.position);
		}
		hitCameraShake.DoShake(this);
		hitSound.SpawnAndPlayOneShot(base.transform.position);
		return IHitResponder.Response.GenericHit;
	}

	private IEnumerator Execute()
	{
		yield return new WaitForSeconds(openGateDelay);
		if ((bool)activatedTinker && !retractAfterHit)
		{
			activatedTinker.SetActive(value: true);
		}
		OnHitDelayed.Invoke();
		UnlockablePropBase[] array = unlockables;
		foreach (UnlockablePropBase unlockablePropBase in array)
		{
			if ((bool)unlockablePropBase && unlockablePropBase.gameObject.activeInHierarchy)
			{
				unlockablePropBase.Open();
			}
		}
		PlayMakerFSM[] array2 = fsmGates;
		foreach (PlayMakerFSM playMakerFSM in array2)
		{
			if ((bool)playMakerFSM)
			{
				playMakerFSM.SendEvent("OPEN");
			}
		}
		if (!string.IsNullOrEmpty(sendEventToRegister))
		{
			EventRegister.SendEvent(sendEventToRegister);
		}
		OnActivated.Invoke();
	}

	public void SetActivated()
	{
		SetActivated(fromStart: false);
	}

	public void SetActivated(bool fromStart)
	{
		if (activated)
		{
			return;
		}
		activated = true;
		if ((bool)activatedTinker && !retractAfterHit)
		{
			activatedTinker.SetActive(value: true);
		}
		if (fromStart)
		{
			OnStartActivated.Invoke();
		}
		else
		{
			OnActivated.Invoke();
		}
		UnlockablePropBase[] array = unlockables;
		foreach (UnlockablePropBase unlockablePropBase in array)
		{
			if ((bool)unlockablePropBase && unlockablePropBase.gameObject.activeInHierarchy)
			{
				unlockablePropBase.Opened();
			}
		}
		PlayMakerFSM[] array2 = fsmGates;
		foreach (PlayMakerFSM playMakerFSM in array2)
		{
			if ((bool)playMakerFSM)
			{
				playMakerFSM.SendEvent("ACTIVATE");
			}
		}
		SetActivatedAnim();
		HitBlocked = true;
	}

	public void SetActivatedInert(bool value)
	{
		activated = value;
		HitBlocked = value;
	}

	public void SetActivatedInertWithAnim(bool value)
	{
		activated = value;
		HitBlocked = value;
		SetActivatedAnim();
	}

	private void SetActivatedAnim()
	{
		if ((bool)animator)
		{
			if (animator.HasParameter(activateAnimParam, null))
			{
				animator.cullingMode = AnimatorCullingMode.AlwaysAnimate;
				animator.SetTrigger(activateAnimParam);
			}
			else if (retractAfterHit)
			{
				animator.cullingMode = AnimatorCullingMode.AlwaysAnimate;
				animator.SetBool(retractedAnimParam, value: true);
				animator.Play(retractAnimId, 0, 1f);
			}
			else
			{
				animator.gameObject.SetActive(value: false);
			}
		}
	}
}

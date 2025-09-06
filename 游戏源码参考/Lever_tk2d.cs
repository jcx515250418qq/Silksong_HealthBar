using System;
using System.Collections;
using UnityEngine;
using UnityEngine.Events;

public class Lever_tk2d : MonoBehaviour, IHitResponder
{
	[SerializeField]
	private UnlockablePropBase[] connectedGates;

	[SerializeField]
	[ModifiableProperty]
	[InspectorValidation("IsFsmEventValidRequiredFSM")]
	private PlayMakerFSM[] fsmGates;

	[SerializeField]
	private GameObject[] camLocks;

	[SerializeField]
	[ModifiableProperty]
	[InspectorValidation("IsFsmEventValidRequired")]
	private string fsmActivateEvent = "ACTIVATE";

	[SerializeField]
	[PlayerDataField(typeof(bool), false)]
	private string setPlayerDataBool;

	[SerializeField]
	private string sendEventToRegister;

	[Space]
	[SerializeField]
	private GameObject hitEffect;

	[Space]
	[SerializeField]
	private float openGateDelay = 1f;

	[Space]
	[SerializeField]
	private Collider2D activeCollider;

	[SerializeField]
	private GameObject effects;

	[SerializeField]
	private GameObject activatedTinker;

	[Space]
	[SerializeField]
	private AudioClip hitSound;

	[SerializeField]
	private VibrationDataAsset hitVibration;

	[SerializeField]
	private bool hitSoundOnActivate;

	[Space]
	[SerializeField]
	private bool checkHeroRange;

	[SerializeField]
	private float xMin;

	[SerializeField]
	private float xMax = 9999f;

	[SerializeField]
	private float yMin;

	[SerializeField]
	private float yMax = 9999f;

	[SerializeField]
	private TrackTriggerObjects canHitTrigger;

	[Space]
	public UnityEvent BeforeOpenDelay;

	public UnityEvent CustomGateOpen;

	public UnityEvent CustomGateActivated;

	private AudioSource source;

	private tk2dSpriteAnimator animator;

	private VibrationPlayer sourceVibrationPlayer;

	private bool activated;

	private void Awake()
	{
		animator = GetComponent<tk2dSpriteAnimator>();
		source = GetComponent<AudioSource>();
		if ((bool)source)
		{
			sourceVibrationPlayer = source.GetComponent<VibrationPlayer>();
		}
		if ((bool)activatedTinker)
		{
			activatedTinker.SetActive(value: false);
		}
	}

	private void Start()
	{
		PersistentBoolItem component = GetComponent<PersistentBoolItem>();
		if ((bool)component)
		{
			component.OnGetSaveState += delegate(out bool value)
			{
				value = activated;
			};
			component.OnSetSaveState += delegate(bool value)
			{
				activated = value;
				if (activated)
				{
					animator.Play("Activated");
					UnlockablePropBase[] array = connectedGates;
					foreach (UnlockablePropBase unlockablePropBase in array)
					{
						if ((bool)unlockablePropBase)
						{
							unlockablePropBase.Opened();
						}
					}
					PlayMakerFSM[] array2 = fsmGates;
					foreach (PlayMakerFSM playMakerFSM in array2)
					{
						if ((bool)playMakerFSM)
						{
							try
							{
								playMakerFSM.SendEvent(fsmActivateEvent);
							}
							catch (Exception exception)
							{
								Debug.LogException(exception);
							}
						}
					}
					GameObject[] array3 = camLocks;
					foreach (GameObject gameObject in array3)
					{
						if ((bool)gameObject)
						{
							gameObject.SetActive(value: false);
						}
					}
					CustomGateActivated.Invoke();
					if ((bool)activatedTinker)
					{
						activatedTinker.SetActive(value: true);
					}
				}
			};
		}
		if ((bool)activeCollider)
		{
			activeCollider.enabled = true;
		}
	}

	private bool? IsFsmEventValidRequiredFSM(PlayMakerFSM fsm)
	{
		if (fsm != null)
		{
			return fsm.IsEventValid(fsmActivateEvent, isRequired: true);
		}
		return null;
	}

	private bool? IsFsmEventValidRequired(string eventName)
	{
		if (fsmGates != null)
		{
			bool value = false;
			PlayMakerFSM[] array = fsmGates;
			foreach (PlayMakerFSM playMakerFSM in array)
			{
				if (!(playMakerFSM == null))
				{
					if (playMakerFSM.IsEventValid(eventName, isRequired: true) == false)
					{
						return false;
					}
					value = true;
				}
			}
			return value;
		}
		return null;
	}

	public IHitResponder.HitResponse Hit(HitInstance damageInstance)
	{
		bool flag = false;
		if (!damageInstance.IsNailDamage)
		{
			return IHitResponder.Response.None;
		}
		if ((bool)canHitTrigger && !canHitTrigger.IsInside)
		{
			return IHitResponder.Response.None;
		}
		if (!activated)
		{
			activated = true;
			flag = true;
			if (!string.IsNullOrEmpty(setPlayerDataBool))
			{
				GameManager.instance.playerData.SetBool(setPlayerDataBool, value: true);
			}
			if ((bool)activeCollider)
			{
				activeCollider.enabled = false;
			}
			StartCoroutine(Execute());
			if (hitSoundOnActivate)
			{
				PlaySoundOneShot(hitSound);
				if (hitVibration != null)
				{
					VibrationManager.PlayVibrationClipOneShot(hitVibration, null);
				}
			}
		}
		if (flag)
		{
			if ((bool)hitEffect)
			{
				hitEffect.Spawn(activeCollider.bounds.center);
			}
			if ((bool)effects)
			{
				effects.SetActive(value: true);
			}
			PlaySound();
			GameCameras instance = GameCameras.instance;
			if ((bool)instance)
			{
				instance.cameraShakeFSM.SendEvent("EnemyKillShake");
			}
			GameManager.instance.FreezeMoment(1);
		}
		return flag ? IHitResponder.Response.GenericHit : IHitResponder.Response.None;
	}

	private IEnumerator Execute()
	{
		animator.Play("Hit");
		BeforeOpenDelay.Invoke();
		yield return new WaitForSeconds(openGateDelay);
		if ((bool)activatedTinker)
		{
			activatedTinker.SetActive(value: true);
		}
		UnlockablePropBase[] array = connectedGates;
		foreach (UnlockablePropBase unlockablePropBase in array)
		{
			if ((bool)unlockablePropBase)
			{
				unlockablePropBase.Open();
			}
		}
		PlayMakerFSM[] array2 = fsmGates;
		foreach (PlayMakerFSM playMakerFSM in array2)
		{
			if ((bool)playMakerFSM)
			{
				try
				{
					playMakerFSM.SendEvent("OPEN");
				}
				catch (Exception exception)
				{
					Debug.LogException(exception);
				}
			}
		}
		GameObject[] array3 = camLocks;
		foreach (GameObject gameObject in array3)
		{
			if ((bool)gameObject)
			{
				gameObject.SetActive(value: false);
			}
		}
		if (!string.IsNullOrEmpty(sendEventToRegister))
		{
			EventRegister.SendEvent(sendEventToRegister);
		}
		CustomGateOpen.Invoke();
	}

	public void PlaySound()
	{
		if ((bool)source)
		{
			source.Play();
			if ((bool)sourceVibrationPlayer)
			{
				sourceVibrationPlayer.Play();
			}
		}
	}

	public void PlaySoundOneShot(AudioClip clip)
	{
		if ((bool)source && (bool)clip)
		{
			source.PlayOneShot(clip);
		}
	}
}

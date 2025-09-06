using System.Collections;
using UnityEngine;

public abstract class PressurePlateBase : EventBase
{
	[SerializeField]
	private TriggerEnterEvent dropOnTrigger;

	[SerializeField]
	private SpriteRenderer plateGraphic;

	[SerializeField]
	private Material plateUpMaterial;

	[SerializeField]
	private Material plateDownMaterial;

	[Space]
	[SerializeField]
	private float touchOffset = 0.1f;

	[SerializeField]
	private GameObject touchParticles;

	[SerializeField]
	private GameObject endParticles;

	[SerializeField]
	private AudioClip landSound;

	[SerializeField]
	private VibrationDataAsset landVibration;

	[Space]
	[SerializeField]
	private float waitTime = 1f;

	[SerializeField]
	private float dropDistance = 1f;

	[SerializeField]
	private float dropTime = 0.1f;

	[SerializeField]
	private AudioClip dropSound;

	[SerializeField]
	private VibrationDataAsset dropVibration;

	[SerializeField]
	private AudioClip raiseSound;

	[SerializeField]
	private VibrationDataAsset raiseVibration;

	[SerializeField]
	private CameraShakeTarget dropCameraShake;

	[SerializeField]
	private float gateOpenDelay = 1f;

	[SerializeField]
	private string sendEventToRegister;

	private Collider2D col;

	private AudioSource source;

	private GameObject player;

	private bool isTouched;

	private float forceTouchedTime;

	private Vector3 initialPos;

	private Coroutine moveRoutine;

	protected abstract bool CanDepress { get; }

	public float GateOpenDelay => gateOpenDelay;

	public override string InspectorInfo => $"Pressure Plate ({base.gameObject.name})";

	protected override void Awake()
	{
		base.Awake();
		col = GetComponent<Collider2D>();
		source = GetComponent<AudioSource>();
		initialPos = (plateGraphic ? plateGraphic.transform.localPosition : Vector3.zero);
		if ((bool)plateUpMaterial && (bool)plateGraphic)
		{
			plateGraphic.sharedMaterial = plateUpMaterial;
		}
		if ((bool)touchParticles)
		{
			touchParticles.SetActive(value: false);
		}
		if ((bool)endParticles)
		{
			endParticles.SetActive(value: false);
		}
		if ((bool)dropOnTrigger)
		{
			dropOnTrigger.OnTriggerEntered += delegate(Collider2D other, GameObject sender)
			{
				OnTouchStart(other.gameObject);
			};
			dropOnTrigger.OnTriggerExited += delegate(Collider2D other, GameObject sender)
			{
				OnTouchEnd(other.gameObject);
			};
		}
	}

	private void OnCollisionEnter2D(Collision2D collision)
	{
		if ((bool)dropOnTrigger || !CanDepress || !collision.collider.CompareTag("Player"))
		{
			return;
		}
		if (Time.fixedTime <= forceTouchedTime)
		{
			HeroController.instance.BounceShort();
			return;
		}
		Collision2DUtils.Collision2DSafeContact safeContact = collision.GetSafeContact();
		Vector2 point = safeContact.Point;
		Bounds bounds = col.bounds;
		Vector3 min = bounds.min;
		Vector3 max = bounds.max;
		if (point.y < max.y || point.x < min.x || point.x > max.x)
		{
			bool flag = false;
			if (safeContact.IsLegitimate)
			{
				flag = safeContact.Normal == Vector2.up;
				if (!flag)
				{
					Bounds bounds2 = collision.collider.bounds;
					Vector3 max2 = bounds2.max;
					Vector3 min2 = bounds2.min;
					flag = min2.y >= max.y && ((max2.x > min.x && max2.x < max.x) || (min2.x > min.x && min2.x < max.x));
				}
			}
			if (!flag)
			{
				return;
			}
		}
		if (!safeContact.IsLegitimate)
		{
			Debug.LogWarning("Pressure Plate contact point was not legitimate! (dang it, Unity D:)", this);
		}
		OnTouchStart(collision.gameObject);
	}

	private void OnCollisionExit2D(Collision2D collision)
	{
		if (!dropOnTrigger)
		{
			OnTouchEnd(collision.gameObject);
		}
	}

	private void OnTouchStart(GameObject touchingPlayer)
	{
		player = touchingPlayer;
		ResetTouched();
		isTouched = true;
		if ((bool)plateGraphic)
		{
			plateGraphic.transform.localPosition += new Vector3(0f, 0f - touchOffset, 0f);
		}
		if ((bool)touchParticles)
		{
			touchParticles.SetActive(value: true);
		}
		PlaySound(landSound);
		PlayVibration(landVibration);
		StartDrop(force: false);
	}

	private void OnTouchEnd(GameObject touchingPlayer)
	{
		if (player == touchingPlayer)
		{
			player = null;
		}
	}

	protected void StartDrop(bool force)
	{
		if (moveRoutine != null)
		{
			StopCoroutine(moveRoutine);
		}
		moveRoutine = StartCoroutine(Drop(force));
	}

	protected void StartRaise(float raiseDelay)
	{
		if (moveRoutine != null)
		{
			StopCoroutine(moveRoutine);
		}
		moveRoutine = StartCoroutine(Raise(raiseDelay, silent: false));
	}

	protected void StartRaiseSilent(float raiseDelay)
	{
		if (moveRoutine != null)
		{
			StopCoroutine(moveRoutine);
		}
		moveRoutine = StartCoroutine(Raise(raiseDelay, silent: true));
	}

	protected void SetDepressed()
	{
		if ((bool)plateGraphic)
		{
			plateGraphic.transform.localPosition = initialPos + new Vector3(0f, 0f - dropDistance, 0f);
			if ((bool)plateDownMaterial)
			{
				plateGraphic.sharedMaterial = plateDownMaterial;
			}
		}
		col.enabled = false;
	}

	private void ResetTouched()
	{
		if (isTouched)
		{
			isTouched = false;
			if ((bool)plateGraphic)
			{
				plateGraphic.transform.localPosition -= Vector3.down * touchOffset;
			}
			if ((bool)touchParticles)
			{
				touchParticles.SetActive(value: false);
			}
			if (source.clip == landSound)
			{
				source.Stop();
				source.clip = null;
			}
		}
	}

	private IEnumerator Drop(bool force)
	{
		if (!force)
		{
			yield return new WaitForSeconds(waitTime);
			if (!player)
			{
				ResetTouched();
				yield break;
			}
			PlaySound(dropSound);
			PlayVibration(dropVibration);
		}
		col.enabled = false;
		if ((bool)plateGraphic)
		{
			Vector3 targetPos = initialPos + new Vector3(0f, 0f - dropDistance, 0f);
			for (float elapsed = 0f; elapsed <= dropTime; elapsed += Time.deltaTime)
			{
				plateGraphic.transform.localPosition = Vector3.Lerp(initialPos, targetPos, elapsed / dropTime);
				yield return null;
			}
			plateGraphic.transform.localPosition = targetPos;
			if ((bool)plateDownMaterial)
			{
				plateGraphic.sharedMaterial = plateDownMaterial;
			}
		}
		if (!force)
		{
			dropCameraShake.DoShake(this);
			if ((bool)endParticles)
			{
				endParticles.SetActive(value: true);
			}
		}
		PreActivate();
		yield return new WaitForSeconds(gateOpenDelay);
		Activate();
		CallReceivedEvent();
		if (!string.IsNullOrWhiteSpace(sendEventToRegister))
		{
			EventRegister.SendEvent(sendEventToRegister);
		}
	}

	private IEnumerator Raise(float raiseDelay, bool silent)
	{
		if (raiseDelay > 0f)
		{
			yield return new WaitForSeconds(raiseDelay);
		}
		forceTouchedTime = Time.fixedTime + 0.1f;
		col.enabled = true;
		if (!silent)
		{
			PlaySound(raiseSound);
			PlayVibration(raiseVibration);
		}
		if ((bool)plateGraphic)
		{
			Vector3 targetPos = initialPos;
			Vector3 startPos = plateGraphic.transform.localPosition;
			if ((bool)plateUpMaterial)
			{
				plateGraphic.sharedMaterial = plateUpMaterial;
			}
			for (float elapsed = 0f; elapsed <= dropTime; elapsed += Time.deltaTime)
			{
				plateGraphic.transform.localPosition = Vector3.Lerp(startPos, targetPos, elapsed / dropTime);
				yield return null;
			}
			plateGraphic.transform.localPosition = targetPos;
		}
		isTouched = false;
		Raised();
	}

	protected void PlaySound(AudioClip clip)
	{
		if ((bool)source)
		{
			source.Stop();
			source.clip = clip;
			source.Play();
		}
	}

	protected void PlayVibration(VibrationDataAsset vibrationDataAsset)
	{
		if ((bool)vibrationDataAsset)
		{
			VibrationManager.PlayVibrationClipOneShot(vibrationDataAsset.VibrationData, null);
		}
	}

	protected virtual void PreActivate()
	{
	}

	protected abstract void Activate();

	protected virtual void Raised()
	{
	}
}

using System.Collections;
using UnityEngine;
using UnityEngine.Events;

public class TrapPressurePlate : MonoBehaviour
{
	[SerializeField]
	private TrackTriggerObjects tracker;

	[SerializeField]
	private Transform plate;

	[SerializeField]
	private float dropOffset;

	[SerializeField]
	private float dropTime;

	[SerializeField]
	private float raiseDelay;

	[SerializeField]
	private float raiseTime;

	[SerializeField]
	private CameraShakeTarget dropShake;

	[SerializeField]
	private AudioEvent dropAudio;

	[SerializeField]
	private AudioEvent riseAudio;

	[Space]
	public UnityEvent OnPressed;

	private Coroutine dropRoutine;

	private bool canReturn;

	private bool isBlocked;

	private void Awake()
	{
		if ((bool)tracker)
		{
			tracker.InsideStateChanged += OnInsideStateChanged;
		}
	}

	private void OnInsideStateChanged(bool isInside)
	{
		if (!plate)
		{
			return;
		}
		if (isInside)
		{
			canReturn = false;
			if (dropRoutine == null)
			{
				dropRoutine = StartCoroutine(Drop());
			}
		}
		else
		{
			canReturn = true;
		}
	}

	private IEnumerator Drop()
	{
		float platePos = plate.transform.localPosition.y;
		float bottomPlatePos = platePos + dropOffset;
		for (float elapsed = 0f; elapsed < dropTime; elapsed += Time.deltaTime)
		{
			plate.SetLocalPositionY(Mathf.Lerp(platePos, bottomPlatePos, elapsed / dropTime));
			yield return null;
		}
		plate.SetLocalPositionY(bottomPlatePos);
		dropShake.DoShake(this);
		dropAudio.SpawnAndPlayOneShot(base.transform.position);
		OnPressed.Invoke();
		for (float elapsed = 0f; elapsed < raiseDelay; elapsed = ((!canReturn || isBlocked) ? 0f : (elapsed + Time.deltaTime)))
		{
			yield return null;
		}
		riseAudio.SpawnAndPlayOneShot(base.transform.position);
		for (float elapsed = 0f; elapsed < raiseTime; elapsed += Time.deltaTime)
		{
			plate.SetLocalPositionY(Mathf.Lerp(bottomPlatePos, platePos, elapsed / raiseTime));
			yield return null;
		}
		plate.SetLocalPositionY(platePos);
		dropRoutine = null;
	}

	public void SetBlocked(bool value)
	{
		isBlocked = value;
	}
}

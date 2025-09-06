using System.Collections;
using UnityEngine;

public class CameraShakeOnEnable : MonoBehaviour
{
	public CameraShakeTarget CameraShake;

	public float delay;

	private Coroutine scheduleRoutine;

	private void OnEnable()
	{
		scheduleRoutine = StartCoroutine(ShakeCameraDelayed());
	}

	private void OnDisable()
	{
		if (scheduleRoutine != null)
		{
			StopCoroutine(scheduleRoutine);
			scheduleRoutine = null;
		}
	}

	private IEnumerator ShakeCameraDelayed()
	{
		yield return new WaitForEndOfFrame();
		if (delay > 0f)
		{
			yield return new WaitForSeconds(delay);
		}
		CameraShake.DoShake(this);
	}
}

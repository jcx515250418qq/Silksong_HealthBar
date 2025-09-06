using System.Collections;
using UnityEngine;

public class CharmVibrations : MonoBehaviour
{
	[SerializeField]
	private VibrationData regularPlace;

	[SerializeField]
	private VibrationData failedPlace;

	[SerializeField]
	private VibrationData overcharmPlace;

	[SerializeField]
	private VibrationData overcharmHit;

	[SerializeField]
	private VibrationData overcharmFinalHit;

	public void PlayRegularPlace()
	{
		PlayDelayedVibration(regularPlace);
	}

	public void PlayFailedPlace()
	{
		PlayDelayedVibration(failedPlace);
	}

	public void PlayOvercharmPlace()
	{
		PlayDelayedVibration(overcharmPlace);
	}

	public void PlayOvercharmHit()
	{
		PlayDelayedVibration(overcharmHit);
	}

	public void PlayOvercharmFinalHit()
	{
		PlayDelayedVibration(overcharmFinalHit);
	}

	protected void PlayDelayedVibration(VibrationData vibrationData)
	{
		StartCoroutine(PlayDelayedVibrationRoutine(vibrationData));
	}

	protected IEnumerator PlayDelayedVibrationRoutine(VibrationData vibrationData)
	{
		yield return null;
		VibrationManager.PlayVibrationClipOneShot(vibrationData, new VibrationTarget(VibrationMotors.All));
	}
}

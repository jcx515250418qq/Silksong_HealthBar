using UnityEngine;

public class VibrationEffect : MonoBehaviour
{
	[SerializeField]
	private VibrationData vibrationData;

	[SerializeField]
	private VibrationTarget vibrationSource;

	protected void OnEnable()
	{
		VibrationManager.PlayVibrationClipOneShot(vibrationData, vibrationSource);
	}
}

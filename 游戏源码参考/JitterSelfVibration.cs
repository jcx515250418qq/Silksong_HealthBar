using UnityEngine;

public sealed class JitterSelfVibration : MonoBehaviour
{
	[SerializeField]
	private JitterSelf jitterSelf;

	[Space]
	[SerializeField]
	private VibrationDataAsset vibrationDataAsset;

	[SerializeField]
	private bool isLooping;

	[SerializeField]
	private bool isRealTime;

	private VibrationEmission emission;

	private void Awake()
	{
		if ((bool)jitterSelf)
		{
			jitterSelf.OnJitterStart.AddListener(StartEmission);
			jitterSelf.OnJitterEnd.AddListener(StopEmission);
		}
	}

	private void OnValidate()
	{
		if (!jitterSelf)
		{
			jitterSelf = GetComponent<JitterSelf>();
		}
	}

	private void OnDisable()
	{
		StopEmission();
	}

	public void StartEmission()
	{
		StopEmission();
		VibrationData vibrationData = vibrationDataAsset;
		bool flag = isLooping;
		bool isRealtime = isRealTime;
		emission = VibrationManager.PlayVibrationClipOneShot(vibrationData, null, flag, "", isRealtime);
	}

	public void StopEmission()
	{
		emission?.Stop();
		emission = null;
	}
}

using UnityEngine;

public sealed class HeroVibrationRegionSyncer : MonoBehaviour
{
	[SerializeField]
	private HeroVibrationRegion heroVibrationRegion;

	[SerializeField]
	private AudioSource audioSource;

	private void Start()
	{
		if ((bool)heroVibrationRegion)
		{
			heroVibrationRegion.MainEmissionStarted += HeroVibrationRegionOnMainEmissionStarted;
		}
		else
		{
			Debug.LogError($"{this} is missing vibration region.");
		}
		if (audioSource == null)
		{
			Debug.LogError($"{this} is missing audio source. Emission Sync will fail.");
		}
	}

	private void OnValidate()
	{
		if (!heroVibrationRegion)
		{
			heroVibrationRegion = GetComponent<HeroVibrationRegion>();
		}
	}

	private void HeroVibrationRegionOnMainEmissionStarted(VibrationEmission emission)
	{
		AudioVibrationSyncer.StartSyncedEmission(emission, audioSource);
	}
}

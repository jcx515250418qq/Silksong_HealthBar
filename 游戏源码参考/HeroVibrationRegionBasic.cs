using UnityEngine;

public sealed class HeroVibrationRegionBasic : MonoBehaviour
{
	[SerializeField]
	private VibrationDataAsset vibrationDataAsset;

	[SerializeField]
	private float strength = 1f;

	[SerializeField]
	private float speed = 1f;

	[SerializeField]
	private bool loop = true;

	[SerializeField]
	private bool isRealTime;

	[SerializeField]
	private string vibrationTag;

	private bool isInside;

	private VibrationEmission emission;

	private void OnTriggerEnter2D(Collider2D other)
	{
		if (other.CompareTag("Player"))
		{
			Enter();
		}
	}

	private void OnTriggerExit2D(Collider2D other)
	{
		if (other.CompareTag("Player"))
		{
			Exit();
		}
	}

	private void Enter()
	{
		if (!isInside)
		{
			isInside = true;
			VibrationData vibrationData = vibrationDataAsset;
			bool isLooping = loop;
			bool isRealtime = isRealTime;
			string text = vibrationTag;
			emission = VibrationManager.PlayVibrationClipOneShot(vibrationData, null, isLooping, text, isRealtime);
			emission?.SetStrength(strength);
			emission?.SetSpeed(speed);
		}
	}

	private void Exit()
	{
		if (isInside)
		{
			isInside = false;
			emission?.Stop();
			emission = null;
		}
	}
}

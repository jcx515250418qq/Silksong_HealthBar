using UnityEngine;

public class CameraShakeWhileEnabled : MonoBehaviour
{
	[SerializeField]
	private CameraShakeTarget runWhileEnabled;

	[SerializeField]
	private CameraShakeTarget runWhenDisabled;

	[SerializeField]
	private bool doRepeat;

	[SerializeField]
	private float interval = 0.15f;

	private float timer;

	private void OnEnable()
	{
		runWhileEnabled.Cache();
		runWhileEnabled.DoShake(this);
		if (doRepeat)
		{
			timer = interval;
		}
	}

	private void OnDisable()
	{
		runWhileEnabled.CancelShake();
		runWhenDisabled.DoShake(this);
	}

	private void Update()
	{
		if (doRepeat)
		{
			timer -= Time.deltaTime;
			if (timer <= 0f)
			{
				timer += interval;
				runWhenDisabled.DoShake(this);
			}
		}
	}
}

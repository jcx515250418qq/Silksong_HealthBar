using UnityEngine;

public class DriftflyCloud : MonoBehaviour
{
	public ParticleSystem ptIdle;

	public ParticleSystem ptDisperse;

	public TrackTriggerObjects disperseRange;

	private bool isPlaying;

	private bool dispersed;

	private float timer;

	private Transform gameCameraTransform;

	private const float CAM_DISTANCE_MAX = 40f;

	private const float DISPERSE_TIME = 5f;

	private void Start()
	{
		gameCameraTransform = GameCameras.instance.mainCamera.gameObject.transform;
	}

	private void Update()
	{
		float num = Vector3.Distance(base.transform.position, gameCameraTransform.position);
		if (num > 40f && isPlaying)
		{
			ptIdle.Stop();
			isPlaying = false;
			dispersed = false;
		}
		else if (!isPlaying && !dispersed && num < 40f)
		{
			ptIdle.Play();
			isPlaying = true;
		}
		else if (disperseRange.IsInside && !dispersed)
		{
			dispersed = true;
			ptIdle.Stop();
			ptDisperse.Play();
			timer = 5f;
		}
		else if (disperseRange.IsInside)
		{
			timer = 5f;
		}
		else if (timer <= 0f && dispersed)
		{
			ptIdle.Play();
			dispersed = false;
		}
		else
		{
			timer -= Time.deltaTime;
		}
	}
}

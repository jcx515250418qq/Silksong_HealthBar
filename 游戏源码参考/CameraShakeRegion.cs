using System.Collections;
using UnityEngine;

public class CameraShakeRegion : MonoBehaviour, ICameraShake
{
	[SerializeField]
	private TrackTriggerObjects trigger;

	[Space]
	[SerializeField]
	private CameraManagerReference cameraRef;

	[SerializeField]
	private CameraShakeProfile sourceProfile;

	[SerializeField]
	private float fadeInDuration;

	[SerializeField]
	private float fadeOutDuration;

	private Coroutine insideRoutine;

	private bool isInsideTrigger;

	private float currentMultiplier;

	public bool CanFinish => sourceProfile.CanFinish;

	public int FreezeFrames => 0;

	public CameraShakeWorldForceIntensities WorldForceOnStart => CameraShakeWorldForceIntensities.None;

	public ICameraShakeVibration CameraShakeVibration => null;

	public float Magnitude => sourceProfile.Magnitude;

	private void Awake()
	{
		trigger.InsideStateChanged += OnInsideStateChanged;
	}

	private void OnInsideStateChanged(bool isInside)
	{
		isInsideTrigger = isInside;
		if (isInside && insideRoutine == null)
		{
			StartCoroutine(InsideRoutine());
		}
	}

	private IEnumerator InsideRoutine()
	{
		cameraRef.DoShake(this, this, doFreeze: false);
		while (true)
		{
			float elapsed = 0f;
			float startMultiplier = currentMultiplier;
			for (; elapsed < fadeInDuration; elapsed += Time.deltaTime)
			{
				currentMultiplier = Mathf.Lerp(startMultiplier, 1f, elapsed / fadeInDuration);
				if (!isInsideTrigger)
				{
					break;
				}
				yield return null;
			}
			currentMultiplier = 1f;
			while (isInsideTrigger)
			{
				yield return null;
			}
			startMultiplier = 0f;
			elapsed = currentMultiplier;
			while (true)
			{
				if (startMultiplier < fadeOutDuration)
				{
					currentMultiplier = Mathf.Lerp(elapsed, 0f, startMultiplier / fadeOutDuration);
					if (isInsideTrigger)
					{
						break;
					}
					yield return null;
					startMultiplier += Time.deltaTime;
					continue;
				}
				currentMultiplier = 0f;
				cameraRef.CancelShake(this);
				insideRoutine = null;
				yield break;
			}
		}
	}

	public Vector2 GetOffset(float elapsedTime)
	{
		return sourceProfile.GetOffset(elapsedTime) * currentMultiplier;
	}

	public bool IsDone(float elapsedTime)
	{
		return sourceProfile.IsDone(elapsedTime);
	}
}

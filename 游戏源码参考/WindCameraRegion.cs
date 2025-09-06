using System.Collections;
using UnityEngine;

public class WindCameraRegion : TrackTriggerObjects, ICameraShake
{
	[SerializeField]
	private CameraManagerReference cameraManager;

	[SerializeField]
	private CameraShakeProfile sourceSway;

	[Space]
	[SerializeField]
	private float fadeTime;

	private static int _insideRegions;

	private bool wasInside;

	private float insideMagnitude;

	private Coroutine insideMagnitudeFadeRoutine;

	public bool CanFinish => false;

	public int FreezeFrames => 0;

	public CameraShakeWorldForceIntensities WorldForceOnStart => CameraShakeWorldForceIntensities.None;

	public float Magnitude
	{
		get
		{
			if (!sourceSway)
			{
				return 0f;
			}
			return sourceSway.Magnitude;
		}
	}

	public ICameraShakeVibration CameraShakeVibration => null;

	public Vector2 GetOffset(float elapsedTime)
	{
		if (!sourceSway)
		{
			return Vector2.zero;
		}
		return sourceSway.GetOffset(elapsedTime) * insideMagnitude;
	}

	public bool IsDone(float elapsedTime)
	{
		if (!this)
		{
			return true;
		}
		if (insideMagnitudeFadeRoutine == null)
		{
			return insideMagnitude <= 0.001f;
		}
		return false;
	}

	protected override void OnInsideStateChanged(bool isInside)
	{
		if (wasInside != isInside)
		{
			wasInside = isInside;
			if (isInside)
			{
				_insideRegions++;
			}
			else
			{
				_insideRegions--;
			}
			if (_insideRegions == 1)
			{
				SetWindy(value: true);
			}
			else if (_insideRegions == 0)
			{
				SetWindy(value: false);
			}
		}
	}

	private void SetWindy(bool value)
	{
		cameraManager.CancelShake(this);
		if (insideMagnitudeFadeRoutine != null)
		{
			StopCoroutine(insideMagnitudeFadeRoutine);
		}
		if (base.isActiveAndEnabled)
		{
			insideMagnitudeFadeRoutine = StartCoroutine(FadeRoutine(value ? 1f : 0f));
			cameraManager.DoShake(this, this);
		}
	}

	private IEnumerator FadeRoutine(float newMagnitude)
	{
		float initialMagnitude = insideMagnitude;
		for (float elapsed = 0f; elapsed < fadeTime; elapsed += Time.deltaTime)
		{
			insideMagnitude = Mathf.Lerp(initialMagnitude, newMagnitude, elapsed / fadeTime);
			yield return null;
		}
		insideMagnitude = newMagnitude;
		insideMagnitudeFadeRoutine = null;
	}
}

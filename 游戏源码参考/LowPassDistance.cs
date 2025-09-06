using System.Collections.Generic;
using TeamCherry.SharedUtils;
using UnityEngine;

public class LowPassDistance : MonoBehaviour
{
	[SerializeField]
	private AudioSource audioSource;

	[SerializeField]
	private AudioLowPassFilter lowPassFilter;

	[SerializeField]
	private MinMaxFloat mapToRange = new MinMaxFloat(0f, 1f);

	private Transform camera;

	private Transform audioTrans;

	private readonly Dictionary<AudioSourceCurveType, AnimationCurve> curveCache = new Dictionary<AudioSourceCurveType, AnimationCurve>();

	private void Reset()
	{
		audioSource = GetComponent<AudioSource>();
	}

	private void OnEnable()
	{
		if (audioSource == null)
		{
			base.enabled = false;
		}
		if (lowPassFilter == null)
		{
			base.enabled = false;
		}
	}

	private void Start()
	{
		camera = GameCameras.instance.mainCamera.transform;
		audioTrans = audioSource.transform;
	}

	private void LateUpdate()
	{
		if (!camera)
		{
			if (!GameCameras.instance)
			{
				return;
			}
			camera = GameCameras.instance.mainCamera.transform;
			if (!camera)
			{
				return;
			}
		}
		Vector3 position = camera.position;
		Vector3 position2 = audioTrans.position;
		float distance = Vector3.Distance(position, position2);
		float tOnCurve = GetTOnCurve(AudioSourceCurveType.CustomRolloff, distance);
		float lerpedValue = mapToRange.GetLerpedValue(tOnCurve);
		lowPassFilter.cutoffFrequency = lerpedValue;
	}

	private float GetTOnCurve(AudioSourceCurveType curveType, float distance)
	{
		if (!curveCache.TryGetValue(curveType, out var value))
		{
			value = audioSource.GetCustomCurve(curveType);
			curveCache[curveType] = value;
		}
		return value.Evaluate(distance / audioSource.maxDistance);
	}
}

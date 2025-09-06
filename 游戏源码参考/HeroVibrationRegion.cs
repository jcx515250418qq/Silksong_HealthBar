using System;
using System.Collections.Generic;
using TeamCherry.SharedUtils;
using UnityEngine;

public sealed class HeroVibrationRegion : MonoBehaviour
{
	public delegate void EmissionEvent(VibrationEmission emission);

	[Serializable]
	private enum ScaleType
	{
		None = 0,
		Radial = 1,
		Rectangle = 2
	}

	[Serializable]
	[Flags]
	private enum PlayStop
	{
		Manual = 0,
		PlayOnEnter = 1,
		StopOnExit = 2
	}

	[Serializable]
	[Flags]
	public enum VibrationSettings
	{
		None = 0,
		Loop = 1,
		RealTime = 2,
		StopOnExit = 4,
		ControlledByRegion = 8
	}

	private struct EmissionTracker
	{
		public VibrationEmission emission;

		public VibrationSettings settings;

		public readonly float strength;

		public bool StopOnExit => settings.HasFlag(VibrationSettings.StopOnExit);

		public bool ControlledByRegion => settings.HasFlag(VibrationSettings.ControlledByRegion);

		public EmissionTracker(VibrationEmission emission, VibrationSettings settings, float strength)
		{
			this.emission = emission;
			this.settings = settings;
			this.strength = strength;
		}

		public void SetStrength(float strength)
		{
			emission?.SetStrength(this.strength * strength);
		}
	}

	private abstract class ScaleSettings
	{
		public abstract float GetIntensity(Vector3 position, Vector3 centerPoint);
	}

	[Serializable]
	private class RadialScaleSettings : ScaleSettings
	{
		public float minIntensityRadius;

		public float maxIntensityRadius;

		public RadialScaleSettings(float minIntensityRadius, float maxIntensityRadius)
		{
			this.minIntensityRadius = minIntensityRadius;
			this.maxIntensityRadius = maxIntensityRadius;
		}

		public override float GetIntensity(Vector3 position, Vector3 centerPoint)
		{
			float num = Vector3.Distance(position, centerPoint);
			if (num <= maxIntensityRadius)
			{
				return 1f;
			}
			if (num >= minIntensityRadius)
			{
				return 0f;
			}
			return Mathf.Lerp(1f, 0f, (num - maxIntensityRadius) / (minIntensityRadius - maxIntensityRadius));
		}
	}

	[Serializable]
	private class RectangleScaleSettings : ScaleSettings
	{
		public float minIntensityWidth;

		public float maxIntensityWidth;

		public float minIntensityHeight;

		public float maxIntensityHeight;

		public RectangleScaleSettings(float min, float max)
		{
			minIntensityWidth = min;
			maxIntensityWidth = max;
			minIntensityHeight = min;
			maxIntensityHeight = max;
		}

		public RectangleScaleSettings(float minIntensityWidth, float maxIntensityWidth, float minIntensityHeight, float maxIntensityHeight)
		{
			this.minIntensityWidth = minIntensityWidth;
			this.maxIntensityWidth = maxIntensityWidth;
			this.minIntensityHeight = minIntensityHeight;
			this.maxIntensityHeight = maxIntensityHeight;
		}

		public override float GetIntensity(Vector3 position, Vector3 centerPoint)
		{
			float num = Mathf.Abs(position.x - centerPoint.x);
			float num2 = Mathf.Abs(position.y - centerPoint.y);
			float a = ((num <= maxIntensityWidth / 2f) ? 1f : Mathf.InverseLerp(minIntensityWidth / 2f, maxIntensityWidth / 2f, num));
			float b = ((num2 <= maxIntensityHeight / 2f) ? 1f : Mathf.InverseLerp(minIntensityHeight / 2f, maxIntensityHeight / 2f, num2));
			return Mathf.Min(a, b);
		}
	}

	[Header("Detection Settings")]
	[SerializeField]
	private TriggerEnterEvent heroDetector;

	[Header("Vibration Configuration")]
	[SerializeField]
	private VibrationDataAsset vibrationDataAsset;

	[SerializeField]
	private float vibrationDataStrength = 1f;

	[SerializeField]
	private PlayStop playStopMode;

	[SerializeField]
	private bool loop;

	[SerializeField]
	private bool isRealTime;

	[Header("Scaling and Strength Settings")]
	[SerializeField]
	private ScaleType distanceScaleMode;

	[SerializeField]
	private Transform vibrationCentre;

	[SerializeField]
	private MinMaxFloat strengthRange = new MinMaxFloat(0f, 1f);

	[SerializeField]
	private AnimationCurve strengthCurve = AnimationCurve.EaseInOut(0f, 0f, 1f, 1f);

	[ModifiableProperty]
	[Conditional("ShowRadial", true, true, true)]
	[SerializeField]
	private RadialScaleSettings radialScaleSettings;

	[ModifiableProperty]
	[Conditional("ShowRectangle", true, true, true)]
	[SerializeField]
	private RectangleScaleSettings rectangleScaleSettings;

	[Header("Additional Settings")]
	[Tooltip("Requires hero inside region to start vibration")]
	[SerializeField]
	private bool requireInside;

	[SerializeField]
	private bool onlyFeltInsideRegion;

	[Header("Debug")]
	[SerializeField]
	private bool alwaysDrawGizmos;

	private bool isInside;

	private VibrationEmission mainEmissionLoop;

	private float strength = 1f;

	private bool loopRequested;

	private List<EmissionTracker> emissions = new List<EmissionTracker>();

	private bool doStrengthUpdate;

	private Transform heroTransform;

	private ScaleSettings scale;

	public event EmissionEvent MainEmissionStarted;

	private void Awake()
	{
		if ((bool)heroDetector)
		{
			heroDetector.OnTriggerEntered += OnTriggerEntered;
			heroDetector.OnTriggerExited += OnTriggerExited;
			if (vibrationCentre == null)
			{
				vibrationCentre = heroDetector.transform;
			}
		}
		if (vibrationCentre == null)
		{
			vibrationCentre = base.transform;
		}
		if (distanceScaleMode == ScaleType.None)
		{
			strength = strengthRange.GetLerpedValue(1f);
		}
		else
		{
			strength = strengthRange.GetLerpedValue(0f);
		}
	}

	private void OnDisable()
	{
		foreach (EmissionTracker emission in emissions)
		{
			emission.emission.Stop();
		}
		emissions.Clear();
	}

	private void Update()
	{
		bool flag = StrengthUpdate();
		if (EmissionsUpdate())
		{
			flag = true;
		}
		if (!flag)
		{
			base.enabled = false;
		}
	}

	private bool ShowRadial()
	{
		return distanceScaleMode == ScaleType.Radial;
	}

	private bool ShowRectangle()
	{
		return distanceScaleMode == ScaleType.Rectangle;
	}

	private void OnTriggerEntered(Collider2D collider, GameObject sender)
	{
		if (IsCorrectTarget(collider) && !isInside)
		{
			isInside = true;
			if (distanceScaleMode == ScaleType.None)
			{
				strength = strengthRange.GetLerpedValue(1f);
				SetEmissionsStrength(strength);
			}
			if (loopRequested || playStopMode.HasFlag(PlayStop.PlayOnEnter))
			{
				StartVibration();
			}
			if (distanceScaleMode != 0)
			{
				StartStrengthUpdate();
			}
		}
	}

	private void OnTriggerExited(Collider2D collider, GameObject sender)
	{
		if (IsCorrectTarget(collider))
		{
			isInside = false;
			if (playStopMode.HasFlag(PlayStop.StopOnExit))
			{
				StopVibration();
			}
			else if (requireInside)
			{
				StopMainVibration();
			}
			StopEmissions(VibrationSettings.StopOnExit);
			if (distanceScaleMode == ScaleType.None)
			{
				strength = strengthRange.GetLerpedValue(0f);
				SetEmissionsStrength(strength);
			}
			StopStrengthUpdate();
		}
	}

	private bool IsCorrectTarget(Collider2D collider2D)
	{
		return collider2D.CompareTag("Player");
	}

	public void StartVibration()
	{
		if (loop)
		{
			loopRequested = true;
		}
		if (requireInside && !isInside)
		{
			return;
		}
		VibrationEmission vibrationEmission = null;
		if (loop)
		{
			if (mainEmissionLoop != null && mainEmissionLoop.IsPlaying && mainEmissionLoop.IsLooping)
			{
				return;
			}
			VibrationData vibrationData = vibrationDataAsset;
			bool isLooping = loop;
			bool isRealtime = isRealTime;
			mainEmissionLoop = VibrationManager.PlayVibrationClipOneShot(vibrationData, null, isLooping, "", isRealtime);
			mainEmissionLoop?.SetStrength(strength * vibrationDataStrength);
			vibrationEmission = mainEmissionLoop;
		}
		else
		{
			VibrationSettings vibrationSettings = VibrationSettings.ControlledByRegion;
			if (isRealTime)
			{
				vibrationSettings |= VibrationSettings.RealTime;
			}
			if (playStopMode.HasFlag(PlayStop.StopOnExit))
			{
				vibrationSettings |= VibrationSettings.StopOnExit;
			}
			vibrationEmission = AddEmission(vibrationDataAsset, vibrationDataStrength, vibrationSettings);
		}
		this.MainEmissionStarted?.Invoke(vibrationEmission);
	}

	public void StopMainVibration()
	{
		if (mainEmissionLoop != null)
		{
			mainEmissionLoop.Stop();
			mainEmissionLoop = null;
		}
	}

	public void StopVibration()
	{
		loopRequested = false;
		StopMainVibration();
		StopEmissions(VibrationSettings.ControlledByRegion);
	}

	private void StartStrengthUpdate()
	{
		scale = GetScaleSetting();
		HeroController instance = HeroController.instance;
		if ((bool)instance)
		{
			heroTransform = instance.transform;
			doStrengthUpdate = true;
			base.enabled = true;
		}
	}

	private void StopStrengthUpdate()
	{
		if (doStrengthUpdate)
		{
			doStrengthUpdate = false;
			strength = strengthRange.GetLerpedValue(0f);
		}
	}

	private bool StrengthUpdate()
	{
		if (doStrengthUpdate)
		{
			float intensity = scale.GetIntensity(heroTransform.position, vibrationCentre.position);
			strength = strengthRange.GetLerpedValue(strengthCurve.Evaluate(intensity));
			SetEmissionsStrength(strength);
			return true;
		}
		return false;
	}

	private void StartEmissionsUpdate()
	{
		if (emissions.Count > 0)
		{
			base.enabled = true;
		}
	}

	private bool EmissionsUpdate()
	{
		if (emissions.Count > 0)
		{
			for (int num = emissions.Count - 1; num >= 0; num--)
			{
				if (!emissions[num].emission.IsPlaying)
				{
					emissions.RemoveAt(num);
				}
			}
		}
		bool flag = mainEmissionLoop != null;
		if (flag && !mainEmissionLoop.IsPlaying)
		{
			mainEmissionLoop = null;
			flag = false;
		}
		if (!flag)
		{
			return emissions.Count > 0;
		}
		return true;
	}

	private void SetEmissionsStrength(float value)
	{
		if (onlyFeltInsideRegion && !isInside)
		{
			value = 0f;
		}
		mainEmissionLoop?.SetStrength(vibrationDataStrength * value);
		foreach (EmissionTracker emission in emissions)
		{
			emission.SetStrength(value);
		}
	}

	private ScaleSettings GetScaleSetting()
	{
		return distanceScaleMode switch
		{
			ScaleType.Radial => radialScaleSettings, 
			ScaleType.Rectangle => rectangleScaleSettings, 
			_ => radialScaleSettings, 
		};
	}

	public VibrationEmission PlayVibrationOneShot(VibrationData vibrationData, bool requireInside, VibrationSettings vibrationSettings = VibrationSettings.None, string tag = null)
	{
		if (requireInside && !isInside)
		{
			return null;
		}
		return AddEmission(vibrationData, vibrationSettings, tag);
	}

	private VibrationEmission AddEmission(VibrationData vibrationData, VibrationSettings vibrationSettings = VibrationSettings.None, string tag = null)
	{
		return AddEmission(vibrationData, 1f, vibrationSettings, tag);
	}

	private VibrationEmission AddEmission(VibrationData vibrationData, float baseStrength, VibrationSettings vibrationSettings = VibrationSettings.None, string tag = null)
	{
		bool flag = vibrationSettings.HasFlag(VibrationSettings.Loop);
		bool flag2 = vibrationSettings.HasFlag(VibrationSettings.RealTime);
		bool isLooping = flag;
		bool isRealtime = flag2;
		VibrationEmission vibrationEmission = VibrationManager.PlayVibrationClipOneShot(vibrationData, null, isLooping, tag, isRealtime);
		if (vibrationEmission != null)
		{
			float num = strength * baseStrength;
			if (onlyFeltInsideRegion && !isInside)
			{
				num = 0f;
			}
			vibrationEmission.SetStrength(num);
			emissions.Add(new EmissionTracker(vibrationEmission, vibrationSettings, baseStrength));
			StartEmissionsUpdate();
		}
		return vibrationEmission;
	}

	private void StopEmissions(VibrationSettings settings)
	{
		if (settings == VibrationSettings.None)
		{
			for (int num = emissions.Count - 1; num >= 0; num--)
			{
				emissions[num].emission.Stop();
			}
			emissions.Clear();
			return;
		}
		for (int num2 = emissions.Count - 1; num2 >= 0; num2--)
		{
			EmissionTracker emissionTracker = emissions[num2];
			if ((emissionTracker.settings & settings) != 0)
			{
				emissionTracker.emission.Stop();
				emissions.RemoveAt(num2);
			}
		}
	}

	private void DrawGizmos()
	{
		Transform transform = vibrationCentre;
		if (!transform)
		{
			transform = (heroDetector ? heroDetector.transform : base.transform);
		}
		switch (distanceScaleMode)
		{
		case ScaleType.Radial:
		{
			Vector3 position2 = transform.position;
			Gizmos.color = Color.yellow.SetAlpha(0.5f);
			Gizmos.DrawWireSphere(position2, radialScaleSettings.minIntensityRadius);
			Gizmos.color = Color.green.SetAlpha(0.5f);
			Gizmos.DrawWireSphere(position2, radialScaleSettings.maxIntensityRadius);
			break;
		}
		case ScaleType.Rectangle:
		{
			Vector3 position = transform.position;
			Gizmos.color = Color.yellow.SetAlpha(0.5f);
			Gizmos.DrawWireCube(position, new Vector3(rectangleScaleSettings.minIntensityWidth, rectangleScaleSettings.minIntensityHeight));
			Gizmos.color = Color.green.SetAlpha(0.5f);
			Gizmos.DrawWireCube(position, new Vector3(rectangleScaleSettings.maxIntensityWidth, rectangleScaleSettings.maxIntensityHeight));
			break;
		}
		}
	}

	private void OnDrawGizmos()
	{
		if (alwaysDrawGizmos || GizmoUtility.IsChildSelected(base.transform))
		{
			DrawGizmos();
		}
	}

	private void OnDrawGizmosSelected()
	{
		if (!alwaysDrawGizmos)
		{
			DrawGizmos();
		}
	}
}

using UnityEngine;

[CreateAssetMenu(menuName = "Camera/Camera Shake Profile")]
public class CameraShakeProfile : ScriptableObject, ICameraShake, ICameraShakeVibration
{
	private enum CameraShakeTypes
	{
		Shake = 0,
		Rumble = 1,
		Sway = 2
	}

	[SerializeField]
	private CameraShakeTypes type;

	[SerializeField]
	[ModifiableProperty]
	[Conditional("ShowDecayField", true, true, true)]
	private AnimationCurve decay = AnimationCurve.Constant(0f, 1f, 1f);

	[SerializeField]
	private bool decayVibrations;

	[SerializeField]
	[ModifiableProperty]
	[Conditional("ShowDurationField", true, true, true)]
	private float duration = 1f;

	[SerializeField]
	[ModifiableProperty]
	[Conditional("ShowDurationField", true, true, true)]
	private bool endAfterDuration;

	[SerializeField]
	private float magnitude = 1f;

	[SerializeField]
	[ModifiableProperty]
	[Conditional("ShowSwayFields", false, true, true)]
	private int freezeFrames = 1;

	[SerializeField]
	[ModifiableProperty]
	[Conditional("ShowSwayFields", true, true, true)]
	private AnimationCurve swayX;

	[SerializeField]
	[ModifiableProperty]
	[Conditional("ShowSwayFields", true, true, true)]
	private AnimationCurve swayY;

	[SerializeField]
	[ModifiableProperty]
	[Conditional("ShowSwayFields", true, true, true)]
	private bool useRandomShake;

	[Space]
	[SerializeField]
	private CameraShakeWorldForceIntensities worldForceOnStart;

	[Space]
	[SerializeField]
	private VibrationData vibration;

	private Vector2 randomOffsetThisFrame;

	private int randomOffsetFrame;

	public float Magnitude => magnitude;

	public int FreezeFrames
	{
		get
		{
			if (ShowSwayFields())
			{
				return 0;
			}
			return freezeFrames;
		}
	}

	public bool CanFinish
	{
		get
		{
			if (type != 0)
			{
				if (type == CameraShakeTypes.Sway)
				{
					return endAfterDuration;
				}
				return false;
			}
			return true;
		}
	}

	public bool LimitUpdates
	{
		get
		{
			if (type == CameraShakeTypes.Sway && !useRandomShake)
			{
				return true;
			}
			return false;
		}
	}

	public CameraShakeWorldForceIntensities WorldForceOnStart => worldForceOnStart;

	public ICameraShakeVibration CameraShakeVibration => this;

	private bool ShowDecayField()
	{
		return type == CameraShakeTypes.Shake;
	}

	private bool ShowDurationField()
	{
		return type != CameraShakeTypes.Rumble;
	}

	private bool ShowSwayFields()
	{
		return type == CameraShakeTypes.Sway;
	}

	public VibrationEmission PlayVibration(bool isRealtime)
	{
		bool isRealtime2;
		if (type == CameraShakeTypes.Rumble)
		{
			VibrationData vibrationData = vibration;
			isRealtime2 = isRealtime;
			return VibrationManager.PlayVibrationClipOneShot(vibrationData, null, isLooping: true, "", isRealtime2);
		}
		VibrationData vibrationData2 = vibration;
		isRealtime2 = isRealtime;
		return VibrationManager.PlayVibrationClipOneShot(vibrationData2, null, isLooping: false, "", isRealtime2);
	}

	public float GetVibrationStrength(float timeElapsed)
	{
		if (decayVibrations && type == CameraShakeTypes.Shake)
		{
			return decay.Evaluate(timeElapsed);
		}
		return 1f;
	}

	public Vector2 GetOffset(float elapsedTime)
	{
		if (duration <= 0f)
		{
			return Vector2.zero;
		}
		float time = elapsedTime / duration;
		if (Time.frameCount != randomOffsetFrame)
		{
			randomOffsetFrame = Time.frameCount;
			randomOffsetThisFrame = Random.insideUnitCircle;
		}
		switch (type)
		{
		case CameraShakeTypes.Shake:
		{
			float num = decay.Evaluate(time);
			return randomOffsetThisFrame * (magnitude * num);
		}
		case CameraShakeTypes.Rumble:
			return randomOffsetThisFrame * magnitude;
		case CameraShakeTypes.Sway:
		{
			if (!useRandomShake)
			{
				return new Vector2(swayX.Evaluate(time), swayY.Evaluate(time)) * magnitude;
			}
			Vector2 vector = randomOffsetThisFrame * magnitude;
			return new Vector2(vector.x * swayX.Evaluate(time), vector.y * swayY.Evaluate(time));
		}
		default:
			return Vector2.zero;
		}
	}

	public bool IsDone(float elapsedTime)
	{
		switch (type)
		{
		case CameraShakeTypes.Shake:
			return elapsedTime >= duration;
		case CameraShakeTypes.Rumble:
			return false;
		case CameraShakeTypes.Sway:
			if (endAfterDuration)
			{
				return elapsedTime >= duration;
			}
			return false;
		default:
			return true;
		}
	}
}

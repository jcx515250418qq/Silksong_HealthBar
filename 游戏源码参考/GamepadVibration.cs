using UnityEngine;

[CreateAssetMenu(fileName = "GamepadVibration", menuName = "Hollow Knight/Gamepad Vibration", order = 164)]
public class GamepadVibration : ScriptableObject
{
	[SerializeField]
	private AnimationCurve smallMotor;

	[SerializeField]
	private AnimationCurve largeMotor;

	[SerializeField]
	[Range(0.01f, 5f)]
	private float playbackRate;

	public AnimationCurve SmallMotor => smallMotor;

	public AnimationCurve LargeMotor => largeMotor;

	public float PlaybackRate => playbackRate;

	protected void Reset()
	{
		smallMotor = AnimationCurve.Constant(0f, 1f, 1f);
		largeMotor = AnimationCurve.Constant(0f, 1f, 1f);
		playbackRate = 1f;
	}

	public float GetDuration()
	{
		return Mathf.Max(GetDuration(smallMotor), GetDuration(largeMotor));
	}

	private static float GetDuration(AnimationCurve animationCurve)
	{
		if (animationCurve.length == 0)
		{
			return 0f;
		}
		return animationCurve[animationCurve.length - 1].time;
	}
}

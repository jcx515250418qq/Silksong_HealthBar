using System;
using System.Collections.Generic;
using UnityEngine;
using UnityStandardAssets.ImageEffects;

public class ColorCorrectionCurvesAnimator : MonoBehaviour
{
	[Serializable]
	private class State
	{
		public float Saturation = 1f;

		public AnimationCurve RedChannel = new AnimationCurve(new Keyframe(0f, 0f), new Keyframe(1f, 1f));

		public AnimationCurve GreenChannel = new AnimationCurve(new Keyframe(0f, 0f), new Keyframe(1f, 1f));

		public AnimationCurve BlueChannel = new AnimationCurve(new Keyframe(0f, 0f), new Keyframe(1f, 1f));
	}

	[SerializeField]
	private ColorCorrectionCurves curves;

	[SerializeField]
	private State startState;

	[SerializeField]
	private State endState;

	[SerializeField]
	[Conditional("getDurationFromCinematic", false, false, false)]
	private float duration;

	[SerializeField]
	private CinematicPlayer getDurationFromCinematic;

	private Coroutine routine;

	private List<Keyframe[]> redPairedKeyframes;

	private List<Keyframe[]> greenPairedKeyframes;

	private List<Keyframe[]> bluePairedKeyframes;

	private void Awake()
	{
		redPairedKeyframes = ColorCurvesManager.PairKeyframes(startState.RedChannel, endState.RedChannel);
		greenPairedKeyframes = ColorCurvesManager.PairKeyframes(startState.GreenChannel, endState.GreenChannel);
		bluePairedKeyframes = ColorCurvesManager.PairKeyframes(startState.BlueChannel, endState.BlueChannel);
	}

	public void DoPlay()
	{
		if (routine != null)
		{
			StopCoroutine(routine);
		}
		routine = this.StartTimerRoutine(0f, getDurationFromCinematic ? getDurationFromCinematic.Duration : duration, delegate(float t)
		{
			curves.saturation = Mathf.Lerp(startState.Saturation, endState.Saturation, t);
			curves.redChannel = ColorCurvesManager.CreateCurveFromKeyframes(redPairedKeyframes, t);
			curves.greenChannel = ColorCurvesManager.CreateCurveFromKeyframes(greenPairedKeyframes, t);
			curves.blueChannel = ColorCurvesManager.CreateCurveFromKeyframes(bluePairedKeyframes, t);
			curves.UpdateParameters();
			curves.UpdateMaterial();
		}, null, delegate
		{
			routine = null;
		});
	}
}

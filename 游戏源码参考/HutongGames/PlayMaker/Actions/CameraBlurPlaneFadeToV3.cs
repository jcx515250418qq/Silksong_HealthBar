using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
	public class CameraBlurPlaneFadeToV3 : FsmStateAction
	{
		public FsmFloat Spacing;

		public FsmFloat Vibrancy;

		public FsmFloat MaskLerp;

		public FsmFloat MaskScale;

		public FsmAnimationCurve Curve;

		public FsmFloat Duration;

		private float fromSpacing;

		private float fromVibrancy;

		private float fromMaskLerp;

		private float fromMaskScale;

		public override void Reset()
		{
			Spacing = null;
			Vibrancy = null;
			MaskLerp = null;
			MaskScale = 1f;
			Curve = new FsmAnimationCurve
			{
				curve = AnimationCurve.Linear(0f, 0f, 1f, 1f)
			};
			Duration = null;
		}

		public override void OnEnter()
		{
			if (Duration.Value <= 0f)
			{
				if (!Spacing.IsNone)
				{
					CameraBlurPlane.Spacing = Spacing.Value;
				}
				if (!Vibrancy.IsNone)
				{
					CameraBlurPlane.Vibrancy = Vibrancy.Value;
				}
				if (!MaskLerp.IsNone)
				{
					CameraBlurPlane.MaskLerp = MaskLerp.Value;
				}
				if (!MaskScale.IsNone)
				{
					CameraBlurPlane.MaskScale = MaskScale.Value;
				}
				Finish();
			}
			fromSpacing = CameraBlurPlane.Spacing;
			fromVibrancy = CameraBlurPlane.Vibrancy;
			fromMaskLerp = CameraBlurPlane.MaskLerp;
			fromMaskScale = CameraBlurPlane.MaskScale;
		}

		public override void OnUpdate()
		{
			float progress = GetProgress();
			float t = Curve.curve.Evaluate(progress);
			if (!Spacing.IsNone)
			{
				CameraBlurPlane.Spacing = Mathf.Lerp(fromSpacing, Spacing.Value, t);
			}
			if (!Vibrancy.IsNone)
			{
				CameraBlurPlane.Vibrancy = Mathf.Lerp(fromVibrancy, Vibrancy.Value, t);
			}
			if (!MaskLerp.IsNone)
			{
				CameraBlurPlane.MaskLerp = Mathf.Lerp(fromMaskLerp, MaskLerp.Value, t);
			}
			if (!MaskScale.IsNone)
			{
				CameraBlurPlane.MaskScale = Mathf.Lerp(fromMaskScale, MaskScale.Value, t);
			}
			if (progress >= 1f)
			{
				Finish();
			}
		}

		public override float GetProgress()
		{
			return Mathf.Clamp01(base.State.StateTime / Duration.Value);
		}
	}
}

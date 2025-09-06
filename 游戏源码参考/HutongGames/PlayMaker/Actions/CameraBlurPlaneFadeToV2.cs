using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
	public class CameraBlurPlaneFadeToV2 : FsmStateAction
	{
		public FsmFloat Spacing;

		public FsmFloat Vibrancy;

		public FsmFloat MaskLerp;

		public FsmAnimationCurve Curve;

		public FsmFloat Duration;

		private float fromSpacing;

		private float fromVibrancy;

		private float fromMaskLerp;

		public override void Reset()
		{
			Spacing = null;
			Vibrancy = null;
			MaskLerp = null;
			Curve = new FsmAnimationCurve
			{
				curve = AnimationCurve.Linear(0f, 0f, 1f, 1f)
			};
			Duration = null;
		}

		public override void OnEnter()
		{
			CameraBlurPlane.MaskScale = 1f;
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
				Finish();
			}
			fromSpacing = CameraBlurPlane.Spacing;
			fromVibrancy = CameraBlurPlane.Vibrancy;
			fromMaskLerp = CameraBlurPlane.MaskLerp;
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

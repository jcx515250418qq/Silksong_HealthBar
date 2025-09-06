using System;
using JetBrains.Annotations;
using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
	[UsedImplicitly]
	public class CameraBlurPlaneFade : FsmStateAction
	{
		[Serializable]
		public class Config
		{
			public FsmFloat Spacing;

			public FsmFloat Vibrancy;

			public FsmFloat MaskLerp;
		}

		[HideIf("HideFrom")]
		public Config From;

		public Config To;

		public FsmAnimationCurve Curve;

		public FsmFloat Duration;

		public bool HideFrom()
		{
			if (Duration != null)
			{
				return Duration.Value <= 0f;
			}
			return false;
		}

		public override void Reset()
		{
			From = null;
			To = null;
			Curve = new FsmAnimationCurve
			{
				curve = AnimationCurve.Linear(0f, 0f, 1f, 1f)
			};
			Duration = null;
		}

		public override void OnEnter()
		{
			if (HideFrom())
			{
				CameraBlurPlane.Spacing = To.Spacing.Value;
				CameraBlurPlane.Vibrancy = To.Vibrancy.Value;
				CameraBlurPlane.MaskLerp = To.MaskLerp?.Value ?? 0f;
				Finish();
			}
			else
			{
				CameraBlurPlane.Spacing = From.Spacing.Value;
				CameraBlurPlane.Vibrancy = From.Vibrancy.Value;
				CameraBlurPlane.MaskLerp = From.MaskLerp?.Value ?? 0f;
			}
		}

		public override void OnUpdate()
		{
			float progress = GetProgress();
			float t = Curve.curve.Evaluate(progress);
			CameraBlurPlane.Spacing = Mathf.Lerp(From.Spacing.Value, To.Spacing.Value, t);
			CameraBlurPlane.Vibrancy = Mathf.Lerp(From.Vibrancy.Value, To.Vibrancy.Value, t);
			if (From.MaskLerp != null && To.MaskLerp != null)
			{
				CameraBlurPlane.MaskLerp = Mathf.Lerp(From.MaskLerp.Value, To.MaskLerp.Value, t);
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

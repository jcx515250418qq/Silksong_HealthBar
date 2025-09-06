using HutongGames.PlayMaker.TweenEnums;
using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
	[ActionCategory(ActionCategory.Tween)]
	[Tooltip("Tween a GameObject's scale.")]
	public class TweenScale : TweenComponentBase<Transform>
	{
		[ActionSection("From")]
		[Title("Options")]
		[Tooltip("Setup where to tween from.")]
		public ScaleOptions fromOptions;

		[Tooltip("Tween from this Target GameObject.")]
		public FsmGameObject fromTarget;

		[Tooltip("Tween from this Scale.")]
		public FsmVector3 fromScale;

		[ActionSection("To")]
		[Title("Options")]
		[Tooltip("Setup where to tween to.")]
		public ScaleOptions toOptions;

		[Tooltip("Tween to this Target GameObject.")]
		public FsmGameObject toTarget;

		[Tooltip("Tween to this Scale.")]
		public FsmVector3 toScale;

		private Transform transform;

		private Transform fromTransform;

		private Transform toTransform;

		public Vector3 StartScale { get; private set; }

		public Vector3 EndScale { get; private set; }

		public override void Reset()
		{
			base.Reset();
			fromOptions = ScaleOptions.CurrentScale;
			fromTarget = null;
			fromScale = new FsmVector3
			{
				Value = Vector3.one
			};
			toOptions = ScaleOptions.LocalScale;
			toTarget = null;
			toScale = new FsmVector3
			{
				Value = Vector3.one
			};
		}

		public override void OnEnter()
		{
			base.OnEnter();
			if (!base.Finished)
			{
				transform = cachedComponent;
				fromTransform = ((fromTarget.Value != null) ? fromTarget.Value.transform : null);
				toTransform = ((toTarget.Value != null) ? toTarget.Value.transform : null);
				InitStartScale();
				InitEndScale();
				DoTween();
			}
		}

		private void InitStartScale()
		{
			StartScale = TweenHelpers.GetTargetScale(fromOptions, transform, fromTransform, fromScale.IsNone ? Vector3.one : fromScale.Value);
		}

		private void InitEndScale()
		{
			EndScale = TweenHelpers.GetTargetScale(toOptions, transform, toTransform, toScale.IsNone ? Vector3.one : toScale.Value);
		}

		private void UpdateStartScale()
		{
			if (fromOptions == ScaleOptions.LocalScale || fromOptions == ScaleOptions.MatchGameObject)
			{
				InitStartScale();
			}
		}

		private void UpdateEndScale()
		{
			if (fromOptions == ScaleOptions.LocalScale || fromOptions == ScaleOptions.MatchGameObject)
			{
				InitEndScale();
			}
		}

		public override void OnUpdate()
		{
			base.OnUpdate();
			if (fromOptions == ScaleOptions.MatchGameObject)
			{
				InitStartScale();
			}
			if (fromOptions == ScaleOptions.MatchGameObject)
			{
				InitEndScale();
			}
		}

		protected override void DoTween()
		{
			float t = base.easingFunction(0f, 1f, normalizedTime);
			transform.localScale = Vector3.Lerp(StartScale, EndScale, t);
		}
	}
}

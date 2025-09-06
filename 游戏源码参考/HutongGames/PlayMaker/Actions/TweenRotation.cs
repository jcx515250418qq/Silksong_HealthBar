using HutongGames.PlayMaker.TweenEnums;
using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
	[ActionCategory(ActionCategory.Tween)]
	[Tooltip("Tween a GameObject's rotation.")]
	public class TweenRotation : TweenComponentBase<Transform>
	{
		[ActionSection("From")]
		[Title("Options")]
		[Tooltip("Setup where to tween from.")]
		public RotationOptions fromOptions;

		[Tooltip("Use this GameObject's rotation.")]
		public FsmGameObject fromTarget;

		[Tooltip("Tween from this rotation")]
		public FsmVector3 fromRotation;

		[ActionSection("To")]
		[Title("Options")]
		[Tooltip("Setup where to tween to.")]
		public RotationOptions toOptions;

		[Tooltip("Use this GameObject's rotation")]
		public FsmGameObject toTarget;

		[Tooltip("Tween to this rotation.")]
		public FsmVector3 toRotation;

		private Transform transform;

		private Transform fromTransform;

		private Transform toTransform;

		private Quaternion midRotation;

		public Quaternion StartRotation { get; private set; }

		public Quaternion EndRotation { get; private set; }

		public override void Reset()
		{
			base.Reset();
			fromOptions = RotationOptions.CurrentRotation;
			fromTarget = null;
			fromRotation = null;
			toOptions = RotationOptions.WorldRotation;
			toTarget = null;
			toRotation = null;
		}

		public override void OnEnter()
		{
			base.OnEnter();
			if (!base.Finished)
			{
				transform = cachedComponent;
				fromTransform = ((fromTarget.Value != null) ? fromTarget.Value.transform : null);
				toTransform = ((toTarget.Value != null) ? toTarget.Value.transform : null);
				InitStartRotation();
				InitEndRotation();
				DoTween();
			}
		}

		private void InitStartRotation()
		{
			StartRotation = TweenHelpers.GetTargetRotation(fromOptions, transform, fromTransform, fromRotation.Value);
		}

		private void InitEndRotation()
		{
			EndRotation = TweenHelpers.GetTargetRotation(toOptions, transform, toTransform, toRotation.Value);
			midRotation = TweenHelpers.GetTargetRotation(toOptions, transform, toTransform, toRotation.Value / 2f);
		}

		private void UpdateStartRotation()
		{
			if (fromOptions == RotationOptions.MatchGameObjectRotation)
			{
				InitStartRotation();
			}
		}

		private void UpdateEndRotation()
		{
			if (toOptions == RotationOptions.MatchGameObjectRotation)
			{
				InitEndRotation();
			}
		}

		public override void OnUpdate()
		{
			base.OnUpdate();
			UpdateStartRotation();
			UpdateEndRotation();
		}

		protected override void DoTween()
		{
			float num = base.easingFunction(0f, 1f, normalizedTime);
			if ((double)num < 0.5)
			{
				transform.rotation = Quaternion.Slerp(StartRotation, midRotation, num * 2f);
			}
			else
			{
				transform.rotation = Quaternion.Slerp(midRotation, EndRotation, (num - 0.5f) * 2f);
			}
		}
	}
}

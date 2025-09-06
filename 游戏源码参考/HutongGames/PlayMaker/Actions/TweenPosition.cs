using System;
using HutongGames.PlayMaker.TweenEnums;
using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
	[ActionCategory(ActionCategory.Tween)]
	[Tooltip("Tween a GameObject's position. Note: This action assumes that GameObject targets do not change during the tween.")]
	public class TweenPosition : TweenComponentBase<Transform>
	{
		[ActionSection("From")]
		[Title("Options")]
		[Tooltip("Setup where to tween from.")]
		public PositionOptions fromOption;

		[Tooltip("Optionally use a GameObject as the from position.")]
		public FsmGameObject fromTarget;

		[Tooltip("Position to tween from.")]
		public FsmVector3 fromPosition;

		[ActionSection("To")]
		[Title("Options")]
		[Tooltip("Setup where to tween from.")]
		public PositionOptions toOption;

		[Tooltip("Optionally use a GameObject as the to position.")]
		public FsmGameObject toTarget;

		[Tooltip("Position to tween to.")]
		public FsmVector3 toPosition;

		[NonSerialized]
		private Transform transform;

		[NonSerialized]
		private Transform fromTransform;

		[NonSerialized]
		private Transform toTransform;

		public Vector3 StartPosition { get; private set; }

		public Vector3 EndPosition { get; private set; }

		public override void Reset()
		{
			base.Reset();
			fromOption = PositionOptions.CurrentPosition;
			fromTarget = null;
			fromPosition = null;
			toOption = PositionOptions.WorldPosition;
			toTarget = null;
			toPosition = null;
		}

		public override void OnEnter()
		{
			base.OnEnter();
			if (!base.Finished)
			{
				transform = cachedComponent;
				fromTransform = ((fromTarget.Value != null) ? fromTarget.Value.transform : null);
				toTransform = ((toTarget.Value != null) ? toTarget.Value.transform : null);
				InitStartPosition();
				InitEndPosition();
				transform.position = StartPosition;
			}
		}

		private void InitStartPosition()
		{
			StartPosition = TweenHelpers.GetTargetPosition(fromOption, transform, fromTransform, fromPosition.Value);
		}

		private void InitEndPosition()
		{
			EndPosition = TweenHelpers.GetTargetPosition(toOption, transform, toTransform, toPosition.Value);
		}

		protected override void DoTween()
		{
			transform.position = Vector3.Lerp(StartPosition, EndPosition, base.easingFunction(0f, 1f, normalizedTime));
		}
	}
}

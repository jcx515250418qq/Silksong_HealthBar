using System;
using HutongGames.PlayMaker.TweenEnums;
using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
	[ActionCategory(ActionCategory.Tween)]
	[ActionTarget(typeof(Camera), "", false)]
	[Tooltip("Tween position of UI GameObjects.")]
	public class TweenUiPosition : TweenComponentBase<RectTransform>
	{
		[ActionSection("From")]
		[Title("Options")]
		[Tooltip("Setup where to tween from.")]
		public UiPositionOptions fromOption;

		[Tooltip("Optionally use a GameObject as the from position.")]
		public FsmGameObject fromTarget;

		[Tooltip("If a GameObject is specified, use this as an offset. Otherwise this is a world position.")]
		public FsmVector3 fromPosition;

		[ActionSection("To")]
		[Title("Options")]
		[Tooltip("Setup where to tween from.")]
		public UiPositionOptions toOption;

		[Tooltip("Optionally use a GameObject as the to position.")]
		public FsmGameObject toTarget;

		[Tooltip("If a GameObject is specified, use this as an offset. Otherwise this is a world position.")]
		public FsmVector3 toPosition;

		[NonSerialized]
		private RectTransform transform;

		[NonSerialized]
		private Transform fromTransform;

		[NonSerialized]
		private Transform toTransform;

		public Vector3 StartPosition { get; private set; }

		public Vector3 EndPosition { get; private set; }

		public override void Reset()
		{
			base.Reset();
			fromOption = UiPositionOptions.CurrentPosition;
			fromTarget = null;
			fromPosition = null;
			toOption = UiPositionOptions.Position;
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
				transform.anchoredPosition3D = StartPosition;
			}
		}

		private void InitStartPosition()
		{
			StartPosition = TweenHelpers.GetUiTargetPosition(fromOption, transform, fromTransform, fromPosition.Value);
		}

		private void InitEndPosition()
		{
			EndPosition = TweenHelpers.GetUiTargetPosition(toOption, transform, toTransform, toPosition.Value);
		}

		protected override void DoTween()
		{
			transform.anchoredPosition3D = Vector3.Lerp(StartPosition, EndPosition, base.easingFunction(0f, 1f, normalizedTime));
		}
	}
}

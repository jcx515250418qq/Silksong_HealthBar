using System;
using HutongGames.PlayMaker.TweenEnums;
using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
	[ActionCategory(ActionCategory.Tween)]
	public abstract class TweenActionBase : BaseUpdateAction
	{
		[ActionSection("Easing")]
		[Tooltip("Delay before starting the tween.")]
		public FsmFloat startDelay;

		[Tooltip("The type of easing to apply.")]
		[ObjectType(typeof(EasingFunction.Ease))]
		[PreviewField("DrawPreview")]
		public FsmEnum easeType;

		[Tooltip("Custom tween curve. Note: Typically you would use the 0-1 range.")]
		[HideIf("HideCustomCurve")]
		public FsmAnimationCurve customCurve;

		[Tooltip("Length of tween in seconds.")]
		public FsmFloat time;

		[Tooltip("Ignore any time scaling.")]
		public FsmBool realTime;

		[Tooltip("Looping options.")]
		public LoopType loopType;

		[Tooltip("Event to send when tween is finished.")]
		public FsmEvent finishEvent;

		[NonSerialized]
		public float normalizedTime;

		protected bool tweenStarted;

		protected bool tweenFinished;

		protected float currentTime;

		protected bool playPreview;

		private EasingFunction.Ease cachedEase;

		private EasingFunction.Function func;

		private static bool showPreviewCurve;

		private bool reverse;

		public EasingFunction.Function easingFunction
		{
			get
			{
				EasingFunction.Ease ease = (EasingFunction.Ease)(object)easeType.Value;
				if (cachedEase != ease || func == null)
				{
					func = EasingFunction.GetEasingFunction(ease);
					cachedEase = ease;
				}
				return func;
			}
		}

		public override void Reset()
		{
			base.Reset();
			startDelay = null;
			easeType = null;
			time = 1f;
			realTime = false;
			finishEvent = null;
			loopType = LoopType.None;
		}

		public override void OnEnter()
		{
			currentTime = 0f;
			normalizedTime = 0f;
			tweenFinished = false;
			tweenStarted = false;
			everyFrame = true;
			reverse = false;
		}

		public override void OnActionUpdate()
		{
			float num = (realTime.Value ? Time.unscaledDeltaTime : Time.deltaTime);
			currentTime += num;
			if (!tweenStarted)
			{
				if (currentTime < startDelay.Value)
				{
					return;
				}
				tweenStarted = true;
				currentTime -= startDelay.Value;
			}
			if (currentTime > time.Value)
			{
				switch (loopType)
				{
				case LoopType.None:
					tweenFinished = true;
					currentTime = time.Value;
					break;
				case LoopType.Loop:
					currentTime -= time.Value;
					break;
				case LoopType.PingPong:
					currentTime -= time.Value;
					reverse = !reverse;
					break;
				default:
					throw new ArgumentOutOfRangeException();
				}
			}
			if (!reverse)
			{
				normalizedTime = currentTime / time.Value;
			}
			else
			{
				normalizedTime = 1f - currentTime / time.Value;
			}
			EasingFunction.AnimationCurve = customCurve.curve;
			DoTween();
			if (tweenFinished)
			{
				Finish();
				base.Fsm.Event(finishEvent);
			}
		}

		protected abstract void DoTween();
	}
}

using System;
using HutongGames.PlayMaker.TweenEnums;

namespace HutongGames.PlayMaker.Actions
{
	[ActionCategory(ActionCategory.Tween)]
	public abstract class TweenPropertyBase<T> : TweenActionBase where T : NamedVariable
	{
		[Title("From")]
		[Tooltip("Setup where to tween from.")]
		public TargetValueOptions fromOption;

		[Tooltip("Tween from this value.")]
		[HideIf("HideFromValue")]
		public T fromValue;

		[Title("To")]
		[Tooltip("Setup where to tween to.")]
		public TargetValueOptions toOption;

		[Tooltip("Tween to this value.")]
		[HideIf("HideToValue")]
		public T toValue;

		public object StartValue { get; protected set; }

		public object EndValue { get; protected set; }

		public override void Reset()
		{
			base.Reset();
			fromOption = TargetValueOptions.CurrentValue;
			fromValue = null;
			toOption = TargetValueOptions.Value;
			toValue = null;
		}

		public override void OnEnter()
		{
			base.OnEnter();
			InitTargets();
			DoTween();
		}

		protected virtual void InitTargets()
		{
			throw new NotImplementedException();
		}

		protected virtual object GetOffsetValue(object value, object offset)
		{
			throw new NotImplementedException();
		}

		protected override void DoTween()
		{
			throw new NotImplementedException();
		}
	}
}

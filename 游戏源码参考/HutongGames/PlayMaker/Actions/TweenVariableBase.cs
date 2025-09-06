using System;
using HutongGames.PlayMaker.TweenEnums;

namespace HutongGames.PlayMaker.Actions
{
	[ActionCategory(ActionCategory.Tween)]
	public abstract class TweenVariableBase<T> : TweenPropertyBase<T> where T : NamedVariable
	{
		[RequiredField]
		[Tooltip("The variable to tween.")]
		[UIHint(UIHint.Variable)]
		public T variable;

		public override void Reset()
		{
			base.Reset();
			variable = null;
			fromOption = TargetValueOptions.CurrentValue;
			fromValue = null;
			toOption = TargetValueOptions.Value;
			toValue = null;
		}

		public override void OnEnter()
		{
			base.OnEnter();
			DoTween();
		}

		protected override void InitTargets()
		{
			switch (fromOption)
			{
			case TargetValueOptions.CurrentValue:
				base.StartValue = variable.RawValue;
				break;
			case TargetValueOptions.Value:
				base.StartValue = fromValue.RawValue;
				break;
			case TargetValueOptions.Offset:
				base.StartValue = GetOffsetValue(variable.RawValue, fromValue.RawValue);
				break;
			default:
				throw new ArgumentOutOfRangeException();
			}
			switch (toOption)
			{
			case TargetValueOptions.CurrentValue:
				base.EndValue = variable.RawValue;
				break;
			case TargetValueOptions.Value:
				base.EndValue = toValue.RawValue;
				break;
			case TargetValueOptions.Offset:
				base.EndValue = GetOffsetValue(variable.RawValue, toValue.RawValue);
				break;
			default:
				throw new ArgumentOutOfRangeException();
			}
		}
	}
}

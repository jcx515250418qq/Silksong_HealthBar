namespace HutongGames.PlayMaker.Actions
{
	[ActionCategory(ActionCategory.Tween)]
	[Tooltip("Tween a float variable using a custom easing function.")]
	public class TweenFloat : TweenVariableBase<FsmFloat>
	{
		protected override object GetOffsetValue(object value, object offset)
		{
			return (float)value + (float)offset;
		}

		protected override void DoTween()
		{
			variable.Value = base.easingFunction((float)base.StartValue, (float)base.EndValue, normalizedTime);
		}
	}
}

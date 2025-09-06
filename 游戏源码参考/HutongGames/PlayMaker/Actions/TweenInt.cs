namespace HutongGames.PlayMaker.Actions
{
	[ActionCategory(ActionCategory.Tween)]
	[Tooltip("Tween an integer variable using a custom easing function. NOTE: Tweening is performed on float values and then rounded to the integer value.")]
	public class TweenInt : TweenVariableBase<FsmInt>
	{
		protected override object GetOffsetValue(object value, object offset)
		{
			return (int)value + (int)offset;
		}

		protected override void DoTween()
		{
			variable.Value = (int)base.easingFunction((int)base.StartValue, (int)base.EndValue, normalizedTime);
		}
	}
}

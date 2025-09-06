using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
	[ActionCategory(ActionCategory.Tween)]
	[Tooltip("Tween a Vector2 variable using a custom easing function.")]
	public class TweenVector2 : TweenVariableBase<FsmVector2>
	{
		protected override object GetOffsetValue(object value, object offset)
		{
			return (Vector2)value + (Vector2)offset;
		}

		protected override void DoTween()
		{
			float t = base.easingFunction(0f, 1f, normalizedTime);
			variable.Value = Vector2.Lerp((Vector2)base.StartValue, (Vector2)base.EndValue, t);
		}
	}
}

using HutongGames.Extensions;
using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
	[ActionCategory(ActionCategory.Tween)]
	[Tooltip("Tween a Rect variable using a custom easing function.")]
	public class TweenRect : TweenVariableBase<FsmRect>
	{
		protected override object GetOffsetValue(object value, object offset)
		{
			Rect rect = (Rect)value;
			Rect rect2 = (Rect)offset;
			return new Rect(rect.x + rect2.x, rect.y + rect2.y, rect.width + rect2.width, rect.height + rect2.height);
		}

		protected override void DoTween()
		{
			float t = base.easingFunction(0f, 1f, normalizedTime);
			variable.Value = variable.Value.Lerp((Rect)base.StartValue, (Rect)base.EndValue, t);
		}
	}
}

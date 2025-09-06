using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
	[ActionCategory(ActionCategory.Tween)]
	[Tooltip("Tween a Vector3 variable using a custom easing function.")]
	public class TweenVector3 : TweenVariableBase<FsmVector3>
	{
		protected override object GetOffsetValue(object value, object offset)
		{
			return (Vector3)value + (Vector3)offset;
		}

		protected override void DoTween()
		{
			float t = base.easingFunction(0f, 1f, normalizedTime);
			variable.Value = Vector3.Lerp((Vector3)base.StartValue, (Vector3)base.EndValue, t);
		}
	}
}

using HutongGames.PlayMaker.TweenEnums;
using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
	[ActionCategory(ActionCategory.Tween)]
	[Tooltip("Tween a Quaternion variable using a custom easing function.")]
	public class TweenQuaternion : TweenVariableBase<FsmQuaternion>
	{
		[Tooltip("Type of interpolation. Linear is faster but looks worse if the rotations are far apart.")]
		[DisplayOrder(1)]
		public RotationInterpolation interpolation;

		protected override object GetOffsetValue(object value, object offset)
		{
			return (Quaternion)value * (Quaternion)offset;
		}

		protected override void DoTween()
		{
			float t = base.easingFunction(0f, 1f, normalizedTime);
			variable.Value = ((interpolation == RotationInterpolation.Linear) ? Quaternion.Lerp(fromValue.Value, toValue.Value, t) : Quaternion.Slerp(fromValue.Value, toValue.Value, t));
		}
	}
}

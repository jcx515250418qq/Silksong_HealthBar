using System;
using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
	[ActionCategory(ActionCategory.Tween)]
	[Tooltip("Punches a GameObject's position, rotation, or scale\u00a0and springs back to starting state")]
	public class TweenPunch : TweenComponentBase<Transform>
	{
		public enum PunchType
		{
			Position = 0,
			Rotation = 1,
			Scale = 2
		}

		[Tooltip("Punch position, rotation, or scale.")]
		public PunchType punchType;

		[Tooltip("Punch magnitude.")]
		public FsmVector3 value;

		private Transform transform;

		private RectTransform rectTransform;

		private Vector3 startVector3;

		private Vector3 endVector3;

		private Quaternion startRotation;

		private Quaternion midRotation;

		private Quaternion endRotation;

		public override void Reset()
		{
			base.Reset();
			punchType = PunchType.Position;
			value = null;
		}

		public override void OnEnter()
		{
			base.OnEnter();
			if (!base.Finished)
			{
				easeType.Value = EasingFunction.Ease.Punch;
				transform = cachedComponent;
				rectTransform = transform as RectTransform;
				switch (punchType)
				{
				case PunchType.Position:
					startVector3 = ((rectTransform != null) ? rectTransform.anchoredPosition3D : transform.position);
					endVector3 = startVector3 + value.Value;
					break;
				case PunchType.Rotation:
					startRotation = transform.rotation;
					midRotation = startRotation * Quaternion.Euler(value.Value * 0.5f);
					endRotation = startRotation * Quaternion.Euler(value.Value);
					break;
				case PunchType.Scale:
					startVector3 = transform.localScale;
					endVector3 = startVector3 + value.Value;
					break;
				default:
					throw new ArgumentOutOfRangeException();
				}
			}
		}

		protected override void DoTween()
		{
			float num = base.easingFunction(0f, 1f, normalizedTime);
			switch (punchType)
			{
			case PunchType.Position:
				if (rectTransform != null)
				{
					rectTransform.anchoredPosition = Vector3.Lerp(startVector3, endVector3, base.easingFunction(0f, 1f, normalizedTime));
				}
				else
				{
					transform.position = Vector3.Lerp(startVector3, endVector3, base.easingFunction(0f, 1f, normalizedTime));
				}
				break;
			case PunchType.Rotation:
				transform.rotation = (((double)num < 0.5) ? Quaternion.Slerp(startRotation, midRotation, num * 2f) : Quaternion.Slerp(midRotation, endRotation, (num - 0.5f) * 2f));
				break;
			case PunchType.Scale:
				transform.localScale = Vector3.Lerp(startVector3, endVector3, num);
				break;
			default:
				throw new ArgumentOutOfRangeException();
			}
		}
	}
}

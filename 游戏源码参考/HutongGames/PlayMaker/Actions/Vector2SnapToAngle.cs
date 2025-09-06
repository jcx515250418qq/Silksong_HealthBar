using System;
using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
	[ActionCategory(ActionCategory.Vector2)]
	[Tooltip("Snap a Vector2 to an angle increment while maintaining length.")]
	public class Vector2SnapToAngle : FsmStateAction
	{
		private static bool showPreview;

		[RequiredField]
		[UIHint(UIHint.Variable)]
		[Tooltip("The vector to snap to an angle.")]
		public FsmVector2 vector2Variable;

		[PreviewField("DrawPreview")]
		[Tooltip("Angle increment to snap to.")]
		public FsmFloat snapAngle;

		[Tooltip("Repeat every frame")]
		public bool everyFrame;

		public override void Reset()
		{
			vector2Variable = null;
			snapAngle = 15f;
			everyFrame = false;
		}

		public override void OnEnter()
		{
			DoSnapToAngle();
			if (!everyFrame)
			{
				Finish();
			}
		}

		public override void OnUpdate()
		{
			DoSnapToAngle();
		}

		private void DoSnapToAngle()
		{
			float value = snapAngle.Value;
			if (!(value <= 0f))
			{
				Vector2 value2 = vector2Variable.Value;
				float magnitude = value2.magnitude;
				float num = Mathf.Atan2(value2.y, value2.y);
				float f = MathF.PI / 180f * Mathf.Round(num / value) * value;
				vector2Variable.Value = new Vector2(Mathf.Cos(f) * magnitude, Mathf.Sin(f) * magnitude);
			}
		}
	}
}

using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
	[ActionCategory(ActionCategory.Physics2D)]
	[Tooltip("Sets the gravity vector, or individual axis.")]
	public class SetGravity2d : FsmStateAction
	{
		[Tooltip("Gravity as Vector2.")]
		public FsmVector2 vector;

		[Tooltip("Override the x value of the gravity")]
		public FsmFloat x;

		[Tooltip("Override the y value of the gravity")]
		public FsmFloat y;

		[Tooltip("Repeat every frame")]
		public bool everyFrame;

		public override void Reset()
		{
			vector = null;
			x = new FsmFloat
			{
				UseVariable = true
			};
			y = new FsmFloat
			{
				UseVariable = true
			};
			everyFrame = false;
		}

		public override void OnEnter()
		{
			DoSetGravity();
			if (!everyFrame)
			{
				Finish();
			}
		}

		public override void OnUpdate()
		{
			DoSetGravity();
		}

		private void DoSetGravity()
		{
			Vector2 value = vector.Value;
			if (!x.IsNone)
			{
				value.x = x.Value;
			}
			if (!y.IsNone)
			{
				value.y = y.Value;
			}
			Physics2D.gravity = value;
		}
	}
}

using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
	[ActionCategory(ActionCategory.Device)]
	[Tooltip("Gets the rotation of the device around its z axis (into the screen). For example when you steer with the iPhone in a driving game.")]
	public class GetDeviceRoll : FsmStateAction
	{
		public enum BaseOrientation
		{
			Portrait = 0,
			LandscapeLeft = 1,
			LandscapeRight = 2
		}

		[Tooltip("How the user is expected to hold the device (where angle will be zero).")]
		public BaseOrientation baseOrientation;

		[UIHint(UIHint.Variable)]
		[Tooltip("Store the roll angle in a Float Variable.")]
		public FsmFloat storeAngle;

		[Tooltip("Limit the roll angle.")]
		public FsmFloat limitAngle;

		[Tooltip("Smooth the roll angle as it changes. You can play with this value to balance responsiveness and lag/smoothness.")]
		public FsmFloat smoothing;

		[Tooltip("Repeat every frame.")]
		public bool everyFrame;

		private float lastZAngle;

		public override void Reset()
		{
			baseOrientation = BaseOrientation.LandscapeLeft;
			storeAngle = null;
			limitAngle = new FsmFloat
			{
				UseVariable = true
			};
			smoothing = 5f;
			everyFrame = true;
		}

		public override void OnEnter()
		{
			DoGetDeviceRoll();
			if (!everyFrame)
			{
				Finish();
			}
		}

		public override void OnUpdate()
		{
			DoGetDeviceRoll();
		}

		private void DoGetDeviceRoll()
		{
			Vector3 deviceAcceleration = ActionHelpers.GetDeviceAcceleration();
			float x = deviceAcceleration.x;
			float y = deviceAcceleration.y;
			float num = 0f;
			switch (baseOrientation)
			{
			case BaseOrientation.Portrait:
				num = 0f - Mathf.Atan2(x, 0f - y);
				break;
			case BaseOrientation.LandscapeLeft:
				num = Mathf.Atan2(y, 0f - x);
				break;
			case BaseOrientation.LandscapeRight:
				num = 0f - Mathf.Atan2(y, x);
				break;
			}
			if (!limitAngle.IsNone)
			{
				num = Mathf.Clamp(57.29578f * num, 0f - limitAngle.Value, limitAngle.Value);
			}
			if (smoothing.Value > 0f)
			{
				num = Mathf.LerpAngle(lastZAngle, num, smoothing.Value * Time.deltaTime);
			}
			lastZAngle = num;
			storeAngle.Value = num;
		}
	}
}

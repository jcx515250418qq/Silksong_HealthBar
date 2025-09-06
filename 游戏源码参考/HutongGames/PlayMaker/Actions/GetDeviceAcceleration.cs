using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
	[ActionCategory(ActionCategory.Device)]
	[Tooltip("Gets the last measured linear acceleration of a device and stores it in a Vector3 Variable.")]
	public class GetDeviceAcceleration : FsmStateAction
	{
		[UIHint(UIHint.Variable)]
		[Tooltip("Store the acceleration in a Vector3 Variable.")]
		public FsmVector3 storeVector;

		[UIHint(UIHint.Variable)]
		[Tooltip("Store the x component of the acceleration in a Float Variable.")]
		public FsmFloat storeX;

		[UIHint(UIHint.Variable)]
		[Tooltip("Store the y component of the acceleration in a Float Variable.")]
		public FsmFloat storeY;

		[UIHint(UIHint.Variable)]
		[Tooltip("Store the z component of the acceleration in a Float Variable.")]
		public FsmFloat storeZ;

		[Tooltip("Multiply the acceleration by a float value.")]
		public FsmFloat multiplier;

		[Tooltip("Repeat every frame.")]
		public bool everyFrame;

		public override void Reset()
		{
			storeVector = null;
			storeX = null;
			storeY = null;
			storeZ = null;
			multiplier = 1f;
			everyFrame = false;
		}

		public override void OnEnter()
		{
			DoGetDeviceAcceleration();
			if (!everyFrame)
			{
				Finish();
			}
		}

		public override void OnUpdate()
		{
			DoGetDeviceAcceleration();
		}

		private void DoGetDeviceAcceleration()
		{
			Vector3 deviceAcceleration = ActionHelpers.GetDeviceAcceleration();
			if (!multiplier.IsNone)
			{
				deviceAcceleration *= multiplier.Value;
			}
			storeVector.Value = deviceAcceleration;
			storeX.Value = deviceAcceleration.x;
			storeY.Value = deviceAcceleration.y;
			storeZ.Value = deviceAcceleration.z;
		}
	}
}

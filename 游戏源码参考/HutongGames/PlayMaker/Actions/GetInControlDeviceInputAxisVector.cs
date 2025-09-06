using InControl;
using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
	[ActionCategory("InControl")]
	[Tooltip("Gets a world direction Vector from 2 Incontrol control Axis for a given device. Typically used for a third person controller with Relative To set to the camera.")]
	public class GetInControlDeviceInputAxisVector : FsmStateAction
	{
		public enum AxisPlane
		{
			XZ = 0,
			XY = 1,
			YZ = 2
		}

		[Tooltip("The index of the device. -1 to use the active device")]
		public FsmInt deviceIndex;

		public InputControlType horizontalAxis;

		public InputControlType verticalAxis;

		[Tooltip("Input axis are reported in the range -1 to 1, this multiplier lets you set a new range.")]
		public FsmFloat multiplier;

		[RequiredField]
		[Tooltip("The world plane to map the 2d input onto.")]
		public AxisPlane mapToPlane;

		[Tooltip("Make the result relative to a GameObject, typically the main camera.")]
		public FsmGameObject relativeTo;

		[RequiredField]
		[UIHint(UIHint.Variable)]
		[Tooltip("Store the direction vector.")]
		public FsmVector3 storeVector;

		[UIHint(UIHint.Variable)]
		[Tooltip("Store the length of the direction vector.")]
		public FsmFloat storeMagnitude;

		private InputDevice _inputDevice;

		private InputControl _inputControl;

		public override void Reset()
		{
			deviceIndex = 0;
			horizontalAxis = InputControlType.LeftStickRight;
			verticalAxis = InputControlType.LeftStickRight;
			multiplier = 1f;
			mapToPlane = AxisPlane.XZ;
			storeVector = null;
			storeMagnitude = null;
		}

		public override void OnUpdate()
		{
			Vector3 vector = default(Vector3);
			Vector3 vector2 = default(Vector3);
			if (relativeTo.Value == null)
			{
				switch (mapToPlane)
				{
				case AxisPlane.XZ:
					vector = Vector3.forward;
					vector2 = Vector3.right;
					break;
				case AxisPlane.XY:
					vector = Vector3.up;
					vector2 = Vector3.right;
					break;
				case AxisPlane.YZ:
					vector = Vector3.up;
					vector2 = Vector3.forward;
					break;
				}
			}
			else
			{
				Transform transform = relativeTo.Value.transform;
				switch (mapToPlane)
				{
				case AxisPlane.XZ:
					vector = transform.TransformDirection(Vector3.forward);
					vector.y = 0f;
					vector = vector.normalized;
					vector2 = new Vector3(vector.z, 0f, 0f - vector.x);
					break;
				case AxisPlane.XY:
				case AxisPlane.YZ:
					vector = Vector3.up;
					vector.z = 0f;
					vector = vector.normalized;
					vector2 = transform.TransformDirection(Vector3.right);
					break;
				}
			}
			if (deviceIndex.Value == -1)
			{
				_inputDevice = InputManager.ActiveDevice;
			}
			else
			{
				_inputDevice = InputManager.Devices[deviceIndex.Value];
			}
			float value = _inputDevice.GetControl(horizontalAxis).Value;
			float value2 = _inputDevice.GetControl(verticalAxis).Value;
			Vector3 value3 = value * vector2 + value2 * vector;
			value3 *= multiplier.Value;
			storeVector.Value = value3;
			if (!storeMagnitude.IsNone)
			{
				storeMagnitude.Value = value3.magnitude;
			}
		}
	}
}

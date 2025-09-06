using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
	[NoActionTargets]
	[ActionCategory(ActionCategory.Input)]
	[Tooltip("Gets a world direction Vector from 2 Input Axis. Typically used for a third person controller with Relative To set to the camera.")]
	[SeeAlso("Unity Input Manager")]
	public class GetAxisVector : FsmStateAction
	{
		public enum AxisPlane
		{
			XZ = 0,
			XY = 1,
			YZ = 2
		}

		[Tooltip("The name of the horizontal input axis. See Unity Input Manager.")]
		public FsmString horizontalAxis;

		[Tooltip("The name of the vertical input axis. See Unity Input Manager.")]
		public FsmString verticalAxis;

		[Tooltip("Normally axis values are in the range -1 to 1. Use the multiplier to make this range bigger. \nE.g., A multiplier of 100 returns values from -100 to 100.\nTypically this represents the maximum movement speed.")]
		public FsmFloat multiplier;

		[RequiredField]
		[Tooltip("Sets the world axis the input maps to. The remaining axis will be set to zero.")]
		public AxisPlane mapToPlane;

		[Tooltip("Calculate a vector relative to this game object. Typically the camera.")]
		public FsmGameObject relativeTo;

		[RequiredField]
		[UIHint(UIHint.Variable)]
		[Tooltip("Store the resulting vector. You can use this in {{Translate}} or other movement actions.")]
		public FsmVector3 storeVector;

		[UIHint(UIHint.Variable)]
		[Tooltip("Store the magnitude of the vector. Useful if you want to measure the strength of the input and react accordingly. Hint: Use {{Float Compare}}.")]
		public FsmFloat storeMagnitude;

		public override void Reset()
		{
			horizontalAxis = "Horizontal";
			verticalAxis = "Vertical";
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
			float num = ((horizontalAxis.IsNone || string.IsNullOrEmpty(horizontalAxis.Value)) ? 0f : Input.GetAxis(horizontalAxis.Value));
			float num2 = ((verticalAxis.IsNone || string.IsNullOrEmpty(verticalAxis.Value)) ? 0f : Input.GetAxis(verticalAxis.Value));
			Vector3 value = num * vector2 + num2 * vector;
			value *= multiplier.Value;
			storeVector.Value = value;
			if (!storeMagnitude.IsNone)
			{
				storeMagnitude.Value = value.magnitude;
			}
		}
	}
}

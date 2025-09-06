using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
	[ActionCategory(ActionCategory.Transform)]
	[Tooltip("Gets the Rotation of a Game Object and stores it in a Vector3 Variable or each Axis in a Float Variable")]
	public class GetRotation : FsmStateAction
	{
		[RequiredField]
		[Tooltip("The Game Object.")]
		public FsmOwnerDefault gameObject;

		[UIHint(UIHint.Variable)]
		[Tooltip("Get the rotation as a Quaternion.")]
		public FsmQuaternion quaternion;

		[UIHint(UIHint.Variable)]
		[Title("Euler Angles")]
		[Tooltip("Get the rotation as Euler angles (rotation around each axis) and store in a Vector3 Variable.")]
		public FsmVector3 vector;

		[UIHint(UIHint.Variable)]
		[Tooltip("Get the angle around the X axis.")]
		public FsmFloat xAngle;

		[UIHint(UIHint.Variable)]
		[Tooltip("Get the angle around the Y axis.")]
		public FsmFloat yAngle;

		[UIHint(UIHint.Variable)]
		[Tooltip("Get the angle around the Z axis.")]
		public FsmFloat zAngle;

		[Tooltip("The coordinate space to get the rotation in.")]
		public Space space;

		[Tooltip("Repeat every frame")]
		public bool everyFrame;

		public override void Reset()
		{
			gameObject = null;
			quaternion = null;
			vector = null;
			xAngle = null;
			yAngle = null;
			zAngle = null;
			space = Space.World;
			everyFrame = false;
		}

		public override void OnEnter()
		{
			DoGetRotation();
			if (!everyFrame)
			{
				Finish();
			}
		}

		public override void OnUpdate()
		{
			DoGetRotation();
		}

		private void DoGetRotation()
		{
			GameObject ownerDefaultTarget = base.Fsm.GetOwnerDefaultTarget(gameObject);
			if (!(ownerDefaultTarget == null))
			{
				if (space == Space.World)
				{
					quaternion.Value = ownerDefaultTarget.transform.rotation;
					Vector3 eulerAngles = ownerDefaultTarget.transform.eulerAngles;
					vector.Value = eulerAngles;
					xAngle.Value = eulerAngles.x;
					yAngle.Value = eulerAngles.y;
					zAngle.Value = eulerAngles.z;
				}
				else
				{
					Vector3 localEulerAngles = ownerDefaultTarget.transform.localEulerAngles;
					quaternion.Value = Quaternion.Euler(localEulerAngles);
					vector.Value = localEulerAngles;
					xAngle.Value = localEulerAngles.x;
					yAngle.Value = localEulerAngles.y;
					zAngle.Value = localEulerAngles.z;
				}
			}
		}
	}
}

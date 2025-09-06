using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
	[ActionCategory(ActionCategory.Transform)]
	[Tooltip("Clamps a rotation around a local axis. Optionally define the default rotation. Clamp is done on LateUpdate")]
	public class ClampRotation : FsmStateAction
	{
		public enum ConstraintAxis
		{
			x = 0,
			y = 1,
			z = 2
		}

		[RequiredField]
		[Tooltip("The GameObject to clamp rotation.")]
		public FsmOwnerDefault gameObject;

		[Tooltip("The default rotation. If none, will use the GameObject target.")]
		public FsmVector3 defaultRotation;

		[ObjectType(typeof(ConstraintAxis))]
		[Tooltip("The axis to constraint the rotation")]
		public FsmEnum constraintAxis;

		[Tooltip("The minimum angle allowed")]
		public FsmFloat minAngle;

		[Tooltip("The maximum angle allowed")]
		public FsmFloat maxAngle;

		[Tooltip("Repeat every frame")]
		public bool everyFrame;

		private float angleFromMin;

		private float angleFromMax;

		private Transform thisTransform;

		private Vector3 rotateAround;

		private Quaternion minQuaternion;

		private Quaternion maxQuaternion;

		private float range;

		private ConstraintAxis axis;

		private int axisIndex;

		private Quaternion axisRotation;

		private Vector3 _defaultRotation;

		public override void Reset()
		{
			gameObject = null;
			defaultRotation = new FsmVector3
			{
				UseVariable = true
			};
			constraintAxis = null;
			minAngle = -45f;
			maxAngle = 45f;
			everyFrame = false;
		}

		public override void OnPreprocess()
		{
			base.Fsm.HandleLateUpdate = true;
		}

		public override void OnEnter()
		{
			GameObject ownerDefaultTarget = base.Fsm.GetOwnerDefaultTarget(gameObject);
			if (!(ownerDefaultTarget == null))
			{
				thisTransform = ownerDefaultTarget.transform;
				axis = (ConstraintAxis)(object)constraintAxis.Value;
				axisIndex = (int)axis;
				switch (axis)
				{
				case ConstraintAxis.x:
					rotateAround = Vector3.right;
					break;
				case ConstraintAxis.y:
					rotateAround = Vector3.up;
					break;
				case ConstraintAxis.z:
					rotateAround = Vector3.forward;
					break;
				}
				_defaultRotation = ((!defaultRotation.IsNone) ? defaultRotation.Value : thisTransform.localRotation.eulerAngles);
				ComputeRange();
			}
		}

		public override void OnLateUpdate()
		{
			DoClampRotation();
			if (!everyFrame)
			{
				Finish();
			}
		}

		private void DoClampRotation()
		{
			GameObject ownerDefaultTarget = base.Fsm.GetOwnerDefaultTarget(gameObject);
			if (ownerDefaultTarget == null)
			{
				return;
			}
			thisTransform = ownerDefaultTarget.transform;
			if (!defaultRotation.IsNone && _defaultRotation != defaultRotation.Value)
			{
				_defaultRotation = defaultRotation.Value;
				ComputeRange();
			}
			axisRotation = Quaternion.AngleAxis(thisTransform.localRotation.eulerAngles[axisIndex], rotateAround);
			angleFromMin = Quaternion.Angle(axisRotation, minQuaternion);
			angleFromMax = Quaternion.Angle(axisRotation, maxQuaternion);
			if (!(angleFromMin <= range) || !(angleFromMax <= range))
			{
				Vector3 eulerAngles = thisTransform.localRotation.eulerAngles;
				if (angleFromMin > angleFromMax)
				{
					eulerAngles[axisIndex] = maxQuaternion.eulerAngles[axisIndex];
				}
				else
				{
					eulerAngles[axisIndex] = minQuaternion.eulerAngles[axisIndex];
				}
				thisTransform.localEulerAngles = eulerAngles;
			}
		}

		private void ComputeRange()
		{
			axisRotation = Quaternion.AngleAxis(defaultRotation.Value[axisIndex], rotateAround);
			minQuaternion = axisRotation * Quaternion.AngleAxis(minAngle.Value, rotateAround);
			maxQuaternion = axisRotation * Quaternion.AngleAxis(maxAngle.Value, rotateAround);
			range = maxAngle.Value - minAngle.Value;
		}
	}
}

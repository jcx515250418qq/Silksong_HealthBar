using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
	[ActionCategory(ActionCategory.Transform)]
	[Tooltip("Gets the Angle between a GameObject's forward axis and a Target. The Target can be defined as a GameObject or a world Position. If you specify both, then the Position will be used as a local offset from the Target Object's position.")]
	public class GetAngleToTarget : FsmStateAction
	{
		[RequiredField]
		[Tooltip("The game object whose forward axis we measure from. If the target is dead ahead the angle will be 0.")]
		public FsmOwnerDefault gameObject;

		[Tooltip("The target object to measure the angle to. Or use target position.")]
		public FsmGameObject targetObject;

		[Tooltip("The world position to measure an angle to. If Target Object is also specified, this vector is used as an offset from that object's position.")]
		public FsmVector3 targetPosition;

		[Tooltip("Ignore height differences when calculating the angle.")]
		public FsmBool ignoreHeight;

		[RequiredField]
		[UIHint(UIHint.Variable)]
		[Tooltip("Store the angle in a float variable.")]
		public FsmFloat storeAngle;

		[Tooltip("Repeat every frame.")]
		public bool everyFrame;

		public override void Reset()
		{
			gameObject = null;
			targetObject = null;
			targetPosition = new FsmVector3
			{
				UseVariable = true
			};
			ignoreHeight = true;
			storeAngle = null;
			everyFrame = false;
		}

		public override void OnPreprocess()
		{
			base.Fsm.HandleLateUpdate = true;
		}

		public override void OnLateUpdate()
		{
			DoGetAngleToTarget();
			if (!everyFrame)
			{
				Finish();
			}
		}

		private void DoGetAngleToTarget()
		{
			GameObject ownerDefaultTarget = base.Fsm.GetOwnerDefaultTarget(gameObject);
			if (ownerDefaultTarget == null)
			{
				return;
			}
			GameObject value = targetObject.Value;
			if (!(value == null) || !targetPosition.IsNone)
			{
				Vector3 vector = ((!(value != null)) ? targetPosition.Value : ((!targetPosition.IsNone) ? value.transform.TransformPoint(targetPosition.Value) : value.transform.position));
				if (ignoreHeight.Value)
				{
					vector.y = ownerDefaultTarget.transform.position.y;
				}
				Vector3 from = vector - ownerDefaultTarget.transform.position;
				storeAngle.Value = Vector3.Angle(from, ownerDefaultTarget.transform.forward);
			}
		}
	}
}

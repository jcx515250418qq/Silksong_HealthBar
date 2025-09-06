using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
	[ActionCategory(ActionCategory.Transform)]
	[Tooltip("Smoothly Rotates a 2d Game Object so its right vector points at a Target. The target can be defined as a 2d Game Object or a 2d/3d world Position. If you specify both, then the position will be used as a local offset from the object's position.")]
	public class SmoothLookAt2d : FsmStateAction
	{
		[RequiredField]
		[Tooltip("The GameObject to rotate to face a target.")]
		public FsmOwnerDefault gameObject;

		[Tooltip("A target GameObject.")]
		public FsmGameObject targetObject;

		[Tooltip("A target position. If a Target Object is defined, this is used as a local offset.")]
		public FsmVector2 targetPosition2d;

		[Tooltip("A target position. If a Target Object is defined, this is used as a local offset.")]
		public FsmVector3 targetPosition;

		[Tooltip("Set the GameObject starting offset. In degrees. 0 if your object is facing right, 180 if facing left etc...")]
		public FsmFloat rotationOffset;

		[HasFloatSlider(0.5f, 15f)]
		[Tooltip("How fast the look at moves.")]
		public FsmFloat speed;

		[Tooltip("Draw a line in the Scene View to the look at position.")]
		public FsmBool debug;

		[Tooltip("If the angle to the target is less than this, send the Finish Event below. Measured in degrees.")]
		public FsmFloat finishTolerance;

		[Tooltip("Event to send if the angle to target is less than the Finish Tolerance.")]
		public FsmEvent finishEvent;

		private GameObject previousGo;

		private Quaternion lastRotation;

		private Quaternion desiredRotation;

		public override void Reset()
		{
			gameObject = null;
			targetObject = null;
			targetPosition2d = new FsmVector2
			{
				UseVariable = true
			};
			targetPosition = new FsmVector3
			{
				UseVariable = true
			};
			rotationOffset = 0f;
			debug = false;
			speed = 5f;
			finishTolerance = 1f;
			finishEvent = null;
		}

		public override void OnPreprocess()
		{
			base.Fsm.HandleLateUpdate = true;
		}

		public override void OnEnter()
		{
			previousGo = null;
		}

		public override void OnLateUpdate()
		{
			DoSmoothLookAt();
		}

		private void DoSmoothLookAt()
		{
			GameObject ownerDefaultTarget = base.Fsm.GetOwnerDefaultTarget(gameObject);
			if (ownerDefaultTarget == null)
			{
				return;
			}
			GameObject value = targetObject.Value;
			if (previousGo != ownerDefaultTarget)
			{
				lastRotation = ownerDefaultTarget.transform.rotation;
				desiredRotation = lastRotation;
				previousGo = ownerDefaultTarget;
			}
			Vector3 vector = new Vector3(targetPosition2d.Value.x, targetPosition2d.Value.y, 0f);
			if (!targetPosition.IsNone)
			{
				vector += targetPosition.Value;
			}
			if (value != null)
			{
				vector = value.transform.position;
				Vector3 zero = Vector3.zero;
				if (!targetPosition.IsNone)
				{
					zero += targetPosition.Value;
				}
				if (!targetPosition2d.IsNone)
				{
					zero.x += targetPosition2d.Value.x;
					zero.y += targetPosition2d.Value.y;
				}
				if (!targetPosition2d.IsNone || !targetPosition.IsNone)
				{
					vector += value.transform.TransformPoint(targetPosition2d.Value);
				}
			}
			Vector3 vector2 = vector - ownerDefaultTarget.transform.position;
			vector2.Normalize();
			float num = Mathf.Atan2(vector2.y, vector2.x) * 57.29578f;
			desiredRotation = Quaternion.Euler(0f, 0f, num - rotationOffset.Value);
			lastRotation = Quaternion.Slerp(lastRotation, desiredRotation, speed.Value * Time.deltaTime);
			ownerDefaultTarget.transform.rotation = lastRotation;
			if (debug.Value)
			{
				Debug.DrawLine(ownerDefaultTarget.transform.position, vector, Color.grey);
			}
			if (finishEvent != null && Mathf.Abs(Vector3.Angle(desiredRotation.eulerAngles, lastRotation.eulerAngles)) <= finishTolerance.Value)
			{
				base.Fsm.Event(finishEvent);
			}
		}
	}
}

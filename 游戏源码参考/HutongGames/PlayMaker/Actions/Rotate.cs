using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
	[ActionCategory(ActionCategory.Transform)]
	[Tooltip("Rotates a Game Object around each Axis. Use a Vector3 Variable and/or XYZ components. To leave any axis unchanged, set variable to 'None'.")]
	public class Rotate : FsmStateAction
	{
		[RequiredField]
		[Tooltip("The game object to rotate.")]
		public FsmOwnerDefault gameObject;

		[Tooltip("A rotation vector specifying rotation around x, y, and z axis. NOTE: You can override individual axis below.")]
		[UIHint(UIHint.Variable)]
		public FsmVector3 vector;

		[Tooltip("Rotation around x axis.")]
		public FsmFloat xAngle;

		[Tooltip("Rotation around y axis.")]
		public FsmFloat yAngle;

		[Tooltip("Rotation around z axis.")]
		public FsmFloat zAngle;

		[Tooltip("Rotate in local or world space.")]
		public Space space;

		[Tooltip("Rotation is specified in degrees per second. In other words, the amount to rotate in over one second. This allows rotations to be frame rate independent. It is the same as multiplying the rotation by Time.deltaTime.")]
		public bool perSecond;

		[Tooltip("Repeat every frame.")]
		public bool everyFrame;

		[Tooltip("Perform the rotation in LateUpdate. This is useful if you want to override the rotation of objects that are animated or otherwise rotated in Update.")]
		public bool lateUpdate;

		[Tooltip("Perform the rotation in FixedUpdate. This is useful when working with rigid bodies and physics.")]
		public bool fixedUpdate;

		public override void Reset()
		{
			gameObject = null;
			vector = null;
			xAngle = new FsmFloat
			{
				UseVariable = true
			};
			yAngle = new FsmFloat
			{
				UseVariable = true
			};
			zAngle = new FsmFloat
			{
				UseVariable = true
			};
			space = Space.Self;
			perSecond = false;
			everyFrame = true;
			lateUpdate = false;
			fixedUpdate = false;
		}

		public override void OnPreprocess()
		{
			if (fixedUpdate)
			{
				base.Fsm.HandleFixedUpdate = true;
			}
			if (lateUpdate)
			{
				base.Fsm.HandleLateUpdate = true;
			}
		}

		public override void OnEnter()
		{
			if (!everyFrame && !lateUpdate && !fixedUpdate)
			{
				DoRotate();
				Finish();
			}
		}

		public override void OnUpdate()
		{
			if (!lateUpdate && !fixedUpdate)
			{
				DoRotate();
			}
		}

		public override void OnLateUpdate()
		{
			if (lateUpdate)
			{
				DoRotate();
			}
			if (!everyFrame)
			{
				Finish();
			}
		}

		public override void OnFixedUpdate()
		{
			if (fixedUpdate)
			{
				DoRotate();
			}
			if (!everyFrame)
			{
				Finish();
			}
		}

		private void DoRotate()
		{
			GameObject ownerDefaultTarget = base.Fsm.GetOwnerDefaultTarget(gameObject);
			if (!(ownerDefaultTarget == null))
			{
				Vector3 vector = (this.vector.IsNone ? new Vector3(xAngle.Value, yAngle.Value, zAngle.Value) : this.vector.Value);
				if (!xAngle.IsNone)
				{
					vector.x = xAngle.Value;
				}
				if (!yAngle.IsNone)
				{
					vector.y = yAngle.Value;
				}
				if (!zAngle.IsNone)
				{
					vector.z = zAngle.Value;
				}
				if (!perSecond)
				{
					ownerDefaultTarget.transform.Rotate(vector, space);
				}
				else
				{
					ownerDefaultTarget.transform.Rotate(vector * Time.deltaTime, space);
				}
			}
		}
	}
}

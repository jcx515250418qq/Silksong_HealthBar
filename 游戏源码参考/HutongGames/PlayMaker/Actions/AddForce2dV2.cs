using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
	[ActionCategory("Physics 2d")]
	[Tooltip("Adds a 2d force to a Game Object. Use Vector2 variable and/or Float variables for each axis. I added the ability to limit speed.")]
	public class AddForce2dV2 : RigidBody2dActionBase
	{
		[RequiredField]
		[CheckForComponent(typeof(Rigidbody2D))]
		[Tooltip("The GameObject to apply the force to.")]
		public FsmOwnerDefault gameObject;

		[UIHint(UIHint.Variable)]
		[Tooltip("Optionally apply the force at a position on the object. This will also add some torque. The position is often returned from MousePick or GetCollision2dInfo actions.")]
		public FsmVector2 atPosition;

		[UIHint(UIHint.Variable)]
		[Tooltip("A Vector2 force to add. Optionally override any axis with the X, Y parameters.")]
		public FsmVector2 vector;

		[Tooltip("Force along the X axis. To leave unchanged, set to 'None'.")]
		public FsmFloat x;

		[Tooltip("Force along the Y axis. To leave unchanged, set to 'None'.")]
		public FsmFloat y;

		[Tooltip("A Vector3 force to add. z is ignored")]
		public FsmVector3 vector3;

		public FsmFloat maxSpeed;

		public FsmFloat maxSpeedX;

		public FsmFloat maxSpeedY;

		[Tooltip("Repeat every frame while the state is active.")]
		public bool everyFrame;

		public override void Reset()
		{
			gameObject = null;
			atPosition = new FsmVector2
			{
				UseVariable = true
			};
			vector = null;
			vector3 = new FsmVector3
			{
				UseVariable = true
			};
			x = new FsmFloat
			{
				UseVariable = true
			};
			y = new FsmFloat
			{
				UseVariable = true
			};
			maxSpeed = null;
			maxSpeedX = null;
			maxSpeedY = null;
			everyFrame = false;
		}

		public override void OnPreprocess()
		{
			base.OnPreprocess();
			base.Fsm.HandleFixedUpdate = true;
		}

		public override void OnEnter()
		{
			CacheRigidBody2d(base.Fsm.GetOwnerDefaultTarget(gameObject));
			DoAddForce();
			if (!everyFrame)
			{
				Finish();
			}
		}

		public override void OnFixedUpdate()
		{
			DoAddForce();
		}

		private void DoAddForce()
		{
			if (!rb2d)
			{
				return;
			}
			Vector2 force = (vector.IsNone ? new Vector2(x.Value, y.Value) : vector.Value);
			if (!vector3.IsNone)
			{
				force.x = vector3.Value.x;
				force.y = vector3.Value.y;
			}
			if (!x.IsNone)
			{
				force.x = x.Value;
			}
			if (!y.IsNone)
			{
				force.y = y.Value;
			}
			if (!atPosition.IsNone)
			{
				rb2d.AddForceAtPosition(force, atPosition.Value);
			}
			else
			{
				rb2d.AddForce(force);
			}
			if (maxSpeedX != null)
			{
				Vector2 linearVelocity = rb2d.linearVelocity;
				if (linearVelocity.x > maxSpeedX.Value)
				{
					linearVelocity = new Vector2(maxSpeedX.Value, linearVelocity.y);
				}
				if (linearVelocity.x < 0f - maxSpeedX.Value)
				{
					linearVelocity = new Vector2(0f - maxSpeedX.Value, linearVelocity.y);
				}
				rb2d.linearVelocity = linearVelocity;
			}
			if (maxSpeedY != null)
			{
				Vector2 linearVelocity2 = rb2d.linearVelocity;
				if (linearVelocity2.y > maxSpeedY.Value)
				{
					linearVelocity2 = new Vector2(linearVelocity2.x, maxSpeedY.Value);
				}
				if (linearVelocity2.y < 0f - maxSpeedY.Value)
				{
					linearVelocity2 = new Vector2(linearVelocity2.x, 0f - maxSpeedY.Value);
				}
				rb2d.linearVelocity = linearVelocity2;
			}
			if (maxSpeed != null)
			{
				Vector2 linearVelocity3 = rb2d.linearVelocity;
				linearVelocity3 = Vector2.ClampMagnitude(linearVelocity3, maxSpeed.Value);
				rb2d.linearVelocity = linearVelocity3;
			}
		}
	}
}

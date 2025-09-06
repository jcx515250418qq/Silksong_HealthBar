using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
	[ActionCategory(ActionCategory.Character)]
	[Tooltip("Makes a CharacterController Jump.")]
	public class ControllerJump : ComponentAction<CharacterController>
	{
		[RequiredField]
		[CheckForComponent(typeof(CharacterController))]
		[Tooltip("The GameObject that owns the CharacterController component.")]
		public FsmOwnerDefault gameObject;

		[Tooltip("How high to jump.")]
		public FsmFloat jumpHeight;

		[Tooltip("Jump in local or word space.")]
		public Space space;

		[Tooltip("Multiplies the speed of the CharacterController at moment of jumping. Higher numbers will jump further. Note: Does not effect the jump height.")]
		public FsmFloat jumpSpeedMultiplier;

		[Tooltip("Gravity multiplier used in air, to correctly calculate jump height.")]
		public FsmFloat gravityMultiplier;

		[Tooltip("Extra gravity multiplier when falling. Note: This is on top of the gravity multiplier above. This can be used to make jumps less 'floaty.'")]
		public FsmFloat fallMultiplier;

		[ActionSection("In Air Controls")]
		[UIHint(UIHint.Variable)]
		[Tooltip("Movement vector applied while in the air. Usually from a Get Axis Vector, allowing the player to influence the jump.")]
		public FsmVector3 moveVector;

		[Tooltip("Multiplies the Move Vector by a Speed factor.")]
		public FsmFloat speed;

		[Tooltip("Clamp horizontal speed while jumping. Set to None for no clamping.")]
		public FsmFloat maxSpeed;

		[ActionSection("Landing")]
		[Tooltip("Event to send when landing. Use this to transition back to a grounded State.")]
		public FsmEvent landedEvent;

		[UIHint(UIHint.Variable)]
		[Tooltip("Store how fast the Character Controlling was moving when it landed.")]
		public FsmFloat landingSpeed;

		[UIHint(UIHint.Variable)]
		[Tooltip("Store the last movement before landing.")]
		public FsmVector3 landingMotion;

		[UIHint(UIHint.Variable)]
		[Tooltip("The total distance fallen, from the start of the jump to landing point. NOTE: This will be negative when jumping to higher ground.")]
		public FsmFloat fallDistance;

		private Vector3 startJumpPosition;

		private Vector3 totalJumpMovement;

		private CharacterController controller => cachedComponent;

		public override void Reset()
		{
			gameObject = null;
			jumpHeight = new FsmFloat
			{
				Value = 0.5f
			};
			jumpSpeedMultiplier = new FsmFloat
			{
				Value = 1f
			};
			gravityMultiplier = new FsmFloat
			{
				Value = 1f
			};
			space = Space.World;
			moveVector = null;
			speed = new FsmFloat
			{
				Value = 1f
			};
			maxSpeed = new FsmFloat
			{
				Value = 2f
			};
			fallMultiplier = new FsmFloat
			{
				Value = 1f
			};
		}

		public override void OnEnter()
		{
			if (!UpdateCacheAndTransform(base.Fsm.GetOwnerDefaultTarget(gameObject)))
			{
				Finish();
				return;
			}
			startJumpPosition = base.cachedTransform.position;
			Vector3 direction = controller.velocity * (jumpSpeedMultiplier.IsNone ? 1f : jumpSpeedMultiplier.Value);
			direction.y = 0f;
			if (space == Space.Self)
			{
				direction = base.cachedTransform.InverseTransformDirection(direction);
			}
			float num = Physics.gravity.y * (gravityMultiplier.IsNone ? 1f : gravityMultiplier.Value);
			float y = direction.y + Mathf.Sqrt(jumpHeight.Value * -3f * num);
			Vector3 vector = new Vector3(direction.x, y, direction.z);
			if (space == Space.Self)
			{
				vector = base.cachedTransform.TransformDirection(vector);
			}
			controller.Move(vector * Time.deltaTime);
		}

		public override void OnUpdate()
		{
			if (!UpdateCacheAndTransform(base.Fsm.GetOwnerDefaultTarget(gameObject)))
			{
				Finish();
				return;
			}
			Vector3 vector = controller.velocity;
			if (!moveVector.IsNone)
			{
				Vector3 value = moveVector.Value;
				if (!speed.IsNone)
				{
					value *= speed.Value;
				}
				vector += value;
			}
			float num = Physics.gravity.y * gravityMultiplier.Value * ((vector.y < 0f) ? fallMultiplier.Value : 1f);
			vector.y += num * Time.deltaTime;
			if (!maxSpeed.IsNone)
			{
				Vector2 vector2 = Vector2.ClampMagnitude(new Vector2(vector.x, vector.z), maxSpeed.Value);
				vector.Set(vector2.x, vector.y, vector2.y);
			}
			if (space == Space.Self)
			{
				vector = base.cachedTransform.TransformDirection(vector);
			}
			controller.Move(vector * Time.deltaTime);
			if (controller.isGrounded && controller.velocity.y < 0.1f)
			{
				controller.Move(Vector3.zero);
				landingMotion.Value = vector;
				landingSpeed.Value = vector.magnitude;
				fallDistance.Value = startJumpPosition.y - base.cachedTransform.position.y;
				base.Fsm.Event(landedEvent);
			}
		}
	}
}

using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
	[ActionCategory("Hornet")]
	[Tooltip("Controls Hornet's horizontal motion in umbrella float")]
	public class UmbrellaMotionX : RigidBody2dActionBase
	{
		[RequiredField]
		[CheckForComponent(typeof(Rigidbody2D))]
		public FsmOwnerDefault gameObject;

		public FsmFloat accelerationFactor;

		public FsmFloat decelerationFactor;

		public FsmFloat maxSpeed;

		private GameManager gm;

		private InputHandler inputHandler;

		private UmbrellaWindObject windObject;

		public override void Reset()
		{
			gameObject = null;
			accelerationFactor = null;
			accelerationFactor = null;
			decelerationFactor = null;
		}

		public override void OnPreprocess()
		{
			base.Fsm.HandleFixedUpdate = true;
		}

		public override void OnEnter()
		{
			gm = GameManager.instance;
			inputHandler = gm.GetComponent<InputHandler>();
			GameObject ownerDefaultTarget = base.Fsm.GetOwnerDefaultTarget(gameObject);
			CacheRigidBody2d(ownerDefaultTarget);
			windObject = ownerDefaultTarget.GetComponent<UmbrellaWindObject>();
			if ((bool)windObject)
			{
				windObject.IsActive = true;
				windObject.SelfXSpeed = rb2d.linearVelocity.x;
			}
			DoAccelerate();
		}

		public override void OnFixedUpdate()
		{
			DoAccelerate();
		}

		public override void OnExit()
		{
			if ((bool)windObject)
			{
				windObject.IsActive = false;
				windObject = null;
			}
		}

		private void DoAccelerate()
		{
			if (rb2d == null)
			{
				return;
			}
			float num = (windObject ? windObject.SelfXSpeed : rb2d.linearVelocity.x);
			if (inputHandler.inputActions.Right.IsPressed)
			{
				if (num < maxSpeed.Value)
				{
					num += accelerationFactor.Value;
				}
			}
			else if (inputHandler.inputActions.Left.IsPressed)
			{
				if (num > 0f - maxSpeed.Value)
				{
					num -= accelerationFactor.Value;
				}
			}
			else if (num > 0f)
			{
				num -= decelerationFactor.Value;
				if (num < 0f)
				{
					num = 0f;
				}
			}
			else if (num < 0f)
			{
				num += decelerationFactor.Value;
				if (num > 0f)
				{
					num = 0f;
				}
			}
			if (num < 0f - maxSpeed.Value)
			{
				num = 0f - maxSpeed.Value;
			}
			if (num > maxSpeed.Value)
			{
				num = maxSpeed.Value;
			}
			if ((bool)windObject)
			{
				windObject.SelfXSpeed = num;
			}
			else
			{
				rb2d.linearVelocity = new Vector2(num, rb2d.linearVelocity.y);
			}
		}
	}
}

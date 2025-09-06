using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
	[ActionCategory("Enemy AI")]
	[Tooltip("Object moves more directly toward target")]
	public class ChaseObjectV2 : RigidBody2dActionBase
	{
		[RequiredField]
		[CheckForComponent(typeof(Rigidbody2D))]
		[UIHint(UIHint.Variable)]
		public FsmOwnerDefault gameObject;

		[UIHint(UIHint.Variable)]
		public FsmGameObject target;

		public FsmFloat speedMax;

		public FsmFloat accelerationForce;

		public FsmFloat offsetX;

		public FsmFloat offsetY;

		private FsmGameObject self;

		public override void Reset()
		{
			gameObject = null;
			target = null;
			accelerationForce = 0f;
			speedMax = 0f;
			offsetX = 0f;
			offsetY = 0f;
		}

		public override void Awake()
		{
			base.Fsm.HandleFixedUpdate = true;
		}

		public override void OnPreprocess()
		{
			base.Fsm.HandleFixedUpdate = true;
		}

		public override void OnEnter()
		{
			CacheRigidBody2d(base.Fsm.GetOwnerDefaultTarget(gameObject));
			self = base.Fsm.GetOwnerDefaultTarget(gameObject);
			DoChase();
		}

		public override void OnFixedUpdate()
		{
			DoChase();
		}

		private void DoChase()
		{
			if (!(rb2d == null))
			{
				Vector2 vector = new Vector2(target.Value.transform.position.x + offsetX.Value - self.Value.transform.position.x, target.Value.transform.position.y + offsetY.Value - self.Value.transform.position.y);
				vector = Vector2.ClampMagnitude(vector, 1f);
				vector = new Vector2(vector.x * accelerationForce.Value, vector.y * accelerationForce.Value);
				rb2d.AddForce(vector);
				Vector2 linearVelocity = rb2d.linearVelocity;
				linearVelocity = Vector2.ClampMagnitude(linearVelocity, speedMax.Value);
				rb2d.linearVelocity = linearVelocity;
			}
		}
	}
}

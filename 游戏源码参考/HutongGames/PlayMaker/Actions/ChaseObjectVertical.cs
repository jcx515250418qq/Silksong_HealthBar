using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
	[ActionCategory("Enemy AI")]
	[Tooltip("Object chases target on Y axis")]
	public class ChaseObjectVertical : RigidBody2dActionBase
	{
		[RequiredField]
		[CheckForComponent(typeof(Rigidbody2D))]
		[UIHint(UIHint.Variable)]
		public FsmOwnerDefault gameObject;

		[UIHint(UIHint.Variable)]
		public FsmGameObject target;

		public FsmFloat offset;

		public FsmFloat speedMax;

		public FsmFloat acceleration;

		private FsmGameObject self;

		private bool turning;

		private float offsetValue;

		public override void Reset()
		{
			gameObject = null;
			target = null;
			offset = null;
			acceleration = 0f;
			speedMax = 0f;
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
			offsetValue = 0f;
			if (!offset.IsNone)
			{
				offsetValue = offset.Value;
			}
			DoChase();
		}

		public override void OnFixedUpdate()
		{
			DoChase();
		}

		private void DoChase()
		{
			if (rb2d == null)
			{
				return;
			}
			Vector2 linearVelocity = rb2d.linearVelocity;
			if (self.Value.transform.position.y < target.Value.transform.position.y + offsetValue || self.Value.transform.position.y > target.Value.transform.position.y + offsetValue)
			{
				if (self.Value.transform.position.y < target.Value.transform.position.y + offsetValue)
				{
					linearVelocity.y += acceleration.Value;
				}
				else
				{
					linearVelocity.y -= acceleration.Value;
				}
				if (linearVelocity.y > speedMax.Value)
				{
					linearVelocity.y = speedMax.Value;
				}
				if (linearVelocity.y < 0f - speedMax.Value)
				{
					linearVelocity.y = 0f - speedMax.Value;
				}
				rb2d.linearVelocity = linearVelocity;
			}
		}
	}
}

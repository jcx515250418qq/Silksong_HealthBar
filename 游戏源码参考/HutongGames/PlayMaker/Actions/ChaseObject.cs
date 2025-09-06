using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
	[ActionCategory("Enemy AI")]
	[Tooltip("Object buzzes towards target")]
	public class ChaseObject : RigidBody2dActionBase
	{
		[RequiredField]
		[CheckForComponent(typeof(Rigidbody2D))]
		[UIHint(UIHint.Variable)]
		public FsmOwnerDefault gameObject;

		[UIHint(UIHint.Variable)]
		public FsmGameObject target;

		public FsmFloat speedMax;

		public FsmFloat acceleration;

		public FsmFloat targetSpread;

		public FsmFloat spreadResetTimeMin;

		public FsmFloat spreadResetTimeMax;

		private bool spreadSet;

		private float spreadResetTime;

		private float spreadX;

		private float spreadY;

		public FsmFloat offsetX;

		public FsmFloat offsetY;

		private FsmGameObject self;

		private float timer;

		private float spreadResetTimer;

		public override void Reset()
		{
			gameObject = null;
			target = null;
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
			DoBuzz();
		}

		public override void OnFixedUpdate()
		{
			DoBuzz();
		}

		private void DoBuzz()
		{
			if (rb2d == null)
			{
				return;
			}
			if (targetSpread.Value > 0f)
			{
				if (timer >= spreadResetTime)
				{
					spreadX = Random.Range(0f - targetSpread.Value, targetSpread.Value);
					spreadY = Random.Range(0f - targetSpread.Value, targetSpread.Value);
					timer = 0f;
					spreadResetTime = Random.Range(spreadResetTimeMin.Value, spreadResetTimeMax.Value);
				}
				else
				{
					timer += Time.deltaTime;
				}
			}
			Vector2 linearVelocity = rb2d.linearVelocity;
			if (self.Value.transform.position.x < target.Value.transform.position.x + spreadX + offsetX.Value)
			{
				linearVelocity.x += acceleration.Value;
			}
			else
			{
				linearVelocity.x -= acceleration.Value;
			}
			if (self.Value.transform.position.y < target.Value.transform.position.y + spreadY + offsetY.Value)
			{
				linearVelocity.y += acceleration.Value;
			}
			else
			{
				linearVelocity.y -= acceleration.Value;
			}
			if (linearVelocity.x > speedMax.Value)
			{
				linearVelocity.x = speedMax.Value;
			}
			if (linearVelocity.x < 0f - speedMax.Value)
			{
				linearVelocity.x = 0f - speedMax.Value;
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

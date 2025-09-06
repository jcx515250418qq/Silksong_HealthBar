using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
	[ActionCategory("Enemy AI")]
	public class SilkBossIdleBuzz : RigidBody2dActionBase
	{
		[RequiredField]
		[CheckForComponent(typeof(Rigidbody2D))]
		public FsmOwnerDefault gameObject;

		public FsmFloat targetY;

		public FsmFloat targetRange;

		public FsmFloat acceleration;

		public FsmFloat speedMax;

		public bool fuzzTargetRange;

		private float targetRangeInit;

		private bool movingDown;

		private Transform tf;

		private Rigidbody2D rb;

		public override void Reset()
		{
			gameObject = null;
		}

		public override void OnEnter()
		{
			GameObject ownerDefaultTarget = base.Fsm.GetOwnerDefaultTarget(gameObject);
			if (!(ownerDefaultTarget == null))
			{
				tf = ownerDefaultTarget.GetComponent<Transform>();
				rb = ownerDefaultTarget.GetComponent<Rigidbody2D>();
				targetRangeInit = targetRange.Value;
				DoBuzz();
			}
		}

		public override void OnUpdate()
		{
			DoBuzz();
		}

		private void DoBuzz()
		{
			Vector2 linearVelocity = rb.linearVelocity;
			if (tf.position.y > targetY.Value + targetRange.Value)
			{
				linearVelocity = new Vector2(linearVelocity.x, linearVelocity.y - acceleration.Value * Time.deltaTime);
				if (!movingDown)
				{
					if (fuzzTargetRange)
					{
						targetRange = targetRangeInit * Random.Range(0.75f, 1.25f);
					}
					movingDown = true;
				}
			}
			else if (!(tf.position.y < targetY.Value - targetRange.Value))
			{
				linearVelocity = ((!movingDown) ? new Vector2(linearVelocity.x, linearVelocity.y + acceleration.Value * Time.deltaTime) : new Vector2(linearVelocity.x, linearVelocity.y - acceleration.Value * Time.deltaTime));
			}
			else
			{
				linearVelocity = new Vector2(linearVelocity.x, linearVelocity.y + acceleration.Value * Time.deltaTime);
				if (movingDown)
				{
					if (fuzzTargetRange)
					{
						targetRange = targetRangeInit * Random.Range(0.75f, 1.25f);
					}
					movingDown = false;
				}
			}
			if (linearVelocity.y > speedMax.Value)
			{
				linearVelocity = new Vector2(linearVelocity.x, speedMax.Value);
			}
			if (linearVelocity.y < 0f - speedMax.Value)
			{
				linearVelocity = new Vector2(linearVelocity.x, 0f - speedMax.Value);
			}
			rb.linearVelocity = linearVelocity;
		}
	}
}

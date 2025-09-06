using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
	[ActionCategory("Enemy AI")]
	[Tooltip("Object idly buzzes about within a defined range")]
	public class IdleBuzz : RigidBody2dActionBase
	{
		[RequiredField]
		[CheckForComponent(typeof(Rigidbody2D))]
		public FsmOwnerDefault gameObject;

		public FsmFloat waitMin;

		public FsmFloat waitMax;

		public FsmFloat speedMax;

		public FsmFloat accelerationMax;

		public FsmFloat roamingRange;

		private FsmGameObject target;

		private float startX;

		private float startY;

		private float accelX;

		private float accelY;

		private float waitTime;

		private const float dampener = 1.125f;

		public override void Reset()
		{
			gameObject = null;
			waitMin = 0f;
			waitMax = 0f;
			accelerationMax = 0f;
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
			target = base.Fsm.GetOwnerDefaultTarget(gameObject);
			startX = target.Value.transform.position.x;
			startY = target.Value.transform.position.y;
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
			Vector2 linearVelocity = rb2d.linearVelocity;
			if (target.Value.transform.position.y < startY - roamingRange.Value)
			{
				if (linearVelocity.y < 0f)
				{
					accelY = accelerationMax.Value;
					accelY /= 2000f;
					linearVelocity.y /= 1.125f;
					waitTime = Random.Range(waitMin.Value, waitMax.Value);
				}
			}
			else if (target.Value.transform.position.y > startY + roamingRange.Value && linearVelocity.y > 0f)
			{
				accelY = 0f - accelerationMax.Value;
				accelY /= 2000f;
				linearVelocity.y /= 1.125f;
				waitTime = Random.Range(waitMin.Value, waitMax.Value);
			}
			if (target.Value.transform.position.x < startX - roamingRange.Value)
			{
				if (linearVelocity.x < 0f)
				{
					accelX = accelerationMax.Value;
					accelX /= 2000f;
					linearVelocity.x /= 1.125f;
					waitTime = Random.Range(waitMin.Value, waitMax.Value);
				}
			}
			else if (target.Value.transform.position.x > startX + roamingRange.Value && linearVelocity.x > 0f)
			{
				accelX = 0f - accelerationMax.Value;
				accelX /= 2000f;
				linearVelocity.x /= 1.125f;
				waitTime = Random.Range(waitMin.Value, waitMax.Value);
			}
			if (waitTime <= Mathf.Epsilon)
			{
				if (target.Value.transform.position.y < startY - roamingRange.Value)
				{
					accelY = Random.Range(0f, accelerationMax.Value);
				}
				else if (target.Value.transform.position.y > startY + roamingRange.Value)
				{
					accelY = Random.Range(0f - accelerationMax.Value, 0f);
				}
				else
				{
					accelY = Random.Range(0f - accelerationMax.Value, accelerationMax.Value);
				}
				if (target.Value.transform.position.x < startX - roamingRange.Value)
				{
					accelX = Random.Range(0f, accelerationMax.Value);
				}
				else if (target.Value.transform.position.x > startX + roamingRange.Value)
				{
					accelX = Random.Range(0f - accelerationMax.Value, 0f);
				}
				else
				{
					accelX = Random.Range(0f - accelerationMax.Value, accelerationMax.Value);
				}
				accelY /= 2000f;
				accelX /= 2000f;
				waitTime = Random.Range(waitMin.Value, waitMax.Value);
			}
			if (waitTime > Mathf.Epsilon)
			{
				waitTime -= Time.deltaTime;
			}
			linearVelocity.x += accelX;
			linearVelocity.y += accelY;
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

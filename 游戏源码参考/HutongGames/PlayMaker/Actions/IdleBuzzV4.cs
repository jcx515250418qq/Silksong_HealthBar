using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
	[ActionCategory("Enemy AI")]
	[Tooltip("Object idly buzzes about within a defined range")]
	public class IdleBuzzV4 : RigidBody2dActionBase
	{
		[RequiredField]
		[CheckForComponent(typeof(Rigidbody2D))]
		public FsmOwnerDefault gameObject;

		public FsmFloat waitMin;

		public FsmFloat waitMax;

		public FsmFloat speedMax;

		public FsmFloat deceleration;

		public FsmFloat accelerationMin;

		public FsmFloat accelerationMax;

		public FsmFloat roamingRangeX;

		public FsmFloat roamingRangeY;

		public FsmVector3 manualStartPos;

		private FsmGameObject target;

		private float startX;

		private float startY;

		private float accelX;

		private float accelY;

		private float waitTime;

		private const float dampener = 1.025f;

		public override void Reset()
		{
			gameObject = null;
			waitMin = 0f;
			waitMax = 0f;
			accelerationMax = 0f;
			roamingRangeX = 0f;
			roamingRangeY = 0f;
			manualStartPos = new FsmVector3
			{
				UseVariable = true
			};
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
			if (!manualStartPos.IsNone)
			{
				startX = manualStartPos.Value.x;
				startY = manualStartPos.Value.y;
			}
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
			if (target.Value.transform.position.y < startY - roamingRangeY.Value)
			{
				if (linearVelocity.y < 0f)
				{
					accelY = accelerationMax.Value;
					accelY /= 2000f;
					linearVelocity.y /= 1.025f;
					waitTime = Random.Range(waitMin.Value, waitMax.Value);
				}
			}
			else if (target.Value.transform.position.y > startY + roamingRangeY.Value && linearVelocity.y > 0f)
			{
				accelY = 0f - accelerationMax.Value;
				accelY /= 2000f;
				linearVelocity.y /= 1.025f;
				waitTime = Random.Range(waitMin.Value, waitMax.Value);
			}
			if (target.Value.transform.position.x < startX - roamingRangeX.Value)
			{
				if (linearVelocity.x < 0f)
				{
					accelX = accelerationMax.Value;
					accelX /= 2000f;
					linearVelocity.x /= 1.025f;
					waitTime = Random.Range(waitMin.Value, waitMax.Value);
				}
			}
			else if (target.Value.transform.position.x > startX + roamingRangeX.Value && linearVelocity.x > 0f)
			{
				accelX = 0f - accelerationMax.Value;
				accelX /= 2000f;
				linearVelocity.x /= 1.025f;
				waitTime = Random.Range(waitMin.Value, waitMax.Value);
			}
			if (waitTime <= Mathf.Epsilon)
			{
				if (target.Value.transform.position.y < startY - roamingRangeY.Value)
				{
					accelY = Random.Range(accelerationMin.Value, accelerationMax.Value);
				}
				else if (target.Value.transform.position.y > startY + roamingRangeY.Value)
				{
					accelY = Random.Range(0f - accelerationMax.Value, accelerationMin.Value);
				}
				else
				{
					accelY = Random.Range(0f - accelerationMax.Value, accelerationMax.Value);
				}
				if (target.Value.transform.position.x < startX - roamingRangeX.Value)
				{
					accelX = Random.Range(accelerationMin.Value, accelerationMax.Value);
				}
				else if (target.Value.transform.position.x > startX + roamingRangeX.Value)
				{
					accelX = Random.Range(0f - accelerationMax.Value, accelerationMin.Value);
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
				linearVelocity.x *= deceleration.Value;
				if (linearVelocity.x < 0f)
				{
					linearVelocity.x = 0f;
				}
			}
			if (linearVelocity.x < 0f - speedMax.Value)
			{
				linearVelocity.x *= deceleration.Value;
				if (linearVelocity.x > 0f)
				{
					linearVelocity.x = 0f;
				}
			}
			if (linearVelocity.y > speedMax.Value)
			{
				linearVelocity.y *= deceleration.Value;
				if (linearVelocity.y < 0f)
				{
					linearVelocity.y = 0f;
				}
			}
			if (linearVelocity.y < 0f - speedMax.Value)
			{
				linearVelocity.y *= deceleration.Value;
				if (linearVelocity.y > 0f)
				{
					linearVelocity.y = 0f;
				}
			}
			rb2d.linearVelocity = linearVelocity;
		}
	}
}

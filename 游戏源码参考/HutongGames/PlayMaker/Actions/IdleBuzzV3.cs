using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
	[ActionCategory("Enemy AI")]
	[Tooltip("Object idly buzzes about within a defined range")]
	public class IdleBuzzV3 : RigidBody2dActionBase
	{
		[RequiredField]
		[CheckForComponent(typeof(Rigidbody2D))]
		public FsmOwnerDefault gameObject;

		public FsmFloat waitMin;

		public FsmFloat waitMax;

		public FsmFloat speedMax;

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

		private const float dampener = 1.125f;

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
			Vector3 position = target.Value.transform.position;
			float value = roamingRangeY.Value;
			float value2 = accelerationMax.Value;
			float value3 = waitMin.Value;
			float value4 = waitMax.Value;
			if (position.y < startY - value)
			{
				if (linearVelocity.y < 0f)
				{
					accelY = value2;
					accelY /= 2000f;
					linearVelocity.y /= 1.125f;
					waitTime = Random.Range(value3, value4);
				}
			}
			else if (position.y > startY + value && linearVelocity.y > 0f)
			{
				accelY = 0f - value2;
				accelY /= 2000f;
				linearVelocity.y /= 1.125f;
				waitTime = Random.Range(value3, value4);
			}
			float value5 = roamingRangeX.Value;
			if (position.x < startX - value5)
			{
				if (linearVelocity.x < 0f)
				{
					accelX = value2;
					accelX /= 2000f;
					linearVelocity.x /= 1.125f;
					waitTime = Random.Range(value3, value4);
				}
			}
			else if (position.x > startX + value5 && linearVelocity.x > 0f)
			{
				accelX = 0f - value2;
				accelX /= 2000f;
				linearVelocity.x /= 1.125f;
				waitTime = Random.Range(value3, value4);
			}
			if (waitTime <= Mathf.Epsilon)
			{
				float value6 = accelerationMin.Value;
				if (position.y < startY - value)
				{
					accelY = Random.Range(value6, value2);
				}
				else if (position.y > startY + value)
				{
					accelY = Random.Range(0f - value2, value6);
				}
				else
				{
					accelY = Random.Range(0f - value2, value2);
				}
				if (position.x < startX - value5)
				{
					accelX = Random.Range(value6, value2);
				}
				else if (position.x > startX + value5)
				{
					accelX = Random.Range(0f - value2, value6);
				}
				else
				{
					accelX = Random.Range(0f - value2, value2);
				}
				accelY /= 2000f;
				accelX /= 2000f;
				waitTime = Random.Range(value3, value4);
			}
			if (waitTime > Mathf.Epsilon)
			{
				waitTime -= Time.deltaTime;
			}
			linearVelocity.x += accelX;
			linearVelocity.y += accelY;
			float value7 = speedMax.Value;
			if (linearVelocity.x > value7)
			{
				linearVelocity.x = value7;
			}
			if (linearVelocity.x < 0f - value7)
			{
				linearVelocity.x = 0f - value7;
			}
			if (linearVelocity.y > value7)
			{
				linearVelocity.y = value7;
			}
			if (linearVelocity.y < 0f - value7)
			{
				linearVelocity.y = 0f - value7;
			}
			rb2d.linearVelocity = linearVelocity;
		}
	}
}

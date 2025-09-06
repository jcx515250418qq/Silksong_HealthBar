using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
	[ActionCategory(ActionCategory.Physics2D)]
	public class TiltBySpeedV2 : ComponentAction<Rigidbody2D>
	{
		[RequiredField]
		public FsmGameObject gameObjectToTilt;

		public FsmGameObject getSpeedFrom;

		public FsmFloat tiltFactor;

		public FsmFloat tiltMax;

		public FsmFloat rotationSpeed;

		private float targetAngle;

		private Transform target_transform;

		private Rigidbody2D rb;

		public override void Reset()
		{
			gameObjectToTilt = null;
			getSpeedFrom = null;
			tiltFactor = null;
			tiltMax = null;
		}

		public override void OnEnter()
		{
			target_transform = gameObjectToTilt.Value.GetComponent<Transform>();
			rb = getSpeedFrom.Value.GetComponent<Rigidbody2D>();
			DoTilt();
		}

		public override void OnUpdate()
		{
			DoTilt();
		}

		private void DoTilt()
		{
			float num = rb.linearVelocity.x * tiltFactor.Value;
			if (!tiltMax.IsNone)
			{
				if (num > tiltMax.Value)
				{
					num = tiltMax.Value;
				}
				if (num < 0f - tiltMax.Value)
				{
					num = 0f - tiltMax.Value;
				}
			}
			DoRotateTo(num);
		}

		private void DoRotateTo(float targetAngle)
		{
			float num = targetAngle - target_transform.localEulerAngles.z;
			if ((num < 0f) ? (num < -180f) : (!(num > 180f)))
			{
				target_transform.Rotate(0f, 0f, rotationSpeed.Value * Time.deltaTime);
				if (target_transform.localEulerAngles.z > targetAngle)
				{
					target_transform.localEulerAngles = new Vector3(target_transform.rotation.x, target_transform.rotation.y, targetAngle);
				}
			}
			else
			{
				target_transform.Rotate(0f, 0f, (0f - rotationSpeed.Value) * Time.deltaTime);
				if (target_transform.localEulerAngles.z < targetAngle)
				{
					target_transform.localEulerAngles = new Vector3(target_transform.rotation.x, target_transform.rotation.y, targetAngle);
				}
			}
		}
	}
}

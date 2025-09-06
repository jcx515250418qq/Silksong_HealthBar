using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
	[ActionCategory(ActionCategory.Physics2D)]
	public class TiltBySpeed : ComponentAction<Rigidbody2D>
	{
		[RequiredField]
		[CheckForComponent(typeof(Rigidbody2D))]
		public FsmOwnerDefault gameObject;

		public FsmFloat tiltFactor;

		public FsmFloat tiltMax;

		public FsmFloat rotationSpeed;

		private float targetAngle;

		public override void Reset()
		{
			gameObject = null;
			tiltFactor = null;
			tiltMax = null;
		}

		public override void OnEnter()
		{
			DoTilt();
		}

		public override void OnUpdate()
		{
			DoTilt();
		}

		private void DoTilt()
		{
			GameObject ownerDefaultTarget = base.Fsm.GetOwnerDefaultTarget(gameObject);
			if (!UpdateCache(ownerDefaultTarget))
			{
				return;
			}
			float num = base.rigidbody2d.linearVelocity.x * tiltFactor.Value;
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
			GameObject ownerDefaultTarget = base.Fsm.GetOwnerDefaultTarget(gameObject);
			float num = targetAngle - ownerDefaultTarget.transform.localEulerAngles.z;
			if ((num < 0f) ? (num < -180f) : (!(num > 180f)))
			{
				ownerDefaultTarget.transform.Rotate(0f, 0f, rotationSpeed.Value * Time.deltaTime);
				if (ownerDefaultTarget.transform.localEulerAngles.z > targetAngle)
				{
					ownerDefaultTarget.transform.localEulerAngles = new Vector3(ownerDefaultTarget.transform.rotation.x, ownerDefaultTarget.transform.rotation.y, targetAngle);
				}
			}
			else
			{
				ownerDefaultTarget.transform.Rotate(0f, 0f, (0f - rotationSpeed.Value) * Time.deltaTime);
				if (ownerDefaultTarget.transform.localEulerAngles.z < targetAngle)
				{
					ownerDefaultTarget.transform.localEulerAngles = new Vector3(ownerDefaultTarget.transform.rotation.x, ownerDefaultTarget.transform.rotation.y, targetAngle);
				}
			}
		}
	}
}

using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
	[ActionCategory("Physics 2d")]
	[Tooltip("Rotate to a specific z angle over time")]
	public class RotateTo : RigidBody2dActionBase
	{
		[RequiredField]
		[CheckForComponent(typeof(Rigidbody2D))]
		public FsmOwnerDefault gameObject;

		public FsmFloat targetAngle;

		public FsmFloat speed;

		public override void Reset()
		{
			gameObject = null;
		}

		public override void OnUpdate()
		{
			DoRotateTo();
		}

		private void DoRotateTo()
		{
			GameObject ownerDefaultTarget = base.Fsm.GetOwnerDefaultTarget(gameObject);
			if (targetAngle.Value > 360f)
			{
				targetAngle.Value -= 360f;
			}
			float num = targetAngle.Value - ownerDefaultTarget.transform.localEulerAngles.z;
			if ((num < 0f) ? (num < -180f) : (!(num > 180f)))
			{
				ownerDefaultTarget.transform.Rotate(0f, 0f, speed.Value * Time.deltaTime);
				if (ownerDefaultTarget.transform.localEulerAngles.z > targetAngle.Value)
				{
					ownerDefaultTarget.transform.localEulerAngles = new Vector3(ownerDefaultTarget.transform.rotation.x, ownerDefaultTarget.transform.rotation.y, targetAngle.Value);
				}
			}
			else
			{
				ownerDefaultTarget.transform.Rotate(0f, 0f, (0f - speed.Value) * Time.deltaTime);
				if (ownerDefaultTarget.transform.localEulerAngles.z < targetAngle.Value)
				{
					ownerDefaultTarget.transform.localEulerAngles = new Vector3(ownerDefaultTarget.transform.rotation.x, ownerDefaultTarget.transform.rotation.y, targetAngle.Value);
				}
			}
		}
	}
}

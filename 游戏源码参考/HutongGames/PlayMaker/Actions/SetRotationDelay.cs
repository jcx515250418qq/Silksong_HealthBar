using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
	[ActionCategory(ActionCategory.Transform)]
	[Tooltip("Sets the Rotation of a Game Object. To leave any axis unchanged, set variable to 'None'.")]
	public class SetRotationDelay : FsmStateAction
	{
		[RequiredField]
		[Tooltip("The GameObject to rotate.")]
		public FsmOwnerDefault gameObject;

		[UIHint(UIHint.Variable)]
		[Tooltip("Use a stored quaternion, or vector angles below.")]
		public FsmQuaternion quaternion;

		[UIHint(UIHint.Variable)]
		[Title("Euler Angles")]
		[Tooltip("Use euler angles stored in a Vector3 variable, and/or set each axis below.")]
		public FsmVector3 vector;

		public FsmFloat xAngle;

		public FsmFloat yAngle;

		public FsmFloat zAngle;

		[Tooltip("Use local or world space.")]
		public Space space;

		public FsmFloat delay;

		private float timer;

		public override void Reset()
		{
			gameObject = null;
			quaternion = null;
			vector = null;
			xAngle = new FsmFloat
			{
				UseVariable = true
			};
			yAngle = new FsmFloat
			{
				UseVariable = true
			};
			zAngle = new FsmFloat
			{
				UseVariable = true
			};
			space = Space.World;
			delay = null;
		}

		public override void OnEnter()
		{
			timer = 0f;
		}

		public override void OnUpdate()
		{
			if (timer < delay.Value)
			{
				timer += Time.deltaTime;
				return;
			}
			DoSetRotation();
			Finish();
		}

		private void DoSetRotation()
		{
			GameObject ownerDefaultTarget = base.Fsm.GetOwnerDefaultTarget(gameObject);
			if (!(ownerDefaultTarget == null))
			{
				Vector3 vector = ((!quaternion.IsNone) ? quaternion.Value.eulerAngles : (this.vector.IsNone ? ((space == Space.Self) ? ownerDefaultTarget.transform.localEulerAngles : ownerDefaultTarget.transform.eulerAngles) : this.vector.Value));
				if (!xAngle.IsNone)
				{
					vector.x = xAngle.Value;
				}
				if (!yAngle.IsNone)
				{
					vector.y = yAngle.Value;
				}
				if (!zAngle.IsNone)
				{
					vector.z = zAngle.Value;
				}
				if (space == Space.Self)
				{
					ownerDefaultTarget.transform.localEulerAngles = vector;
				}
				else
				{
					ownerDefaultTarget.transform.eulerAngles = vector;
				}
			}
		}
	}
}

using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
	public class SetRigidBody2dInterpolation : FsmStateAction
	{
		[CheckForComponent(typeof(Rigidbody2D))]
		public FsmOwnerDefault Target;

		[ObjectType(typeof(RigidbodyInterpolation2D))]
		public FsmEnum SetValue;

		[UIHint(UIHint.Variable)]
		[ObjectType(typeof(RigidbodyInterpolation2D))]
		public FsmEnum SaveCurrentValue;

		public FsmBool ResetOnExit;

		private Rigidbody2D body;

		private RigidbodyInterpolation2D currentValue;

		public override void Reset()
		{
			Target = null;
			SetValue = null;
			SaveCurrentValue = null;
			ResetOnExit = false;
		}

		public override void OnEnter()
		{
			GameObject safe = Target.GetSafe(this);
			body = safe.GetComponent<Rigidbody2D>();
			currentValue = body.interpolation;
			SaveCurrentValue.Value = currentValue;
			body.interpolation = (RigidbodyInterpolation2D)(object)SetValue.Value;
			if (!ResetOnExit.Value)
			{
				Finish();
			}
		}

		public override void OnExit()
		{
			if (ResetOnExit.Value)
			{
				body.interpolation = currentValue;
			}
		}
	}
}

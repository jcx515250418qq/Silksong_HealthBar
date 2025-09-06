using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
	[ActionCategory(ActionCategory.AnimateVariables)]
	public class AnimateRigidBodyYPositionTo : EaseFsmAction
	{
		[RequiredField]
		[CheckForComponent(typeof(Rigidbody2D))]
		public FsmOwnerDefault GameObject;

		[RequiredField]
		public FsmFloat ToValue;

		private bool finishInNextStep;

		private Rigidbody2D body;

		private float fromValue;

		public override void Reset()
		{
			base.Reset();
			fromValue = 0f;
			ToValue = null;
			finishInNextStep = false;
		}

		public override void OnEnter()
		{
			base.OnEnter();
			body = base.Fsm.GetOwnerDefaultTarget(GameObject).GetComponent<Rigidbody2D>();
			if (!(body == null))
			{
				fromValue = body.position.y;
				fromFloats = new float[1];
				fromFloats[0] = fromValue;
				toFloats = new float[1];
				toFloats[0] = ToValue.Value;
				resultFloats = new float[1];
				finishInNextStep = false;
			}
		}

		public override void OnExit()
		{
			base.OnExit();
		}

		public override void OnUpdate()
		{
			base.OnUpdate();
			if (isRunning)
			{
				body.MovePosition(new Vector2(body.position.x, resultFloats[0]));
			}
			if (finishInNextStep)
			{
				Finish();
				if (finishEvent != null)
				{
					base.Fsm.Event(finishEvent);
				}
			}
			if (finishAction && !finishInNextStep)
			{
				body.MovePosition(new Vector2(body.position.x, reverse.IsNone ? ToValue.Value : (reverse.Value ? fromValue : ToValue.Value)));
				finishInNextStep = true;
			}
		}
	}
}

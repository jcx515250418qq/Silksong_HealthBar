using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
	[ActionCategory(ActionCategory.AnimateVariables)]
	public class AnimateRigidBody2DPositionTo : EaseFsmAction
	{
		[RequiredField]
		[CheckForComponent(typeof(Rigidbody2D))]
		public FsmOwnerDefault GameObject;

		[RequiredField]
		public FsmVector2 ToValue;

		private bool finishInNextStep;

		private Rigidbody2D body;

		private Vector3 fromValue;

		public override void Reset()
		{
			base.Reset();
			GameObject = null;
			ToValue = null;
		}

		public override void OnEnter()
		{
			base.OnEnter();
			body = base.Fsm.GetOwnerDefaultTarget(GameObject).GetComponent<Rigidbody2D>();
			if (!(body == null))
			{
				fromValue = body.position;
				fromFloats = new float[2];
				fromFloats[0] = fromValue.x;
				fromFloats[1] = fromValue.y;
				toFloats = new float[2];
				toFloats[0] = ToValue.Value.x;
				toFloats[1] = ToValue.Value.y;
				resultFloats = new float[2];
				finishInNextStep = false;
			}
		}

		public override void OnUpdate()
		{
			base.OnUpdate();
			if (isRunning)
			{
				body.MovePosition(new Vector2(resultFloats[0], resultFloats[1]));
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
				body.MovePosition(new Vector2(reverse.IsNone ? ToValue.Value.x : (reverse.Value ? fromValue.x : ToValue.Value.x), reverse.IsNone ? ToValue.Value.y : (reverse.Value ? fromValue.y : ToValue.Value.y)));
				finishInNextStep = true;
			}
		}
	}
}

using JetBrains.Annotations;
using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
	[ActionCategory(ActionCategory.AnimateVariables)]
	public class AnimateRigidBody2DPositionToV2 : EaseFsmAction
	{
		[RequiredField]
		[CheckForComponent(typeof(Rigidbody2D))]
		public FsmOwnerDefault GameObject;

		public FsmVector2 ToValue;

		[HideIf("HasVectorValue")]
		public FsmFloat ToX;

		[HideIf("HasVectorValue")]
		public FsmFloat ToY;

		private bool finishInNextStep;

		private Rigidbody2D body;

		private Vector3 fromValue;

		private Vector3 toValue;

		[UsedImplicitly]
		public bool HasVectorValue()
		{
			return !ToValue.IsNone;
		}

		public override void Reset()
		{
			base.Reset();
			GameObject = null;
			ToValue = new FsmVector2
			{
				UseVariable = true
			};
			ToX = null;
			ToY = null;
		}

		public override void OnEnter()
		{
			base.OnEnter();
			body = base.Fsm.GetOwnerDefaultTarget(GameObject).GetComponent<Rigidbody2D>();
			if (!(body == null))
			{
				fromValue = body.position;
				toValue = ((!ToValue.IsNone) ? ToValue.Value : new Vector2(ToX.Value, ToY.Value));
				fromFloats = new float[2];
				fromFloats[0] = fromValue.x;
				fromFloats[1] = fromValue.y;
				toFloats = new float[2];
				toFloats[0] = toValue.x;
				toFloats[1] = toValue.y;
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
				body.MovePosition(new Vector2(reverse.IsNone ? toValue.x : (reverse.Value ? fromValue.x : toValue.x), reverse.IsNone ? toValue.y : (reverse.Value ? fromValue.y : toValue.y)));
				finishInNextStep = true;
			}
		}
	}
}

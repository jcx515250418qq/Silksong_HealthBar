using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
	[ActionCategory("Physics 2d")]
	[Tooltip("Get the dimensions of the BoxCollider 2D")]
	public class BoundsBoxCollider : FsmStateAction
	{
		[RequiredField]
		public FsmOwnerDefault gameObject1;

		[Tooltip("Store the dimensions of the BoxCollider2D")]
		[UIHint(UIHint.Variable)]
		public FsmVector2 scaleVector2;

		[UIHint(UIHint.Variable)]
		public FsmFloat scaleX;

		[UIHint(UIHint.Variable)]
		public FsmFloat scaleY;

		public bool everyFrame;

		public override void Reset()
		{
			gameObject1 = null;
			scaleX = null;
			scaleY = null;
			everyFrame = false;
		}

		public void GetEm()
		{
			Vector2 value = base.Fsm.GetOwnerDefaultTarget(gameObject1).GetComponent<BoxCollider2D>().bounds.size;
			if (scaleVector2 != null)
			{
				scaleVector2.Value = value;
			}
			if (scaleX != null)
			{
				scaleX.Value = value.x;
			}
			if (scaleY != null)
			{
				scaleY.Value = value.y;
			}
		}

		public override void OnEnter()
		{
			GetEm();
			if (!everyFrame)
			{
				Finish();
			}
		}

		public override void OnUpdate()
		{
			GetEm();
		}
	}
}

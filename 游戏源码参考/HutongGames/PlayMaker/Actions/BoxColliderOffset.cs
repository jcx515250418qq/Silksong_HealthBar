using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
	[ActionCategory("Physics 2d")]
	[Tooltip("Get the offset of the BoxCollider 2D as a Vector2")]
	public class BoxColliderOffset : FsmStateAction
	{
		[RequiredField]
		public FsmOwnerDefault gameObject1;

		[Tooltip("Vector2 where the offset of the BoxCollider2D is stored")]
		[UIHint(UIHint.Variable)]
		public FsmVector2 offsetVector2;

		[UIHint(UIHint.Variable)]
		public FsmFloat offsetX;

		[UIHint(UIHint.Variable)]
		public FsmFloat offsetY;

		public bool everyFrame;

		public override void Reset()
		{
			gameObject1 = null;
			offsetX = null;
			offsetY = null;
			everyFrame = false;
		}

		public void GetOffset()
		{
			Vector2 offset = base.Fsm.GetOwnerDefaultTarget(gameObject1).GetComponent<BoxCollider2D>().offset;
			if (offsetVector2 != null)
			{
				offsetVector2.Value = offset;
			}
			if (offsetX != null)
			{
				offsetX.Value = offset.x;
			}
			if (offsetY != null)
			{
				offsetY.Value = offset.y;
			}
		}

		public override void OnEnter()
		{
			GetOffset();
			if (!everyFrame)
			{
				Finish();
			}
		}

		public override void OnUpdate()
		{
			GetOffset();
		}
	}
}

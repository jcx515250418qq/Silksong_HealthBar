using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
	[ActionCategory("Physics 2d")]
	[Tooltip("Set the dimensions of the first BoxCollider 2D on object")]
	public class SetBoxCollider2DSize : FsmStateAction
	{
		[RequiredField]
		public FsmOwnerDefault gameObject;

		public FsmFloat width;

		public FsmFloat height;

		public FsmFloat offsetX;

		public FsmFloat offsetY;

		public override void Reset()
		{
			width = new FsmFloat
			{
				UseVariable = true
			};
			height = new FsmFloat
			{
				UseVariable = true
			};
			offsetX = new FsmFloat
			{
				UseVariable = true
			};
			offsetY = new FsmFloat
			{
				UseVariable = true
			};
		}

		public void SetDimensions()
		{
			BoxCollider2D component = base.Fsm.GetOwnerDefaultTarget(gameObject).GetComponent<BoxCollider2D>();
			Vector2 size = component.size;
			if (!width.IsNone)
			{
				size.x = width.Value;
			}
			if (!height.IsNone)
			{
				size.y = height.Value;
			}
			if (!offsetX.IsNone)
			{
				component.offset = new Vector3(offsetX.Value, component.offset.y);
			}
			if (!offsetY.IsNone)
			{
				component.offset = new Vector3(component.offset.x, offsetY.Value);
			}
			component.size = size;
		}

		public override void OnEnter()
		{
			SetDimensions();
			Finish();
		}
	}
}

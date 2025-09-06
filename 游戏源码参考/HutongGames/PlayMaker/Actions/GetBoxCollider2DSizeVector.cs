using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
	[ActionCategory("Physics 2d")]
	[Tooltip("Get the dimensions of the first BoxCollider 2D on object. Uses vector2s")]
	public class GetBoxCollider2DSizeVector : FsmStateAction
	{
		[RequiredField]
		public FsmOwnerDefault gameObject1;

		public FsmVector2 size;

		public FsmVector2 offset;

		public override void Reset()
		{
			size = new FsmVector2
			{
				UseVariable = true
			};
			offset = new FsmVector2
			{
				UseVariable = true
			};
		}

		public void GetDimensions()
		{
			GameObject ownerDefaultTarget = base.Fsm.GetOwnerDefaultTarget(gameObject1);
			if (!ownerDefaultTarget)
			{
				Debug.LogError("gameObject1 is null!", base.Owner);
				return;
			}
			BoxCollider2D component = ownerDefaultTarget.GetComponent<BoxCollider2D>();
			if (!size.IsNone)
			{
				size.Value = component.size;
			}
			if (!offset.IsNone)
			{
				offset.Value = component.offset;
			}
		}

		public override void OnEnter()
		{
			GetDimensions();
			Finish();
		}
	}
}

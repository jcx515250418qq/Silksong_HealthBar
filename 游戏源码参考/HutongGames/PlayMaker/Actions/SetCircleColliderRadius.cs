using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
	[ActionCategory("Physics 2d")]
	[Tooltip("Set BoxCollider2D to active or inactive. Can only be one collider on object. ")]
	public class SetCircleColliderRadius : FsmStateAction
	{
		[RequiredField]
		public FsmOwnerDefault gameObject;

		public FsmFloat radius;

		public bool everyFrame;

		public override void Reset()
		{
			gameObject = null;
			radius = null;
		}

		public override void OnEnter()
		{
			SetRadius();
			if (!everyFrame)
			{
				Finish();
			}
		}

		public override void OnUpdate()
		{
			SetRadius();
		}

		private void SetRadius()
		{
			if (gameObject != null)
			{
				CircleCollider2D component = base.Fsm.GetOwnerDefaultTarget(gameObject).GetComponent<CircleCollider2D>();
				if (component != null)
				{
					component.radius = radius.Value;
				}
			}
		}
	}
}

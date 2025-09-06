using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
	[ActionCategory("Physics 2d")]
	public class SetCircleColliderTrigger : FsmStateAction
	{
		[RequiredField]
		[Tooltip("The particle emitting GameObject")]
		public FsmOwnerDefault gameObject;

		public FsmBool trigger;

		public override void Reset()
		{
			gameObject = null;
			trigger = false;
		}

		public override void OnEnter()
		{
			if (gameObject != null)
			{
				CircleCollider2D component = base.Fsm.GetOwnerDefaultTarget(gameObject).GetComponent<CircleCollider2D>();
				if (component != null)
				{
					component.isTrigger = trigger.Value;
				}
			}
			Finish();
		}
	}
}

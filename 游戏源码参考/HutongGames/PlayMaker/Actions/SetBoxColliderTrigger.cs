using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
	[ActionCategory("Physics 2d")]
	[Tooltip("Set BoxCollider2D to active or inactive. Can only be one collider on object. ")]
	public class SetBoxColliderTrigger : FsmStateAction
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
				BoxCollider2D component = base.Fsm.GetOwnerDefaultTarget(gameObject).GetComponent<BoxCollider2D>();
				if (component != null)
				{
					component.isTrigger = trigger.Value;
				}
			}
			Finish();
		}
	}
}

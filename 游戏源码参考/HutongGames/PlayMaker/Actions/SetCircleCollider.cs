using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
	[ActionCategory("Physics 2d")]
	[Tooltip("Set BoxCollider2D to active or inactive. Can only be one collider on object. ")]
	public class SetCircleCollider : FsmStateAction
	{
		[RequiredField]
		[Tooltip("The particle emitting GameObject")]
		public FsmOwnerDefault gameObject;

		public FsmBool active;

		public override void Reset()
		{
			gameObject = null;
			active = false;
		}

		public override void OnEnter()
		{
			if (gameObject != null)
			{
				GameObject ownerDefaultTarget = base.Fsm.GetOwnerDefaultTarget(gameObject);
				if (ownerDefaultTarget == null)
				{
					return;
				}
				CircleCollider2D component = ownerDefaultTarget.GetComponent<CircleCollider2D>();
				if (component != null)
				{
					component.enabled = active.Value;
				}
			}
			Finish();
		}
	}
}

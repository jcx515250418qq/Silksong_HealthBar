using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
	[ActionCategory("Physics 2d")]
	public class ActivateBoxCollider2D : FsmStateAction
	{
		[RequiredField]
		public FsmOwnerDefault gameObject;

		public FsmBool active;

		public bool resetOnExit;

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
				BoxCollider2D boxCollider2D = (ownerDefaultTarget ? ownerDefaultTarget.GetComponent<BoxCollider2D>() : null);
				if (boxCollider2D != null)
				{
					boxCollider2D.enabled = active.Value;
				}
			}
			Finish();
		}

		public override void OnExit()
		{
			if (resetOnExit && gameObject != null)
			{
				BoxCollider2D component = base.Fsm.GetOwnerDefaultTarget(gameObject).GetComponent<BoxCollider2D>();
				if (component != null)
				{
					component.enabled = !active.Value;
				}
			}
		}
	}
}

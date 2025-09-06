using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
	[ActionCategory("Physics 2d")]
	[Tooltip("Set PolygonCollider to active or inactive. Can only be one collider on object. ")]
	public class SetPolygonCollider : FsmStateAction
	{
		[RequiredField]
		[Tooltip("The particle emitting GameObject")]
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
				PolygonCollider2D polygonCollider2D = (ownerDefaultTarget ? ownerDefaultTarget.GetComponent<PolygonCollider2D>() : null);
				if (polygonCollider2D != null)
				{
					polygonCollider2D.enabled = active.Value;
				}
			}
			Finish();
		}

		public override void OnExit()
		{
			if (resetOnExit && gameObject != null)
			{
				GameObject ownerDefaultTarget = base.Fsm.GetOwnerDefaultTarget(gameObject);
				PolygonCollider2D polygonCollider2D = (ownerDefaultTarget ? ownerDefaultTarget.GetComponent<PolygonCollider2D>() : null);
				if (polygonCollider2D != null)
				{
					polygonCollider2D.enabled = !active.Value;
				}
			}
		}
	}
}

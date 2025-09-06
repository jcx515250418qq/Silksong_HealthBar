using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
	[ActionCategory("Physics 2d")]
	[Tooltip("Set Collider2D to active or inactive.")]
	public class SetCollider : FsmStateAction
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
			GameObject ownerDefaultTarget = base.Fsm.GetOwnerDefaultTarget(gameObject);
			if (ownerDefaultTarget == null)
			{
				Finish();
				return;
			}
			if (gameObject != null)
			{
				Collider2D[] components = ownerDefaultTarget.GetComponents<Collider2D>();
				for (int i = 0; i < components.Length; i++)
				{
					components[i].enabled = active.Value;
				}
			}
			Finish();
		}

		public override void OnExit()
		{
			if (!resetOnExit)
			{
				return;
			}
			GameObject ownerDefaultTarget = base.Fsm.GetOwnerDefaultTarget(gameObject);
			if (!(ownerDefaultTarget == null) && gameObject != null)
			{
				Collider2D[] components = ownerDefaultTarget.GetComponents<Collider2D>();
				for (int i = 0; i < components.Length; i++)
				{
					components[i].enabled = !active.Value;
				}
			}
		}
	}
}

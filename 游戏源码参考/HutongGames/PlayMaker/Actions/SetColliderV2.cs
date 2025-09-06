using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
	[ActionCategory("Physics 2d")]
	[Tooltip("Set Collider2D to active or inactive. Can only be one collider on object. ")]
	public class SetColliderV2 : FsmStateAction
	{
		[RequiredField]
		[Tooltip("The particle emitting GameObject")]
		public FsmOwnerDefault Target;

		public FsmBool SetActive;

		public bool EveryFrame;

		public bool ResetOnExit;

		private Collider2D col;

		private bool initialState;

		public override void Reset()
		{
			Target = null;
			SetActive = null;
			EveryFrame = false;
			ResetOnExit = false;
		}

		public override void OnEnter()
		{
			GameObject ownerDefaultTarget = base.Fsm.GetOwnerDefaultTarget(Target);
			if (ownerDefaultTarget == null)
			{
				Finish();
				return;
			}
			if (Target != null)
			{
				col = ownerDefaultTarget.GetComponent<Collider2D>();
				if (col != null)
				{
					initialState = col.enabled;
					col.enabled = SetActive.Value;
				}
			}
			if (!EveryFrame || !col)
			{
				Finish();
			}
		}

		public override void OnUpdate()
		{
			if ((bool)col)
			{
				col.enabled = SetActive.Value;
			}
		}

		public override void OnExit()
		{
			if (ResetOnExit && col != null)
			{
				col.enabled = initialState;
			}
		}
	}
}

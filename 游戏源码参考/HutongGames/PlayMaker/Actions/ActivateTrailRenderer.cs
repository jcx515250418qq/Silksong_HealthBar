using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
	[ActionCategory("Trail Renderer")]
	[Tooltip("Set trail renderer parameters")]
	public class ActivateTrailRenderer : FsmStateAction
	{
		[RequiredField]
		[Tooltip("The particle emitting GameObject")]
		public FsmOwnerDefault gameObject;

		public FsmBool activate;

		public override void Reset()
		{
			gameObject = null;
			activate = null;
		}

		public override void OnEnter()
		{
			if (gameObject != null)
			{
				base.Fsm.GetOwnerDefaultTarget(gameObject).GetComponent<TrailRenderer>().enabled = activate.Value;
				Finish();
			}
			else
			{
				Finish();
			}
		}
	}
}

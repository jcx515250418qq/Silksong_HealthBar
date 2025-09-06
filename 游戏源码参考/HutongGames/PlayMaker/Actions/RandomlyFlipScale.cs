using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
	[ActionCategory(ActionCategory.Math)]
	[Tooltip("50/50 chance to either x scale as is or flip it")]
	public class RandomlyFlipScale : FsmStateAction
	{
		public FsmOwnerDefault gameObject;

		public override void Reset()
		{
			gameObject = null;
		}

		public override void OnEnter()
		{
			if ((double)Random.value >= 0.5)
			{
				GameObject ownerDefaultTarget = base.Fsm.GetOwnerDefaultTarget(gameObject);
				if (ownerDefaultTarget == null)
				{
					return;
				}
				ownerDefaultTarget.transform.localScale = new Vector3(0f - ownerDefaultTarget.transform.localScale.x, ownerDefaultTarget.transform.localScale.y, ownerDefaultTarget.transform.localScale.z);
			}
			Finish();
		}
	}
}

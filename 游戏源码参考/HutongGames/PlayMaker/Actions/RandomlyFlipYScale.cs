using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
	[ActionCategory(ActionCategory.Math)]
	public class RandomlyFlipYScale : FsmStateAction
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
				ownerDefaultTarget.transform.localScale = new Vector3(ownerDefaultTarget.transform.localScale.x, 0f - ownerDefaultTarget.transform.localScale.y, ownerDefaultTarget.transform.localScale.z);
			}
			Finish();
		}
	}
}

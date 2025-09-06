using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
	[ActionCategory(ActionCategory.Transform)]
	public class FlipScaleOnExit : FsmStateAction
	{
		[RequiredField]
		public FsmOwnerDefault gameObject;

		public bool flipHorizontally;

		public bool flipVertically;

		public override void Reset()
		{
			flipHorizontally = false;
			flipVertically = false;
		}

		public override void OnExit()
		{
			DoFlipScale();
		}

		private void DoFlipScale()
		{
			GameObject ownerDefaultTarget = base.Fsm.GetOwnerDefaultTarget(gameObject);
			if (!(ownerDefaultTarget == null))
			{
				Vector3 localScale = ownerDefaultTarget.transform.localScale;
				if (flipHorizontally)
				{
					localScale.x = 0f - localScale.x;
				}
				if (flipVertically)
				{
					localScale.y = 0f - localScale.y;
				}
				ownerDefaultTarget.transform.localScale = localScale;
			}
		}
	}
}

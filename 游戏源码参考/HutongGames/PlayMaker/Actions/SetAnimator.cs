using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
	public class SetAnimator : FsmStateAction
	{
		public FsmOwnerDefault target;

		public FsmBool active;

		public override void Reset()
		{
			target = null;
			active = null;
		}

		public override void OnEnter()
		{
			GameObject safe = target.GetSafe(this);
			if ((bool)safe)
			{
				Animator component = safe.GetComponent<Animator>();
				if ((bool)component)
				{
					component.enabled = active.Value;
				}
			}
			Finish();
		}
	}
}

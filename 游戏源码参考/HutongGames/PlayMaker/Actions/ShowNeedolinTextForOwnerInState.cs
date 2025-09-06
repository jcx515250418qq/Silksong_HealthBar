using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
	public class ShowNeedolinTextForOwnerInState : FsmStateAction
	{
		[RequiredField]
		[CheckForComponent(typeof(NeedolinTextOwner))]
		public FsmOwnerDefault Target;

		private NeedolinTextOwner owner;

		public override void Reset()
		{
			Target = null;
		}

		public override void OnEnter()
		{
			owner = null;
			GameObject safe = Target.GetSafe(this);
			if (!safe)
			{
				return;
			}
			BlackThreadState component = safe.GetComponent<BlackThreadState>();
			if (!component || !component.IsInForcedSing)
			{
				owner = safe.GetComponent<NeedolinTextOwner>();
				if ((bool)owner)
				{
					owner.AddNeedolinText();
				}
			}
		}

		public override void OnExit()
		{
			if ((bool)owner)
			{
				owner.RemoveNeedolinText();
				owner = null;
			}
		}
	}
}

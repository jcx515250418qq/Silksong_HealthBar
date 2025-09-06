using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
	public class SetNeedolinTextOwner : FsmStateAction
	{
		[RequiredField]
		[CheckForComponent(typeof(NeedolinTextOwner))]
		public FsmOwnerDefault Target;

		public FsmBool SetEnabled;

		public override void Reset()
		{
			Target = null;
			SetEnabled = null;
		}

		public override void OnEnter()
		{
			GameObject safe = Target.GetSafe(this);
			if ((bool)safe)
			{
				NeedolinTextOwner component = safe.GetComponent<NeedolinTextOwner>();
				if ((bool)component)
				{
					component.enabled = SetEnabled.Value;
				}
			}
			Finish();
		}
	}
}

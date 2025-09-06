using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
	public class SetRecoilBlocked : FsmStateAction
	{
		public FsmOwnerDefault Target;

		public FsmBool IsUpBlocked;

		public FsmBool IsDownBlocked;

		public FsmBool IsLeftBlocked;

		public FsmBool IsRightBlocked;

		public override void Reset()
		{
			Target = null;
			IsUpBlocked = null;
			IsDownBlocked = null;
			IsLeftBlocked = null;
			IsRightBlocked = null;
		}

		public override void OnEnter()
		{
			GameObject safe = Target.GetSafe(this);
			if ((bool)safe)
			{
				Recoil component = safe.GetComponent<Recoil>();
				if ((bool)component)
				{
					if (!IsUpBlocked.IsNone)
					{
						component.IsUpBlocked = IsUpBlocked.Value;
					}
					if (!IsDownBlocked.IsNone)
					{
						component.IsDownBlocked = IsDownBlocked.Value;
					}
					if (!IsLeftBlocked.IsNone)
					{
						component.IsLeftBlocked = IsLeftBlocked.Value;
					}
					if (!IsRightBlocked.IsNone)
					{
						component.IsRightBlocked = IsRightBlocked.Value;
					}
				}
			}
			Finish();
		}
	}
}

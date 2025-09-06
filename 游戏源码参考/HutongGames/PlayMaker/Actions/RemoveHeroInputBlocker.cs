using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
	public class RemoveHeroInputBlocker : FsmStateAction
	{
		public FsmOwnerDefault Blocker;

		public override void Reset()
		{
			Blocker = null;
		}

		public override void OnEnter()
		{
			GameObject safe = Blocker.GetSafe(this);
			if ((bool)safe)
			{
				HeroController instance = HeroController.instance;
				if ((bool)instance)
				{
					instance.RemoveInputBlocker(safe);
				}
			}
			Finish();
		}
	}
}

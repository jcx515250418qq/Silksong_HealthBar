using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
	public class AddHeroInputBlocker : FsmStateAction
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
					instance.AddInputBlocker(safe);
				}
			}
			Finish();
		}
	}
}

using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
	public class SetHeroParent : FsmStateAction
	{
		public FsmOwnerDefault NewParent;

		public override void Reset()
		{
			NewParent = null;
		}

		public override void OnEnter()
		{
			GameObject safe = NewParent.GetSafe(this);
			HeroController instance = HeroController.instance;
			if ((bool)instance)
			{
				instance.SetHeroParent(safe ? safe.transform : null);
			}
			Finish();
		}
	}
}

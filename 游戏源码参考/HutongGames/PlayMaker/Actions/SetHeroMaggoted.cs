using GlobalSettings;

namespace HutongGames.PlayMaker.Actions
{
	public class SetHeroMaggoted : FsmStateAction
	{
		public FsmBool Value;

		public override void Reset()
		{
			Value = true;
		}

		public override void OnEnter()
		{
			if (Value.Value)
			{
				HeroController instance = HeroController.instance;
				if (Gameplay.MaggotCharm.IsEquipped && instance.playerData.MaggotCharmHits < 3)
				{
					instance.DidMaggotCharmHit();
				}
				else
				{
					MaggotRegion.SetIsMaggoted(value: true);
				}
			}
			else
			{
				MaggotRegion.SetIsMaggoted(value: false);
			}
			Finish();
		}
	}
}

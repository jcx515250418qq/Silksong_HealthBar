using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
	public class HeroChargeEffectStart : FsmStateAction
	{
		public FsmColor TintColor;

		public override void Reset()
		{
			TintColor = Color.white;
		}

		public override void OnEnter()
		{
			ManagerSingleton<HeroChargeEffects>.Instance.StartCharge(TintColor.Value);
			Finish();
		}
	}
}

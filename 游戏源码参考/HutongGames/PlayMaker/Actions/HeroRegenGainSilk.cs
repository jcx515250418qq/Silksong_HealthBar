using System.Collections;
using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
	public class HeroRegenGainSilk : FsmStateAction
	{
		public FsmInt ExtraSilkAmount;

		public override void Reset()
		{
			ExtraSilkAmount = null;
		}

		public override void OnEnter()
		{
			HeroController instance = HeroController.instance;
			instance.StartCoroutine(GainSilkOverTime(ExtraSilkAmount.Value, instance));
			Finish();
		}

		private static IEnumerator GainSilkOverTime(int silkAmount, HeroController hc)
		{
			PlayerData pd = hc.playerData;
			int num = pd.CurrentSilkRegenMax - pd.silk;
			if (num > 0)
			{
				silkAmount += num;
			}
			WaitForSeconds wait = new WaitForSeconds(0.1f);
			int addedAmount = 0;
			while (addedAmount < silkAmount && pd.silk < pd.CurrentSilkMax)
			{
				hc.AddSilk(1, heroEffect: false);
				addedAmount++;
				yield return wait;
			}
		}
	}
}

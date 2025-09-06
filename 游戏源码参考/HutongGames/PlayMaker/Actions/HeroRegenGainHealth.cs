using System.Collections;
using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
	public class HeroRegenGainHealth : FsmStateAction
	{
		public FsmInt ExtraHealthAmount;

		public override void Reset()
		{
			ExtraHealthAmount = null;
		}

		public override void OnEnter()
		{
			HeroController instance = HeroController.instance;
			instance.StartCoroutine(GainHealthOverTime(ExtraHealthAmount.Value, instance));
			Finish();
		}

		private static IEnumerator GainHealthOverTime(int healthAmount, HeroController hc)
		{
			PlayerData pd = hc.playerData;
			WaitForSeconds wait = new WaitForSeconds(0.2f);
			int addedAmount = 0;
			while (addedAmount < healthAmount && pd.health < pd.CurrentMaxHealth)
			{
				hc.AddHealth(1);
				addedAmount++;
				yield return wait;
			}
		}
	}
}

using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
	[ActionCategory(ActionCategory.Math)]
	[Tooltip("Sets a Bool Variable to True or False randomly.")]
	public class RandomBoolPercent : FsmStateAction
	{
		[UIHint(UIHint.Variable)]
		public FsmBool storeResult;

		public FsmFloat trueChance;

		public override void Reset()
		{
			trueChance = 50f;
			storeResult = null;
		}

		public override void OnEnter()
		{
			float num = Random.Range(1, 100);
			storeResult.Value = num < trueChance.Value;
			Finish();
		}
	}
}

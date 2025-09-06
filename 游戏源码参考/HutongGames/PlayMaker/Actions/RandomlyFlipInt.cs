using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
	[ActionCategory(ActionCategory.Math)]
	[Tooltip("50/50 chance to either leave a int as is or multiply it by -1")]
	public class RandomlyFlipInt : FsmStateAction
	{
		[UIHint(UIHint.Variable)]
		public FsmInt storeResult;

		public override void Reset()
		{
			storeResult = null;
		}

		public override void OnEnter()
		{
			if ((double)Random.value >= 0.5)
			{
				storeResult.Value *= -1;
			}
			Finish();
		}
	}
}

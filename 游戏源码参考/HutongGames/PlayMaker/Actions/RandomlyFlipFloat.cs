using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
	[ActionCategory(ActionCategory.Math)]
	[Tooltip("50/50 chance to either leave a flaot as is or multiply it by -1")]
	public class RandomlyFlipFloat : FsmStateAction
	{
		[UIHint(UIHint.Variable)]
		public FsmFloat storeResult;

		public override void Reset()
		{
			storeResult = null;
		}

		public override void OnEnter()
		{
			if ((double)Random.value >= 0.5)
			{
				storeResult.Value *= -1f;
			}
			Finish();
		}
	}
}

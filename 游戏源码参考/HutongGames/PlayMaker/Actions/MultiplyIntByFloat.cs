using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
	[ActionCategory(ActionCategory.Math)]
	public class MultiplyIntByFloat : FsmStateAction
	{
		[RequiredField]
		public FsmInt integer;

		[RequiredField]
		public FsmFloat multiplyFloat;

		[RequiredField]
		[UIHint(UIHint.Variable)]
		public FsmInt storeResult;

		public bool everyFrame;

		public bool forceRoundUp;

		public override void Reset()
		{
			integer = null;
			multiplyFloat = null;
			storeResult = null;
			everyFrame = false;
		}

		public override void OnEnter()
		{
			DoMultiply();
			if (!everyFrame)
			{
				Finish();
			}
		}

		public override void OnUpdate()
		{
			DoMultiply();
		}

		private void DoMultiply()
		{
			if (forceRoundUp)
			{
				storeResult.Value = (int)Mathf.Ceil((float)integer.Value * multiplyFloat.Value);
			}
			else
			{
				storeResult.Value = (int)((float)integer.Value * multiplyFloat.Value);
			}
		}
	}
}

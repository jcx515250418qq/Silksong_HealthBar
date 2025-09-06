using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
	[ActionCategory(ActionCategory.Math)]
	public class FloatSignToBool : FsmStateAction
	{
		public FsmFloat Value;

		[UIHint(UIHint.Variable)]
		public FsmBool StoreIsPositive;

		public bool EveryFrame;

		public override void Reset()
		{
			Value = null;
			StoreIsPositive = null;
		}

		public override void OnEnter()
		{
			DoAction();
			if (!EveryFrame)
			{
				Finish();
			}
		}

		public override void OnUpdate()
		{
			DoAction();
		}

		private void DoAction()
		{
			StoreIsPositive.Value = Mathf.Sign(Value.Value) > 0f;
		}
	}
}

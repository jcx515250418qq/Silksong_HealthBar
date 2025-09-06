using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
	[ActionCategory(ActionCategory.Math)]
	public class FloatSign : FsmStateAction
	{
		public FsmFloat Value;

		[UIHint(UIHint.Variable)]
		public FsmFloat StoreValue;

		public bool EveryFrame;

		public override void Reset()
		{
			Value = null;
			StoreValue = null;
		}

		public override void OnEnter()
		{
			DoAction();
			if (!EveryFrame)
			{
				Finish();
			}
		}

		private void DoAction()
		{
			StoreValue.Value = Mathf.Sign(Value.Value);
		}
	}
}

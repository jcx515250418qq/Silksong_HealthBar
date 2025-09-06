namespace HutongGames.PlayMaker.Actions
{
	[ActionCategory(ActionCategory.Logic)]
	[Tooltip("Sends Events based on the value of a Boolean Variable.")]
	public class BoolTestV2 : FsmStateAction
	{
		[UIHint(UIHint.Variable)]
		[RequiredField]
		public FsmBool BoolVariable;

		public FsmBool ExpectedValue;

		public FsmEvent IsExpected;

		public FsmEvent IsNotExpected;

		public bool EveryFrame;

		public override void Reset()
		{
			BoolVariable = null;
			ExpectedValue = null;
			IsExpected = null;
			IsNotExpected = null;
			EveryFrame = false;
		}

		public override void OnEnter()
		{
			EvalEvents();
			if (!EveryFrame)
			{
				Finish();
			}
		}

		public override void OnUpdate()
		{
			EvalEvents();
		}

		private void EvalEvents()
		{
			base.Fsm.Event((BoolVariable.Value == ExpectedValue.Value) ? IsExpected : IsNotExpected);
		}
	}
}

namespace HutongGames.PlayMaker.Actions
{
	public class IntSwitchToArray : FsmStateAction
	{
		[RequiredField]
		public FsmInt Value;

		[CompoundArray("Switches", "From Int", "To FsmArray")]
		public FsmInt[] From;

		[UIHint(UIHint.Variable)]
		public FsmArray[] To;

		[UIHint(UIHint.Variable)]
		public FsmArray StoreValue;

		public bool EveryFrame;

		public override void Reset()
		{
			Value = null;
			From = null;
			To = null;
			StoreValue = null;
			EveryFrame = false;
		}

		public override void OnEnter()
		{
			DoEnumSwitch();
			if (!EveryFrame)
			{
				Finish();
			}
		}

		public override void OnUpdate()
		{
			DoEnumSwitch();
		}

		private void DoEnumSwitch()
		{
			for (int i = 0; i < From.Length; i++)
			{
				if (Value.Value == From[i].Value)
				{
					StoreValue.CopyValues(To[i]);
				}
			}
		}
	}
}

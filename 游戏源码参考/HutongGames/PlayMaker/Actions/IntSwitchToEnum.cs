namespace HutongGames.PlayMaker.Actions
{
	public class IntSwitchToEnum : FsmStateAction
	{
		[RequiredField]
		public FsmInt Value;

		[CompoundArray("Switches", "From Int", "To Enum")]
		public FsmInt[] From;

		[MatchFieldType("StoreValue")]
		public FsmEnum[] To;

		[UIHint(UIHint.Variable)]
		public FsmEnum StoreValue;

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
					StoreValue.Value = To[i].Value;
				}
			}
		}
	}
}

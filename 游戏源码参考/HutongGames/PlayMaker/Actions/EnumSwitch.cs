namespace HutongGames.PlayMaker.Actions
{
	[ActionCategory(ActionCategory.Logic)]
	[Tooltip("Sends an Event based on the value of an Enum Variable.")]
	public class EnumSwitch : FsmStateAction
	{
		[RequiredField]
		[UIHint(UIHint.Variable)]
		[Tooltip("The Enum Variable to use.")]
		public FsmEnum enumVariable;

		[CompoundArray("Enum Switches", "Compare Enum Value", "Send Event")]
		[MatchFieldType("enumVariable")]
		[Tooltip("Compare Enum Values")]
		public FsmEnum[] compareTo;

		[Tooltip("Event to send if the Enum Variable value is equal.")]
		public FsmEvent[] sendEvent;

		[Tooltip("Repeat every frame.")]
		public bool everyFrame;

		public override void Reset()
		{
			enumVariable = null;
			compareTo = new FsmEnum[0];
			sendEvent = new FsmEvent[0];
			everyFrame = false;
		}

		public override void OnEnter()
		{
			DoEnumSwitch();
			if (!everyFrame)
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
			if (enumVariable.IsNone)
			{
				return;
			}
			for (int i = 0; i < compareTo.Length; i++)
			{
				if (object.Equals(enumVariable.Value, compareTo[i].Value))
				{
					base.Fsm.Event(sendEvent[i]);
					break;
				}
			}
		}
	}
}

namespace HutongGames.PlayMaker.Actions
{
	public class CompareNames : FsmStateAction
	{
		public FsmString name;

		[ArrayEditor(VariableType.String, "", 0, 0, 65536)]
		public FsmArray strings;

		public FsmEventTarget target;

		public FsmEvent trueEvent;

		public FsmEvent falseEvent;

		public override void Reset()
		{
			name = new FsmString();
			target = new FsmEventTarget();
			strings = new FsmArray();
			trueEvent = null;
			falseEvent = null;
		}

		public override void OnEnter()
		{
			if (!name.IsNone && name.Value != "")
			{
				string[] stringValues = strings.stringValues;
				foreach (string value in stringValues)
				{
					if (name.Value.Contains(value))
					{
						base.Fsm.Event(target, trueEvent);
						Finish();
						return;
					}
				}
				base.Fsm.Event(target, falseEvent);
			}
			Finish();
		}
	}
}

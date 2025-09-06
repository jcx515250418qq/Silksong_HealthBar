namespace HutongGames.PlayMaker.Actions
{
	[ActionCategory(ActionCategory.Logic)]
	[Tooltip("Sends an event if bool true, and resets bool to false.")]
	public class BoolTrigger : FsmStateAction
	{
		[RequiredField]
		[UIHint(UIHint.Variable)]
		[Readonly]
		[Tooltip("The Bool variable to test.")]
		public FsmBool boolVariable;

		[Tooltip("Event to send if the Bool variable is True.")]
		public FsmEvent triggerEvent;

		public override void Reset()
		{
			boolVariable = null;
			triggerEvent = null;
		}

		public override void OnEnter()
		{
			if (boolVariable.Value)
			{
				base.Fsm.Event(triggerEvent);
				boolVariable.Value = false;
			}
			Finish();
		}
	}
}

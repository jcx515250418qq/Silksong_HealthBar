namespace HutongGames.PlayMaker.Actions
{
	[ActionCategory(ActionCategory.StateMachine)]
	[Tooltip("Sets the String data to send with the next event. Get the Event Data, along with sender information, using the {{Get Event Info}} action.")]
	public class SetEventStringData : FsmStateAction
	{
		[Tooltip("The string value to send.")]
		public FsmString stringData;

		public override void Reset()
		{
			stringData = null;
		}

		public override void OnEnter()
		{
			Fsm.EventData.StringData = stringData.Value;
			Finish();
		}
	}
}

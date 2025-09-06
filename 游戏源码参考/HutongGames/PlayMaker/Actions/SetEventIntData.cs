namespace HutongGames.PlayMaker.Actions
{
	[ActionCategory(ActionCategory.StateMachine)]
	[Tooltip("Sets the Int data to send with the next event. Get the Event Data, along with sender information, using the {{Get Event Info}} action.")]
	public class SetEventIntData : FsmStateAction
	{
		[Tooltip("The int value to send.")]
		public FsmInt intData;

		public override void Reset()
		{
			intData = null;
		}

		public override void OnEnter()
		{
			Fsm.EventData.IntData = intData.Value;
			Finish();
		}
	}
}

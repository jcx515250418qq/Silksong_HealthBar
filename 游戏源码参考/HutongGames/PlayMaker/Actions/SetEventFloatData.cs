namespace HutongGames.PlayMaker.Actions
{
	[ActionCategory(ActionCategory.StateMachine)]
	[Tooltip("Sets the Float data to send with the next event. Get the Event Data, along with sender information, using the {{Get Event Info}} action.")]
	public class SetEventFloatData : FsmStateAction
	{
		[Tooltip("The float value to send.")]
		public FsmFloat floatData;

		public override void Reset()
		{
			floatData = null;
		}

		public override void OnEnter()
		{
			Fsm.EventData.FloatData = floatData.Value;
			Finish();
		}
	}
}

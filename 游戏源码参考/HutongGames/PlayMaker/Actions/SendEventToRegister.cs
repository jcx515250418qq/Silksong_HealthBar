namespace HutongGames.PlayMaker.Actions
{
	[ActionCategory("Hollow Knight")]
	public class SendEventToRegister : FsmStateAction
	{
		public FsmString eventName;

		public override void Reset()
		{
			eventName = new FsmString();
		}

		public override void OnEnter()
		{
			EventRegister.SendEvent(eventName.Value);
			Finish();
		}
	}
}

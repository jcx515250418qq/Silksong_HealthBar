namespace HutongGames.PlayMaker.Actions
{
	[ActionCategory("Hollow Knight")]
	public class SendEventToRegisterV2 : FsmStateAction
	{
		public FsmString EventName;

		public FsmOwnerDefault ExcludeTarget;

		private int eventNameHash;

		public override void Reset()
		{
			EventName = new FsmString();
			ExcludeTarget = new FsmOwnerDefault();
		}

		public override void Awake()
		{
			eventNameHash = ((!EventName.UsesVariable) ? EventRegister.GetEventHashCode(EventName.Value) : 0);
		}

		public override void OnEnter()
		{
			if (eventNameHash != 0)
			{
				EventRegister.SendEvent(eventNameHash, ExcludeTarget.GetSafe(this));
			}
			else
			{
				EventRegister.SendEvent(EventName.Value, ExcludeTarget.GetSafe(this));
			}
			Finish();
		}
	}
}

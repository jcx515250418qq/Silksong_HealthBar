namespace HutongGames.PlayMaker.Actions
{
	public sealed class CanAddBlueHealth : FsmStateAction
	{
		public FsmEvent CanAddEvent;

		public FsmEvent CannotAddEvent;

		public FsmBool Consume;

		public override void Reset()
		{
			CanAddEvent = null;
			CannotAddEvent = null;
			Consume = true;
		}

		public override void OnEnter()
		{
			GameManager instance = GameManager.instance;
			if (instance != null)
			{
				if (instance.QueuedBlueHealth > 0)
				{
					base.Fsm.Event(CanAddEvent);
					if (Consume.Value)
					{
						instance.QueuedBlueHealth--;
					}
				}
				else
				{
					base.Fsm.Event(CannotAddEvent);
				}
			}
			Finish();
		}
	}
}

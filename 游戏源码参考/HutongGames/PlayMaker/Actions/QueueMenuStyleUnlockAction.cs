namespace HutongGames.PlayMaker.Actions
{
	[ActionCategory("Hollow Knight")]
	public class QueueMenuStyleUnlockAction : FsmStateAction
	{
		public FsmString unlockKey;

		public override void Reset()
		{
			unlockKey = null;
		}

		public override void OnEnter()
		{
			if (!string.IsNullOrEmpty(unlockKey.Value))
			{
				GameManager instance = GameManager.instance;
				if ((bool)instance)
				{
					instance.QueuedMenuStyleUnlock(unlockKey.Value);
				}
			}
			Finish();
		}
	}
}

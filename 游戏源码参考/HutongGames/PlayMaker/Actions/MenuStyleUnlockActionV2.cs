namespace HutongGames.PlayMaker.Actions
{
	[ActionCategory("Hollow Knight")]
	public class MenuStyleUnlockActionV2 : FsmStateAction
	{
		public FsmString UnlockKey;

		public FsmBool ForceChange;

		public override void Reset()
		{
			UnlockKey = null;
			ForceChange = true;
		}

		public override void OnEnter()
		{
			if (!string.IsNullOrEmpty(UnlockKey.Value))
			{
				MenuStyleUnlock.Unlock(UnlockKey.Value, ForceChange.Value);
			}
			Finish();
		}
	}
}

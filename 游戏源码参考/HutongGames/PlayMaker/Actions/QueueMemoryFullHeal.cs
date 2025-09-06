namespace HutongGames.PlayMaker.Actions
{
	public sealed class QueueMemoryFullHeal : FsmStateAction
	{
		public override void Reset()
		{
		}

		public override void OnEnter()
		{
			GameManager instance = GameManager.instance;
			PlayerData playerData = null;
			if (instance != null)
			{
				playerData = instance.playerData;
			}
			if (playerData != null)
			{
				playerData.PreMemoryState.DoFullHeal = true;
			}
			Finish();
		}
	}
}

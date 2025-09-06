namespace HutongGames.PlayMaker.Actions
{
	public class QueueAchievement : FsmStateAction
	{
		public FsmString Key;

		public override void Reset()
		{
			Key = null;
		}

		public override void OnEnter()
		{
			GameManager instance = GameManager.instance;
			if (!string.IsNullOrWhiteSpace(Key.Value))
			{
				instance.QueueAchievement(Key.Value);
			}
			Finish();
		}
	}
}

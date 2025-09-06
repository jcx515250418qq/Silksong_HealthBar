namespace HutongGames.PlayMaker.Actions
{
	public class AwardQueuedAchievements : FsmStateAction
	{
		public FsmFloat delay;

		public override void Reset()
		{
			delay = null;
		}

		public override void OnEnter()
		{
			GameManager instance = GameManager.instance;
			if (delay.Value > 0f)
			{
				instance.AwardQueuedAchievements(delay.Value);
			}
			else
			{
				instance.AwardQueuedAchievements();
			}
			Finish();
		}
	}
}

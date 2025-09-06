namespace HutongGames.PlayMaker.Actions
{
	public class UpdateHeartPieceAchievement : FsmStateAction
	{
		public override void Reset()
		{
		}

		public override void OnEnter()
		{
			GameManager.instance.CheckHeartAchievements();
			Finish();
		}
	}
}

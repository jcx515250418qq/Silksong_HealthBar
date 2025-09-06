namespace HutongGames.PlayMaker.Actions
{
	public class UpdateSilkSpoolAchievement : FsmStateAction
	{
		public override void Reset()
		{
		}

		public override void OnEnter()
		{
			GameManager.instance.CheckSilkSpoolAchievements();
			Finish();
		}
	}
}

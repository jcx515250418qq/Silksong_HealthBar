namespace HutongGames.PlayMaker.Actions
{
	public class AwardAchievementProgress : FsmStateAction
	{
		public FsmString Key;

		public FsmInt CurrentValue;

		public FsmInt MaxValue;

		public override void Reset()
		{
			Key = null;
			CurrentValue = new FsmInt
			{
				UseVariable = true
			};
			MaxValue = new FsmInt
			{
				UseVariable = true
			};
		}

		public override void OnEnter()
		{
			GameManager instance = GameManager.instance;
			if (!string.IsNullOrWhiteSpace(Key.Value))
			{
				if (CurrentValue.IsNone)
				{
					instance.AwardAchievement(Key.Value);
				}
				else
				{
					instance.UpdateAchievementProgress(Key.Value, CurrentValue.Value, MaxValue.Value);
				}
			}
			Finish();
		}
	}
}

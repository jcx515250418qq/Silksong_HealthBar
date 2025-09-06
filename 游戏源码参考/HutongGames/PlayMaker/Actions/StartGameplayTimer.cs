namespace HutongGames.PlayMaker.Actions
{
	public class StartGameplayTimer : FSMUtility.GetComponentFsmStateAction<GameplayTimer>
	{
		public FsmFloat Duration;

		public override void Reset()
		{
			base.Reset();
			Duration = null;
		}

		protected override void DoAction(GameplayTimer component)
		{
			component.StartTimer(Duration.Value);
		}
	}
}

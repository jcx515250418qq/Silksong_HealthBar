namespace HutongGames.PlayMaker.Actions
{
	public class SetSkipNextActorFadeIn : FsmStateAction
	{
		public FsmBool skipNextActorFadeIn;

		public override void Reset()
		{
			skipNextActorFadeIn = null;
		}

		public override void OnEnter()
		{
			GameManager instance = GameManager.instance;
			if (instance != null)
			{
				instance.SetSkipNextLevelReadyActorFadeIn(skipNextActorFadeIn.Value);
			}
			Finish();
		}
	}
}

namespace HutongGames.PlayMaker.Actions
{
	public sealed class ClearSpoolMossChunks : FsmStateAction
	{
		public override void OnEnter()
		{
			HeroController instance = HeroController.instance;
			if (instance != null)
			{
				instance.ClearSpoolMossChunks();
			}
			Finish();
		}
	}
}

namespace HutongGames.PlayMaker.Actions
{
	public class ControlNeedolin : FsmStateAction
	{
		public FsmOwnerDefault target;

		public FsmBool isPlaying;

		public override void Reset()
		{
			isPlaying = null;
		}

		public override void OnEnter()
		{
			HeroPerformanceRegion.IsPerforming = isPlaying.Value;
			Finish();
		}
	}
}

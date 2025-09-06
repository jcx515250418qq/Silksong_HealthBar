namespace HutongGames.PlayMaker.Actions
{
	public class ShakeAllGrass : FsmStateAction
	{
		private const string DeprecatedEffectName = "SHAKE ALL GRASS";

		public override void OnEnter()
		{
			base.OnEnter();
			PlayMakerFSM.BroadcastEvent("SHAKE ALL GRASS");
			Grass.PushAll();
			Finish();
		}
	}
}

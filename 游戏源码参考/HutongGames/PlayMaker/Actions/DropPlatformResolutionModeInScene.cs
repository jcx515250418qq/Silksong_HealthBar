namespace HutongGames.PlayMaker.Actions
{
	public class DropPlatformResolutionModeInScene : FsmStateAction
	{
		[ObjectType(typeof(Platform.ResolutionModes))]
		public FsmEnum ResolutionMode;

		public override void Reset()
		{
			ResolutionMode = null;
		}

		public override void OnEnter()
		{
			Platform.Current.DropResolutionModeInScene((Platform.ResolutionModes)(object)ResolutionMode.Value);
			Finish();
		}
	}
}

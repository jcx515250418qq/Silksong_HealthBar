namespace HutongGames.PlayMaker.Actions
{
	[ActionCategory(ActionCategory.Debug)]
	public class DebugPause : FsmStateAction
	{
		public override void OnEnter()
		{
			Finish();
		}
	}
}

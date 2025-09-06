namespace HutongGames.PlayMaker.Actions
{
	public class ToolsActiveStateControl : FsmStateAction
	{
		[ObjectType(typeof(ToolsActiveStates))]
		public FsmEnum SetActiveState;

		public override void Reset()
		{
			SetActiveState = null;
		}

		public override void OnEnter()
		{
			ToolItemManager.SetActiveState((ToolsActiveStates)(object)SetActiveState.Value);
			Finish();
		}
	}
}

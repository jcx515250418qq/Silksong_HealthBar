namespace HutongGames.PlayMaker.Actions
{
	public class ToolsActiveStateControlV2 : FsmStateAction
	{
		[ObjectType(typeof(ToolsActiveStates))]
		public FsmEnum SetActiveState;

		public FsmBool SkipAnims;

		public override void Reset()
		{
			SetActiveState = null;
			SkipAnims = null;
		}

		public override void OnEnter()
		{
			ToolItemManager.SetActiveState((ToolsActiveStates)(object)SetActiveState.Value, SkipAnims.Value);
			Finish();
		}
	}
}

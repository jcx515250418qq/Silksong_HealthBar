namespace HutongGames.PlayMaker.Actions
{
	public class ToolsCutsceneControl : FsmStateAction
	{
		public FsmBool SetInCutscene;

		public override void Reset()
		{
			SetInCutscene = null;
		}

		public override void OnEnter()
		{
			ToolItemManager.SetIsInCutscene(SetInCutscene.Value);
			Finish();
		}
	}
}

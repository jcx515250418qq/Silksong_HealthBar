namespace HutongGames.PlayMaker.Actions
{
	public class SendToolEquipChanged : FsmStateAction
	{
		public override void OnEnter()
		{
			ToolItemManager.SendEquippedChangedEvent();
			Finish();
		}
	}
}

namespace HutongGames.PlayMaker.Actions
{
	public class ToolItemStatesLiquidRefillRefills : FsmStateAction
	{
		[ObjectType(typeof(ToolItemStatesLiquid))]
		public FsmObject Tool;

		public override void Reset()
		{
			Tool = null;
		}

		public override void OnEnter()
		{
			ToolItemStatesLiquid toolItemStatesLiquid = Tool.Value as ToolItemStatesLiquid;
			if ((bool)toolItemStatesLiquid)
			{
				toolItemStatesLiquid.RefillRefills(showPopup: true);
			}
			Finish();
		}
	}
}

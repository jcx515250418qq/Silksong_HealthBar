namespace HutongGames.PlayMaker.Actions
{
	public class CustomToolUsage : FsmStateAction
	{
		[ObjectType(typeof(ToolItem))]
		public FsmObject Tool;

		public FsmInt UseAmount;

		public override void Reset()
		{
			Tool = null;
			UseAmount = 1;
		}

		public override void OnEnter()
		{
			ToolItem toolItem = Tool.Value as ToolItem;
			if (toolItem != null)
			{
				toolItem.CustomUsage(UseAmount.Value);
			}
			Finish();
		}
	}
}

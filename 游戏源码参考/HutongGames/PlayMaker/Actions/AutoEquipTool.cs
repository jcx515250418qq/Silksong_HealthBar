namespace HutongGames.PlayMaker.Actions
{
	public class AutoEquipTool : FsmStateAction
	{
		[ObjectType(typeof(ToolItem))]
		public FsmObject Tool;

		public override void Reset()
		{
			Tool = null;
		}

		public override void OnEnter()
		{
			if (!Tool.IsNone && (bool)Tool.Value)
			{
				ToolItemManager.AutoEquip((ToolItem)Tool.Value);
			}
			Finish();
		}
	}
}

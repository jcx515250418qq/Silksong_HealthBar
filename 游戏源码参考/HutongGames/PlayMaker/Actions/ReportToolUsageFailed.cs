namespace HutongGames.PlayMaker.Actions
{
	public class ReportToolUsageFailed : FsmStateAction
	{
		[ObjectType(typeof(ToolItem))]
		public FsmObject Tool;

		public override void Reset()
		{
			Tool = null;
		}

		public override void OnEnter()
		{
			ToolItem toolItem = Tool.Value as ToolItem;
			if (toolItem != null)
			{
				AttackToolBinding? attackToolBinding = ToolItemManager.GetAttackToolBinding(toolItem);
				if (attackToolBinding.HasValue)
				{
					ToolItemManager.ReportBoundAttackToolFailed(attackToolBinding.Value);
				}
			}
			Finish();
		}
	}
}

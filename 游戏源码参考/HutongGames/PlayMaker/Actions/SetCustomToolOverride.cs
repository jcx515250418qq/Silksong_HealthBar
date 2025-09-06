using TeamCherry.Localization;

namespace HutongGames.PlayMaker.Actions
{
	public class SetCustomToolOverride : FsmStateAction
	{
		[ObjectType(typeof(ToolItem))]
		public FsmObject Tool;

		[HideIf("IsToolNotSet")]
		public FsmInt Amount;

		[HideIf("IsToolNotSet")]
		public FsmString PromptTextSheet;

		[HideIf("IsToolNotSet")]
		public FsmString PromptTextKey;

		[HideIf("IsToolNotSet")]
		public FsmFloat PromptAppearDelay;

		public bool IsToolNotSet()
		{
			return !Tool.Value;
		}

		public override void Reset()
		{
			Tool = null;
			Amount = new FsmInt
			{
				UseVariable = true
			};
			PromptTextSheet = null;
			PromptTextKey = null;
			PromptAppearDelay = 1f;
		}

		public override void OnEnter()
		{
			ToolItem toolItem = Tool.Value as ToolItem;
			if ((bool)toolItem)
			{
				ToolItemManager.SetCustomToolOverride(promptText: new LocalisedString(PromptTextSheet.Value, PromptTextKey.Value), tool: toolItem, amount: Amount.IsNone ? ((int?)null) : new int?(Amount.Value), promptAppearDelay: PromptAppearDelay.Value);
			}
			else
			{
				ToolItemManager.ClearCustomToolOverride();
			}
			Finish();
		}
	}
}

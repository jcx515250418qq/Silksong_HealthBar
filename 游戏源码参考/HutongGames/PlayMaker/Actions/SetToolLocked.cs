namespace HutongGames.PlayMaker.Actions
{
	public class SetToolLocked : FsmStateAction
	{
		[ObjectType(typeof(ToolItem))]
		public FsmObject Tool;

		public FsmBool ShowPopup;

		public override void Reset()
		{
			Tool = null;
			ShowPopup = true;
		}

		public override void OnEnter()
		{
			if (!Tool.IsNone && (bool)Tool.Value)
			{
				ToolItem toolItem = (ToolItem)Tool.Value;
				toolItem.Lock();
				if (ShowPopup.Value)
				{
					CollectableUIMsg.ShowTakeMsg(toolItem, TakeItemTypes.Taken);
				}
			}
			Finish();
		}
	}
}

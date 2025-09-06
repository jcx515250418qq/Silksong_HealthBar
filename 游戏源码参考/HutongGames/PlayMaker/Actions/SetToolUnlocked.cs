namespace HutongGames.PlayMaker.Actions
{
	public class SetToolUnlocked : FsmStateAction
	{
		[ObjectType(typeof(ToolItem))]
		public FsmObject Tool;

		public FsmBool WaitForTutorialMsgEnd;

		public override void Reset()
		{
			Tool = null;
			WaitForTutorialMsgEnd = null;
		}

		public override void OnEnter()
		{
			if (!Tool.IsNone && (bool)Tool.Value)
			{
				ToolItem toolItem = (ToolItem)Tool.Value;
				if (WaitForTutorialMsgEnd.Value)
				{
					toolItem.Unlock(base.Finish);
					return;
				}
				toolItem.Unlock();
			}
			Finish();
		}
	}
}

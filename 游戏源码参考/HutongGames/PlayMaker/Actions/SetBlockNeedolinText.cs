namespace HutongGames.PlayMaker.Actions
{
	public class SetBlockNeedolinText : FsmStateAction
	{
		[RequiredField]
		public FsmBool IsBlocked;

		public override void Reset()
		{
			IsBlocked = null;
		}

		public override void OnEnter()
		{
			if (IsBlocked.Value)
			{
				NeedolinMsgBox.AddBlocker(base.Owner);
			}
			else
			{
				NeedolinMsgBox.RemoveBlocker(base.Owner);
			}
			Finish();
		}
	}
}

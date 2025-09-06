namespace HutongGames.PlayMaker.Actions
{
	public class BlockNeedolinTextInState : FsmStateAction
	{
		[RequiredField]
		public FsmBool IsBlocked;

		private bool wasBlocked;

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
			wasBlocked = IsBlocked.Value;
		}

		public override void OnUpdate()
		{
			if (IsBlocked.Value != wasBlocked)
			{
				if (IsBlocked.Value)
				{
					NeedolinMsgBox.AddBlocker(base.Owner);
				}
				else
				{
					NeedolinMsgBox.RemoveBlocker(base.Owner);
				}
				wasBlocked = IsBlocked.Value;
			}
		}

		public override void OnExit()
		{
			NeedolinMsgBox.RemoveBlocker(base.Owner);
		}
	}
}

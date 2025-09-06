namespace HutongGames.PlayMaker.Actions
{
	public class SetBitmaskAtIndex : FsmStateAction
	{
		public FsmInt ReadMask;

		public FsmInt Index;

		public FsmBool SetValue;

		[UIHint(UIHint.Variable)]
		public FsmInt WriteMask;

		public override void Reset()
		{
			ReadMask = null;
			Index = null;
			SetValue = null;
			WriteMask = null;
		}

		public override void OnEnter()
		{
			if (SetValue.Value)
			{
				WriteMask.Value |= 1 << Index.Value;
			}
			else
			{
				WriteMask.Value &= ~(1 << Index.Value);
			}
			Finish();
		}
	}
}

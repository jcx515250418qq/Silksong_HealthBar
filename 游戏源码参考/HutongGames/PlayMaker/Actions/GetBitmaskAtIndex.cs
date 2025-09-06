namespace HutongGames.PlayMaker.Actions
{
	public class GetBitmaskAtIndex : FsmStateAction
	{
		public FsmInt ReadMask;

		public FsmInt Index;

		[UIHint(UIHint.Variable)]
		public FsmBool StoreValue;

		public FsmEvent TrueEvent;

		public FsmEvent FalseEvent;

		public override void Reset()
		{
			ReadMask = null;
			Index = null;
			StoreValue = null;
			TrueEvent = null;
			FalseEvent = null;
		}

		public override void OnEnter()
		{
			int num = 1 << Index.Value;
			bool flag = (ReadMask.Value & num) == num;
			StoreValue.Value = flag;
			base.Fsm.Event(flag ? TrueEvent : FalseEvent);
			Finish();
		}
	}
}

namespace HutongGames.PlayMaker.Actions
{
	public class EnableFsmSelf : FsmStateAction
	{
		public FsmBool SetEnabled;

		public override void Reset()
		{
			SetEnabled = null;
		}

		public override void OnEnter()
		{
			if (!SetEnabled.IsNone)
			{
				base.Fsm.Owner.enabled = SetEnabled.Value;
			}
			Finish();
		}
	}
}

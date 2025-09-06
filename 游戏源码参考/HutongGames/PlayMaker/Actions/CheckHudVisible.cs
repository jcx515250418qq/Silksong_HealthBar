namespace HutongGames.PlayMaker.Actions
{
	[Tooltip("Checks Visible State Of Hud")]
	public sealed class CheckHudVisible : FsmStateAction
	{
		public FsmEvent trueEvent;

		public FsmEvent falseEvent;

		[UIHint(UIHint.Variable)]
		public FsmBool storeValue;

		public FsmBool everyFrame;

		public override void Reset()
		{
			trueEvent = null;
			falseEvent = null;
			storeValue = null;
			everyFrame = null;
		}

		public override void OnEnter()
		{
			if (HudCanvas.IsVisible)
			{
				base.Fsm.Event(trueEvent);
				storeValue.Value = true;
			}
			else
			{
				base.Fsm.Event(falseEvent);
				storeValue.Value = false;
			}
			if (!everyFrame.Value)
			{
				Finish();
			}
		}
	}
}

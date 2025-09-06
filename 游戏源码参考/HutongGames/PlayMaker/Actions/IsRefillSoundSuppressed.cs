namespace HutongGames.PlayMaker.Actions
{
	public class IsRefillSoundSuppressed : FsmStateAction
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
			HeroController instance = HeroController.instance;
			if (instance != null && instance.IsRefillSoundsSuppressed)
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

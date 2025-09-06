using JetBrains.Annotations;

namespace HutongGames.PlayMaker.Actions
{
	[UsedImplicitly]
	public class NextFixedFrameEvent : FsmStateAction
	{
		public FsmEvent SendEvent;

		public override void Reset()
		{
			SendEvent = null;
		}

		public override void OnPreprocess()
		{
			base.Fsm.HandleFixedUpdate = true;
		}

		public override void OnFixedUpdate()
		{
			Finish();
			base.Fsm.Event(SendEvent);
		}
	}
}

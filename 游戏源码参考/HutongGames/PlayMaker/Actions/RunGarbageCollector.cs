namespace HutongGames.PlayMaker.Actions
{
	[ActionCategory("System")]
	[Tooltip("Tell the Garbage Collector to run.")]
	public class RunGarbageCollector : FsmStateAction
	{
		public FsmEvent finishEvent;

		public override void Reset()
		{
			finishEvent = null;
		}

		public override void OnEnter()
		{
			GCManager.Collect();
			if (finishEvent != null)
			{
				base.Fsm.Event(finishEvent);
			}
			Finish();
		}
	}
}

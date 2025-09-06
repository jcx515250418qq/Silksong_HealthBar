using System;

namespace HutongGames.PlayMaker.Actions
{
	public class WaitForSceneAudioSnapshotsApplied : FsmStateAction
	{
		public FsmEvent SendEvent;

		public override void Reset()
		{
			SendEvent = null;
		}

		public override void OnEnter()
		{
			GameManager instance = GameManager.instance;
			if ((bool)instance)
			{
				CustomSceneManager sm = instance.sm;
				if (!sm.IsAudioSnapshotsApplied)
				{
					Action temp = null;
					temp = delegate
					{
						base.Fsm.Event(SendEvent);
						sm.AudioSnapshotsApplied -= temp;
						Finish();
					};
					sm.AudioSnapshotsApplied += temp;
					return;
				}
			}
			base.Fsm.Event(SendEvent);
			Finish();
		}
	}
}

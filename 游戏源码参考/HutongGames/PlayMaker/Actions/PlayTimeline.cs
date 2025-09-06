using UnityEngine;
using UnityEngine.Playables;

namespace HutongGames.PlayMaker.Actions
{
	public class PlayTimeline : FsmStateAction
	{
		public FsmOwnerDefault Target;

		public FsmEvent FinishEvent;

		private PlayableDirector timeline;

		public override void Reset()
		{
			Target = null;
			FinishEvent = null;
		}

		public override void OnEnter()
		{
			timeline = null;
			GameObject safe = Target.GetSafe(this);
			if ((bool)safe)
			{
				timeline = safe.GetComponent<PlayableDirector>();
				timeline.time = 0.0;
				timeline.Play();
			}
			else
			{
				Finish();
			}
		}

		public override void OnUpdate()
		{
			if (timeline.time >= timeline.duration)
			{
				End();
			}
		}

		private void End()
		{
			base.Fsm.Event(FinishEvent);
			Finish();
		}
	}
}

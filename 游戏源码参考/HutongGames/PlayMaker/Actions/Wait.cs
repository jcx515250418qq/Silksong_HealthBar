using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
	[ActionCategory(ActionCategory.Time)]
	[Tooltip("Delays a State from finishing. Optionally send an event after the specified time. NOTE: Other actions continue running and can send events before this action finishes.")]
	public class Wait : FsmStateAction
	{
		[RequiredField]
		[Tooltip("Time to wait in seconds.")]
		public FsmFloat time;

		[Tooltip("Event to send after the specified time.")]
		public FsmEvent finishEvent;

		[Tooltip("Ignore TimeScale. E.g., if the game is paused using Scale Time.")]
		public bool realTime;

		private float startTime;

		private float timer;

		public override void Reset()
		{
			time = 1f;
			finishEvent = null;
			realTime = false;
		}

		public override void OnEnter()
		{
			if (time.Value <= 0f)
			{
				base.Fsm.Event(finishEvent);
				Finish();
			}
			else
			{
				startTime = FsmTime.RealtimeSinceStartup;
				timer = 0f;
			}
		}

		public override void OnUpdate()
		{
			if (realTime)
			{
				timer = FsmTime.RealtimeSinceStartup - startTime;
			}
			else
			{
				timer += Time.deltaTime;
			}
			if (timer >= time.Value)
			{
				Finish();
				if (finishEvent != null)
				{
					base.Fsm.Event(finishEvent);
				}
			}
		}
	}
}

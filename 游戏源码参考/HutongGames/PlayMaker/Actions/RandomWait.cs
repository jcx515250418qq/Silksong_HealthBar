using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
	[ActionCategory(ActionCategory.Time)]
	[Tooltip("Delays a State from finishing by a random time. NOTE: Other actions continue running during this time.\nAfter the random time the specified Finish Event or FINISHED is sent.")]
	public class RandomWait : FsmStateAction
	{
		[RequiredField]
		[Tooltip("Minimum amount of time to wait.")]
		public FsmFloat min;

		[RequiredField]
		[Tooltip("Maximum amount of time to wait.")]
		public FsmFloat max;

		[Tooltip("Event to send when timer is finished.")]
		public FsmEvent finishEvent;

		[Tooltip("Ignore time scale.")]
		public bool realTime;

		private float startTime;

		private float timer;

		private float time;

		public override void Reset()
		{
			min = 0f;
			max = 1f;
			finishEvent = null;
			realTime = false;
		}

		public override void OnEnter()
		{
			time = Random.Range(min.Value, max.Value);
			if (time <= 0f)
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
			if (timer >= time)
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

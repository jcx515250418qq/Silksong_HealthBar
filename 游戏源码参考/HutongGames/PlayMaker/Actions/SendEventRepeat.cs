using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
	[ActionCategory(ActionCategory.StateMachine)]
	public class SendEventRepeat : FsmStateAction
	{
		[Tooltip("Where to send the event.")]
		public FsmEventTarget eventTarget;

		[RequiredField]
		[Tooltip("The event to send. NOTE: Events must be marked Global to send between FSMs.")]
		public FsmString sendEvent;

		public FsmFloat repeatTime;

		public bool sendEventOnEntry;

		private float timer;

		public override void Reset()
		{
			eventTarget = null;
			sendEvent = null;
			repeatTime = null;
			sendEventOnEntry = false;
		}

		public override void OnEnter()
		{
			if (sendEventOnEntry)
			{
				base.Fsm.Event(eventTarget, sendEvent.Value);
			}
			timer = 0f;
		}

		public override void OnUpdate()
		{
			if (timer < repeatTime.Value)
			{
				timer += Time.deltaTime;
				return;
			}
			base.Fsm.Event(eventTarget, sendEvent.Value);
			timer -= repeatTime.Value;
		}
	}
}

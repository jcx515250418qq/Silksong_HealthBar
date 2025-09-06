using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
	[ActionCategory("Hollow Knight")]
	public class TriggerEnterEventSubscribe : FsmStateAction
	{
		[ObjectType(typeof(TriggerEnterEvent))]
		public FsmObject trigger;

		public FsmEvent triggerEnteredEvent;

		public FsmEvent triggerExitedEvent;

		public FsmEvent triggerStayedEvent;

		private bool subbedStayEvent;

		public override void Reset()
		{
			trigger = null;
			triggerEnteredEvent = null;
			triggerExitedEvent = null;
			triggerStayedEvent = null;
		}

		public override void OnEnter()
		{
			if (!trigger.IsNone)
			{
				TriggerEnterEvent triggerEnterEvent = (TriggerEnterEvent)trigger.Value;
				triggerEnterEvent.OnTriggerEntered += SendEnteredEvent;
				triggerEnterEvent.OnTriggerExited += SendExitedEvent;
				if (triggerStayedEvent != null && !string.IsNullOrEmpty(triggerStayedEvent.Name))
				{
					subbedStayEvent = true;
					triggerEnterEvent.OnTriggerStayed += SendStayedEvent;
				}
			}
			Finish();
		}

		public override void OnExit()
		{
			if (!trigger.IsNone)
			{
				TriggerEnterEvent triggerEnterEvent = (TriggerEnterEvent)trigger.Value;
				triggerEnterEvent.OnTriggerEntered -= SendEnteredEvent;
				triggerEnterEvent.OnTriggerExited -= SendExitedEvent;
				if (subbedStayEvent)
				{
					subbedStayEvent = false;
					triggerEnterEvent.OnTriggerStayed -= SendStayedEvent;
				}
			}
		}

		private void SendEnteredEvent(Collider2D collider, GameObject sender)
		{
			base.Fsm.Event(triggerEnteredEvent);
		}

		private void SendExitedEvent(Collider2D collider, GameObject sender)
		{
			base.Fsm.Event(triggerExitedEvent);
		}

		private void SendStayedEvent(Collider2D collider, GameObject sender)
		{
			base.Fsm.Event(triggerStayedEvent);
		}
	}
}

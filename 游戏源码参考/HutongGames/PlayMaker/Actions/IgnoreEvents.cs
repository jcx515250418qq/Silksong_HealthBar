using System;

namespace HutongGames.PlayMaker.Actions
{
	[ActionCategory(ActionCategory.StateMachine)]
	[Tooltip("Ignore specified events while this action is active.")]
	public class IgnoreEvents : FsmStateAction
	{
		[Serializable]
		public enum EventType
		{
			mouse = 0,
			application = 1,
			collision = 2,
			collision2d = 3,
			trigger = 4,
			trigger2d = 5,
			UI = 6,
			anyUnityEvent = 7
		}

		[Tooltip("Type of events to ignore.")]
		public EventType[] eventTypes;

		[Tooltip("Event names to ignore.")]
		[UIHint(UIHint.FsmEvent)]
		public FsmString[] events;

		[ActionSection("Debug")]
		[Tooltip("Log any events blocked by this action. Helpful for debugging.")]
		public FsmBool logIgnoredEvents;

		public override void Reset()
		{
			eventTypes = new EventType[0];
			events = new FsmString[0];
			logIgnoredEvents = false;
		}

		public override void Awake()
		{
			base.HandlesOnEvent = true;
			base.BlocksFinish = false;
		}

		public override bool Event(FsmEvent fsmEvent)
		{
			bool num = DoIgnoreEvent(fsmEvent);
			if (num && logIgnoredEvents.Value)
			{
				ActionHelpers.DebugLog(base.Fsm, LogLevel.Info, "Ignored: " + fsmEvent.Name, sendToUnityLog: true);
			}
			return num;
		}

		private bool DoIgnoreEvent(FsmEvent fsmEvent)
		{
			if (fsmEvent == null)
			{
				return false;
			}
			EventType[] array = eventTypes;
			for (int i = 0; i < array.Length; i++)
			{
				switch (array[i])
				{
				case EventType.anyUnityEvent:
					if (fsmEvent.IsUnityEvent)
					{
						return true;
					}
					break;
				case EventType.mouse:
					if (fsmEvent.IsMouseEvent)
					{
						return true;
					}
					break;
				case EventType.application:
					if (fsmEvent.IsApplicationEvent)
					{
						return true;
					}
					break;
				case EventType.collision:
					if (fsmEvent.IsCollisionEvent)
					{
						return true;
					}
					break;
				case EventType.collision2d:
					if (fsmEvent.IsCollision2DEvent)
					{
						return true;
					}
					break;
				case EventType.trigger:
					if (fsmEvent.IsTriggerEvent)
					{
						return true;
					}
					break;
				case EventType.trigger2d:
					if (fsmEvent.IsTrigger2DEvent)
					{
						return true;
					}
					break;
				case EventType.UI:
					if (fsmEvent.IsUIEvent)
					{
						return true;
					}
					break;
				default:
					throw new ArgumentOutOfRangeException();
				}
			}
			string text = fsmEvent.Name;
			for (int j = 0; j < events.Length; j++)
			{
				if (events[j].Value == text)
				{
					return true;
				}
			}
			return false;
		}
	}
}

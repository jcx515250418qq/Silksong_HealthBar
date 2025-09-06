using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.Events;

namespace HutongGames.PlayMaker.Actions
{
	public abstract class EventTriggerActionBase : ComponentAction<EventTrigger>
	{
		[DisplayOrder(0)]
		[RequiredField]
		[Tooltip("The GameObject with the UI component.")]
		public FsmOwnerDefault gameObject;

		[DisplayOrder(1)]
		[Tooltip("Where to send the event.")]
		public FsmEventTarget eventTarget;

		protected EventTrigger trigger;

		protected EventTrigger.Entry entry;

		public override void Reset()
		{
			gameObject = null;
			eventTarget = null;
		}

		protected void Init(EventTriggerType eventTriggerType, UnityAction<BaseEventData> call)
		{
			GameObject ownerDefaultTarget = base.Fsm.GetOwnerDefaultTarget(gameObject);
			if (UpdateCacheAddComponent(ownerDefaultTarget))
			{
				trigger = cachedComponent;
				if (entry == null)
				{
					entry = new EventTrigger.Entry();
				}
				entry.eventID = eventTriggerType;
				entry.callback.AddListener(call);
				trigger.triggers.Add(entry);
			}
		}

		public override void OnExit()
		{
			if (entry != null && entry.callback != null)
			{
				entry.callback.RemoveAllListeners();
			}
			if (trigger != null && trigger.triggers != null)
			{
				trigger.triggers.Remove(entry);
			}
		}
	}
}

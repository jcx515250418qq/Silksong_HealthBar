using UnityEngine.EventSystems;

namespace HutongGames.PlayMaker.Actions
{
	[ActionCategory(ActionCategory.UI)]
	[Tooltip("Sends event when Called by the EventSystem when a Select event occurs. \nUse GetLastPointerDataInfo action to get info from the event")]
	public class UiOnSelectEvent : EventTriggerActionBase
	{
		[UIHint(UIHint.Variable)]
		[Tooltip("Event sent when OnSelect is called")]
		public FsmEvent onSelectEvent;

		public override void Reset()
		{
			base.Reset();
			onSelectEvent = null;
		}

		public override void OnEnter()
		{
			Init(EventTriggerType.Select, OnSelectDelegate);
		}

		private void OnSelectDelegate(BaseEventData data)
		{
			UiGetLastPointerDataInfo.lastPointerEventData = (PointerEventData)data;
			SendEvent(eventTarget, onSelectEvent);
		}
	}
}

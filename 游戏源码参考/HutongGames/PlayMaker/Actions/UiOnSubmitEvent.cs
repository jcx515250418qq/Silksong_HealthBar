using UnityEngine.EventSystems;

namespace HutongGames.PlayMaker.Actions
{
	[ActionCategory(ActionCategory.UI)]
	[Tooltip("Sends event when OnSubmit is called on the GameObject. \nUse GetLastPointerDataInfo action to get info from the event")]
	public class UiOnSubmitEvent : EventTriggerActionBase
	{
		[UIHint(UIHint.Variable)]
		[Tooltip("Event sent when OnSubmitEvent is called")]
		public FsmEvent onSubmitEvent;

		public override void Reset()
		{
			base.Reset();
			onSubmitEvent = null;
		}

		public override void OnEnter()
		{
			Init(EventTriggerType.Submit, OnSubmitDelegate);
		}

		private void OnSubmitDelegate(BaseEventData data)
		{
			UiGetLastPointerDataInfo.lastPointerEventData = (PointerEventData)data;
			SendEvent(eventTarget, onSubmitEvent);
		}
	}
}

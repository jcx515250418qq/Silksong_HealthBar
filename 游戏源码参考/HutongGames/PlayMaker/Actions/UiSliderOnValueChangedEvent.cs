using UnityEngine;
using UnityEngine.UI;

namespace HutongGames.PlayMaker.Actions
{
	[ActionCategory(ActionCategory.UI)]
	[Tooltip("Catches onValueChanged event for a UI Slider component. Store the new value and/or send events. Event float data will contain the new slider value")]
	public class UiSliderOnValueChangedEvent : ComponentAction<Slider>
	{
		[RequiredField]
		[CheckForComponent(typeof(Slider))]
		[Tooltip("The GameObject with the UI Slider component.")]
		public FsmOwnerDefault gameObject;

		[Tooltip("Where to send the event.")]
		public FsmEventTarget eventTarget;

		[Tooltip("Send this event when Clicked.")]
		public FsmEvent sendEvent;

		[Tooltip("Store the new value in float variable.")]
		[UIHint(UIHint.Variable)]
		public FsmFloat value;

		private Slider slider;

		public override void Reset()
		{
			gameObject = null;
			eventTarget = null;
			sendEvent = null;
			value = null;
		}

		public override void OnEnter()
		{
			GameObject ownerDefaultTarget = base.Fsm.GetOwnerDefaultTarget(gameObject);
			if (UpdateCache(ownerDefaultTarget))
			{
				slider = cachedComponent;
				if (slider != null)
				{
					slider.onValueChanged.AddListener(DoOnValueChanged);
				}
			}
		}

		public override void OnExit()
		{
			if (slider != null)
			{
				slider.onValueChanged.RemoveListener(DoOnValueChanged);
			}
		}

		public void DoOnValueChanged(float _value)
		{
			value.Value = _value;
			Fsm.EventData.FloatData = _value;
			SendEvent(eventTarget, sendEvent);
		}
	}
}

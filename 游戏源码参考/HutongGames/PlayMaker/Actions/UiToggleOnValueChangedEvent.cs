using UnityEngine;
using UnityEngine.UI;

namespace HutongGames.PlayMaker.Actions
{
	[ActionCategory(ActionCategory.UI)]
	[Tooltip("Catches onValueChanged event in a UI Toggle component. Store the new value and/or send events. Event bool data will contain the new Toggle value")]
	public class UiToggleOnValueChangedEvent : ComponentAction<Toggle>
	{
		[RequiredField]
		[CheckForComponent(typeof(Toggle))]
		[Tooltip("The GameObject with the UI Toggle component.")]
		public FsmOwnerDefault gameObject;

		[Tooltip("Where to send the event.")]
		public FsmEventTarget eventTarget;

		[Tooltip("Send this event when the value changes.")]
		public FsmEvent sendEvent;

		[Tooltip("Store the new value in bool variable.")]
		[UIHint(UIHint.Variable)]
		public FsmBool value;

		private Toggle toggle;

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
				if (toggle != null)
				{
					toggle.onValueChanged.RemoveListener(DoOnValueChanged);
				}
				toggle = cachedComponent;
				if (toggle != null)
				{
					toggle.onValueChanged.AddListener(DoOnValueChanged);
				}
				else
				{
					LogError("Missing UI.Toggle on " + ownerDefaultTarget.name);
				}
			}
			else
			{
				LogError("Missing GameObject");
			}
		}

		public override void OnExit()
		{
			if (toggle != null)
			{
				toggle.onValueChanged.RemoveListener(DoOnValueChanged);
			}
		}

		public void DoOnValueChanged(bool _value)
		{
			value.Value = _value;
			Fsm.EventData.BoolData = _value;
			SendEvent(eventTarget, sendEvent);
		}
	}
}

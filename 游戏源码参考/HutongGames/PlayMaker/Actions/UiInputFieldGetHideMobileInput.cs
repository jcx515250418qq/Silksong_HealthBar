using UnityEngine;
using UnityEngine.UI;

namespace HutongGames.PlayMaker.Actions
{
	[ActionCategory(ActionCategory.UI)]
	[Tooltip("Gets the Hide Mobile Input value of a UI InputField component.")]
	public class UiInputFieldGetHideMobileInput : ComponentAction<InputField>
	{
		[RequiredField]
		[CheckForComponent(typeof(InputField))]
		[Tooltip("The GameObject with the UI InputField component.")]
		public FsmOwnerDefault gameObject;

		[UIHint(UIHint.Variable)]
		[Tooltip("Store the Hide Mobile flag value of the UI InputField component.")]
		public FsmBool hideMobileInput;

		[Tooltip("Event sent if hide mobile input property is true")]
		public FsmEvent mobileInputHiddenEvent;

		[Tooltip("Event sent if hide mobile input property is false")]
		public FsmEvent mobileInputShownEvent;

		private InputField inputField;

		public override void Reset()
		{
			hideMobileInput = null;
			mobileInputHiddenEvent = null;
			mobileInputShownEvent = null;
		}

		public override void OnEnter()
		{
			GameObject ownerDefaultTarget = base.Fsm.GetOwnerDefaultTarget(gameObject);
			if (UpdateCache(ownerDefaultTarget))
			{
				inputField = cachedComponent;
			}
			DoGetValue();
			Finish();
		}

		private void DoGetValue()
		{
			if (!(inputField == null))
			{
				hideMobileInput.Value = inputField.shouldHideMobileInput;
				base.Fsm.Event(inputField.shouldHideMobileInput ? mobileInputHiddenEvent : mobileInputShownEvent);
			}
		}
	}
}

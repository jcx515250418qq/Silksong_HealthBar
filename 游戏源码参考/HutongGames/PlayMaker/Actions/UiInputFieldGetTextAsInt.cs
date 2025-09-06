using UnityEngine;
using UnityEngine.UI;

namespace HutongGames.PlayMaker.Actions
{
	[ActionCategory(ActionCategory.UI)]
	[Tooltip("Gets the text value of a UI InputField component as an Int.")]
	public class UiInputFieldGetTextAsInt : ComponentAction<InputField>
	{
		[RequiredField]
		[CheckForComponent(typeof(InputField))]
		[Tooltip("The GameObject with the UI InputField component.")]
		public FsmOwnerDefault gameObject;

		[RequiredField]
		[UIHint(UIHint.Variable)]
		[Tooltip("Store the text value as an int.")]
		public FsmInt value;

		[UIHint(UIHint.Variable)]
		[Tooltip("True if text resolves to an int")]
		public FsmBool isInt;

		[Tooltip("Event to send if text resolves to an int")]
		public FsmEvent isIntEvent;

		[Tooltip("Event to send if text does NOT resolve to an int")]
		public FsmEvent isNotIntEvent;

		[Tooltip("Repeat every frame.")]
		public bool everyFrame;

		private InputField inputField;

		private int _value;

		private bool _success;

		public override void Reset()
		{
			value = null;
			isInt = null;
			isIntEvent = null;
			isNotIntEvent = null;
			everyFrame = false;
		}

		public override void OnEnter()
		{
			GameObject ownerDefaultTarget = base.Fsm.GetOwnerDefaultTarget(gameObject);
			if (UpdateCache(ownerDefaultTarget))
			{
				inputField = cachedComponent;
			}
			DoGetTextValue();
			if (!everyFrame)
			{
				Finish();
			}
		}

		public override void OnUpdate()
		{
			DoGetTextValue();
		}

		private void DoGetTextValue()
		{
			if (!(inputField == null))
			{
				_success = int.TryParse(inputField.text, out _value);
				value.Value = _value;
				isInt.Value = _success;
				base.Fsm.Event(_success ? isIntEvent : isNotIntEvent);
			}
		}
	}
}

using UnityEngine;
using UnityEngine.UI;

namespace HutongGames.PlayMaker.Actions
{
	[ActionCategory(ActionCategory.UI)]
	[Tooltip("Sets the caret's blink rate of a UI InputField component.")]
	public class UiInputFieldSetCaretBlinkRate : ComponentAction<InputField>
	{
		[RequiredField]
		[CheckForComponent(typeof(InputField))]
		[Tooltip("The GameObject with the UI InputField component.")]
		public FsmOwnerDefault gameObject;

		[RequiredField]
		[Tooltip("The caret's blink rate for the UI InputField component.")]
		public FsmInt caretBlinkRate;

		[Tooltip("Deactivate when exiting this state.")]
		public FsmBool resetOnExit;

		[Tooltip("Repeats every frame")]
		public bool everyFrame;

		private InputField inputField;

		private float originalValue;

		public override void Reset()
		{
			gameObject = null;
			caretBlinkRate = null;
			resetOnExit = null;
			everyFrame = false;
		}

		public override void OnEnter()
		{
			GameObject ownerDefaultTarget = base.Fsm.GetOwnerDefaultTarget(gameObject);
			if (UpdateCache(ownerDefaultTarget))
			{
				inputField = cachedComponent;
			}
			originalValue = inputField.caretBlinkRate;
			DoSetValue();
			if (!everyFrame)
			{
				Finish();
			}
		}

		public override void OnUpdate()
		{
			DoSetValue();
		}

		private void DoSetValue()
		{
			if (inputField != null)
			{
				inputField.caretBlinkRate = caretBlinkRate.Value;
			}
		}

		public override void OnExit()
		{
			if (!(inputField == null) && resetOnExit.Value)
			{
				inputField.caretBlinkRate = originalValue;
			}
		}
	}
}

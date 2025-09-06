using UnityEngine;
using UnityEngine.UI;

namespace HutongGames.PlayMaker.Actions
{
	[ActionCategory(ActionCategory.UI)]
	[Tooltip("Sets the Asterix Character of a UI InputField component.")]
	public class UiInputFieldSetAsterixChar : ComponentAction<InputField>
	{
		[RequiredField]
		[CheckForComponent(typeof(InputField))]
		[Tooltip("The GameObject with the UI InputField component.")]
		public FsmOwnerDefault gameObject;

		[RequiredField]
		[Tooltip("The asterix Character used for password field type of the UI InputField component. Only the first character will be used, the rest of the string will be ignored")]
		public FsmString asterixChar;

		[Tooltip("Reset when exiting this state.")]
		public FsmBool resetOnExit;

		private InputField inputField;

		private char originalValue;

		private static char __char__ = ' ';

		public override void Reset()
		{
			gameObject = null;
			asterixChar = "*";
			resetOnExit = null;
		}

		public override void OnEnter()
		{
			GameObject ownerDefaultTarget = base.Fsm.GetOwnerDefaultTarget(gameObject);
			if (UpdateCache(ownerDefaultTarget))
			{
				inputField = cachedComponent;
			}
			originalValue = inputField.asteriskChar;
			DoSetValue();
			Finish();
		}

		private void DoSetValue()
		{
			char asteriskChar = __char__;
			if (asterixChar.Value.Length > 0)
			{
				asteriskChar = asterixChar.Value[0];
			}
			if (inputField != null)
			{
				inputField.asteriskChar = asteriskChar;
			}
		}

		public override void OnExit()
		{
			if (!(inputField == null) && resetOnExit.Value)
			{
				inputField.asteriskChar = originalValue;
			}
		}
	}
}

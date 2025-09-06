using UnityEngine;
using UnityEngine.UI;

namespace HutongGames.PlayMaker.Actions
{
	[ActionCategory(ActionCategory.UI)]
	[Tooltip("Deactivate a UI InputField to stop the processing of Events and send OnSubmit if not canceled. Optionally Activate on state exit")]
	public class UiInputFieldDeactivate : ComponentAction<InputField>
	{
		[RequiredField]
		[CheckForComponent(typeof(InputField))]
		[Tooltip("The GameObject with the UI InputField component.")]
		public FsmOwnerDefault gameObject;

		[Tooltip("Activate when exiting this state.")]
		public FsmBool activateOnExit;

		private InputField inputField;

		public override void Reset()
		{
			gameObject = null;
			activateOnExit = null;
		}

		public override void OnEnter()
		{
			GameObject ownerDefaultTarget = base.Fsm.GetOwnerDefaultTarget(gameObject);
			if (UpdateCache(ownerDefaultTarget))
			{
				inputField = cachedComponent;
			}
			DoAction();
			Finish();
		}

		private void DoAction()
		{
			if (inputField != null)
			{
				inputField.DeactivateInputField();
			}
		}

		public override void OnExit()
		{
			if (!(inputField == null) && activateOnExit.Value)
			{
				inputField.ActivateInputField();
			}
		}
	}
}

using UnityEngine;
using UnityEngine.UI;

namespace HutongGames.PlayMaker.Actions
{
	[ActionCategory(ActionCategory.UI)]
	[Tooltip("Activate a UI InputField component to begin processing Events. Optionally Deactivate on state exit")]
	public class UiInputFieldActivate : ComponentAction<InputField>
	{
		[RequiredField]
		[CheckForComponent(typeof(InputField))]
		[Tooltip("The GameObject with the UI InputField component.")]
		public FsmOwnerDefault gameObject;

		[Tooltip("Reset when exiting this state.")]
		public FsmBool deactivateOnExit;

		private InputField inputField;

		public override void Reset()
		{
			gameObject = null;
			deactivateOnExit = null;
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
				inputField.ActivateInputField();
			}
		}

		public override void OnExit()
		{
			if (!(inputField == null) && deactivateOnExit.Value)
			{
				inputField.DeactivateInputField();
			}
		}
	}
}

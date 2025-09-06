using UnityEngine;
using UnityEngine.UI;

namespace HutongGames.PlayMaker.Actions
{
	[ActionCategory(ActionCategory.UI)]
	[Tooltip("Sets the placeholder of a UI InputField component. Optionally reset on exit")]
	public class UiInputFieldSetPlaceHolder : ComponentAction<InputField>
	{
		[RequiredField]
		[CheckForComponent(typeof(InputField))]
		[Tooltip("The GameObject with the UI InputField component.")]
		public FsmOwnerDefault gameObject;

		[RequiredField]
		[CheckForComponent(typeof(Graphic))]
		[Tooltip("The placeholder (any graphic UI Component) for the UI InputField component.")]
		public FsmGameObject placeholder;

		[Tooltip("Reset when exiting this state.")]
		public FsmBool resetOnExit;

		private InputField inputField;

		private Graphic originalValue;

		public override void Reset()
		{
			gameObject = null;
			placeholder = null;
			resetOnExit = null;
		}

		public override void OnEnter()
		{
			GameObject ownerDefaultTarget = base.Fsm.GetOwnerDefaultTarget(gameObject);
			if (UpdateCache(ownerDefaultTarget))
			{
				inputField = cachedComponent;
			}
			originalValue = inputField.placeholder;
			DoSetValue();
			Finish();
		}

		private void DoSetValue()
		{
			if (inputField != null)
			{
				GameObject value = placeholder.Value;
				if (value == null)
				{
					inputField.placeholder = null;
				}
				else
				{
					inputField.placeholder = value.GetComponent<Graphic>();
				}
			}
		}

		public override void OnExit()
		{
			if (!(inputField == null) && resetOnExit.Value)
			{
				inputField.placeholder = originalValue;
			}
		}
	}
}

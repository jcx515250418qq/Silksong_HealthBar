using UnityEngine;
using UnityEngine.UI;

namespace HutongGames.PlayMaker.Actions
{
	[ActionCategory(ActionCategory.UI)]
	[Tooltip("Gets the placeHolder GameObject of a UI InputField component.")]
	public class UiInputFieldGetPlaceHolder : ComponentAction<InputField>
	{
		[RequiredField]
		[CheckForComponent(typeof(InputField))]
		[Tooltip("The GameObject with the UI InputField component.")]
		public FsmOwnerDefault gameObject;

		[UIHint(UIHint.Variable)]
		[Tooltip("Store the placeholder for the UI InputField component.")]
		public FsmGameObject placeHolder;

		[Tooltip("true if placeholder is found")]
		public FsmBool placeHolderDefined;

		[Tooltip("Event sent if no placeholder is defined")]
		public FsmEvent foundEvent;

		[Tooltip("Event sent if a placeholder is defined")]
		public FsmEvent notFoundEvent;

		private InputField inputField;

		public override void Reset()
		{
			placeHolder = null;
			placeHolderDefined = null;
			foundEvent = null;
			notFoundEvent = null;
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
				Graphic placeholder = inputField.placeholder;
				placeHolderDefined.Value = placeholder != null;
				if (placeholder != null)
				{
					placeHolder.Value = placeholder.gameObject;
					base.Fsm.Event(foundEvent);
				}
				else
				{
					base.Fsm.Event(notFoundEvent);
				}
			}
		}
	}
}

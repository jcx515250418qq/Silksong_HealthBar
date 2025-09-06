using UnityEngine;
using UnityEngine.UI;

namespace HutongGames.PlayMaker.Actions
{
	[ActionCategory(ActionCategory.UI)]
	[Tooltip("Sets the interactable flag of a UI Selectable component.")]
	public class UiSetIsInteractable : FsmStateAction
	{
		[RequiredField]
		[CheckForComponent(typeof(Selectable))]
		[Tooltip("The GameObject with the UI Selectable component.")]
		public FsmOwnerDefault gameObject;

		[Tooltip("The Interactable value")]
		public FsmBool isInteractable;

		[Tooltip("Reset when exiting this state.")]
		public FsmBool resetOnExit;

		private Selectable _selectable;

		private bool _originalState;

		public override void Reset()
		{
			gameObject = null;
			isInteractable = null;
			resetOnExit = false;
		}

		public override void OnEnter()
		{
			GameObject ownerDefaultTarget = base.Fsm.GetOwnerDefaultTarget(gameObject);
			if (ownerDefaultTarget != null)
			{
				_selectable = ownerDefaultTarget.GetComponent<Selectable>();
			}
			if (_selectable != null && resetOnExit.Value)
			{
				_originalState = _selectable.IsInteractable();
			}
			DoSetValue();
			Finish();
		}

		private void DoSetValue()
		{
			if (_selectable != null)
			{
				_selectable.interactable = isInteractable.Value;
			}
		}

		public override void OnExit()
		{
			if (!(_selectable == null) && resetOnExit.Value)
			{
				_selectable.interactable = _originalState;
			}
		}
	}
}

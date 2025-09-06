using UnityEngine;
using UnityEngine.UI;

namespace HutongGames.PlayMaker.Actions
{
	[ActionCategory(ActionCategory.UI)]
	[Tooltip("Gets the interactable flag of a UI Selectable component.")]
	public class UiGetIsInteractable : ComponentAction<Selectable>
	{
		[RequiredField]
		[CheckForComponent(typeof(Selectable))]
		[Tooltip("The GameObject with the UI Selectable component.")]
		public FsmOwnerDefault gameObject;

		[Tooltip("The Interactable value")]
		[UIHint(UIHint.Variable)]
		public FsmBool isInteractable;

		[Tooltip("Event sent if Component is Interactable")]
		public FsmEvent isInteractableEvent;

		[Tooltip("Event sent if Component is not Interactable")]
		public FsmEvent isNotInteractableEvent;

		private Selectable selectable;

		public override void Reset()
		{
			gameObject = null;
			isInteractable = null;
			isInteractableEvent = null;
			isNotInteractableEvent = null;
		}

		public override void OnEnter()
		{
			GameObject ownerDefaultTarget = base.Fsm.GetOwnerDefaultTarget(gameObject);
			if (UpdateCache(ownerDefaultTarget))
			{
				selectable = cachedComponent;
			}
			DoGetValue();
			Finish();
		}

		private void DoGetValue()
		{
			if (!(selectable == null))
			{
				bool flag = selectable.IsInteractable();
				isInteractable.Value = flag;
				base.Fsm.Event(flag ? isInteractableEvent : isNotInteractableEvent);
			}
		}
	}
}

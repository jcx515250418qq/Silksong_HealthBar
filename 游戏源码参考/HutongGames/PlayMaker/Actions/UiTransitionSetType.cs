using UnityEngine;
using UnityEngine.UI;

namespace HutongGames.PlayMaker.Actions
{
	[ActionCategory(ActionCategory.UI)]
	[Tooltip("Sets the transition type of a UI Selectable component.")]
	public class UiTransitionSetType : ComponentAction<Selectable>
	{
		[RequiredField]
		[CheckForComponent(typeof(Selectable))]
		[Tooltip("The GameObject with the UI Selectable component.")]
		public FsmOwnerDefault gameObject;

		[Tooltip("The transition value")]
		public Selectable.Transition transition;

		[Tooltip("Reset when exiting this state.")]
		public FsmBool resetOnExit;

		private Selectable selectable;

		private Selectable.Transition originalTransition;

		public override void Reset()
		{
			gameObject = null;
			transition = Selectable.Transition.ColorTint;
			resetOnExit = false;
		}

		public override void OnEnter()
		{
			GameObject ownerDefaultTarget = base.Fsm.GetOwnerDefaultTarget(gameObject);
			if (UpdateCache(ownerDefaultTarget))
			{
				selectable = cachedComponent;
			}
			if (selectable != null && resetOnExit.Value)
			{
				originalTransition = selectable.transition;
			}
			DoSetValue();
			Finish();
		}

		private void DoSetValue()
		{
			if (selectable != null)
			{
				selectable.transition = transition;
			}
		}

		public override void OnExit()
		{
			if (!(selectable == null) && resetOnExit.Value)
			{
				selectable.transition = originalTransition;
			}
		}
	}
}

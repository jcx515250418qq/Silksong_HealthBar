using UnityEngine;
using UnityEngine.UI;

namespace HutongGames.PlayMaker.Actions
{
	[ActionCategory(ActionCategory.UI)]
	[Tooltip("Sets the navigation mode of a UI Selectable component.")]
	public class UiNavigationSetMode : ComponentAction<Selectable>
	{
		[RequiredField]
		[CheckForComponent(typeof(Selectable))]
		[Tooltip("The GameObject with the UI Selectable component.")]
		public FsmOwnerDefault gameObject;

		[Tooltip("The navigation mode value")]
		public Navigation.Mode navigationMode;

		[Tooltip("Reset when exiting this state.")]
		public FsmBool resetOnExit;

		private Selectable selectable;

		private Navigation _navigation;

		private Navigation.Mode originalValue;

		public override void Reset()
		{
			gameObject = null;
			navigationMode = Navigation.Mode.Automatic;
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
				originalValue = selectable.navigation.mode;
			}
			DoSetValue();
			Finish();
		}

		private void DoSetValue()
		{
			if (selectable != null)
			{
				_navigation = selectable.navigation;
				_navigation.mode = navigationMode;
				selectable.navigation = _navigation;
			}
		}

		public override void OnExit()
		{
			if (!(selectable == null) && resetOnExit.Value)
			{
				_navigation = selectable.navigation;
				_navigation.mode = originalValue;
				selectable.navigation = _navigation;
			}
		}
	}
}

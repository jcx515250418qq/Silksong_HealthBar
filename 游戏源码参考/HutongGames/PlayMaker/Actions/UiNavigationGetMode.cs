using UnityEngine;
using UnityEngine.UI;

namespace HutongGames.PlayMaker.Actions
{
	[ActionCategory(ActionCategory.UI)]
	[Tooltip("Gets the navigation mode of a UI Selectable component.")]
	public class UiNavigationGetMode : ComponentAction<Selectable>
	{
		[RequiredField]
		[CheckForComponent(typeof(Selectable))]
		[Tooltip("The GameObject with the UI Selectable component.")]
		public FsmOwnerDefault gameObject;

		[Tooltip("The navigation mode value")]
		public FsmString navigationMode;

		[Tooltip("Event sent if transition is ColorTint")]
		public FsmEvent automaticEvent;

		[Tooltip("Event sent if transition is ColorTint")]
		public FsmEvent horizontalEvent;

		[Tooltip("Event sent if transition is SpriteSwap")]
		public FsmEvent verticalEvent;

		[Tooltip("Event sent if transition is Animation")]
		public FsmEvent explicitEvent;

		[Tooltip("Event sent if transition is none")]
		public FsmEvent noNavigationEvent;

		private Selectable selectable;

		public override void Reset()
		{
			gameObject = null;
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
				navigationMode.Value = selectable.navigation.mode.ToString();
				if (selectable.navigation.mode == Navigation.Mode.None)
				{
					base.Fsm.Event(noNavigationEvent);
				}
				else if (selectable.navigation.mode == Navigation.Mode.Automatic)
				{
					base.Fsm.Event(automaticEvent);
				}
				else if (selectable.navigation.mode == Navigation.Mode.Vertical)
				{
					base.Fsm.Event(verticalEvent);
				}
				else if (selectable.navigation.mode == Navigation.Mode.Horizontal)
				{
					base.Fsm.Event(horizontalEvent);
				}
				else if (selectable.navigation.mode == Navigation.Mode.Explicit)
				{
					base.Fsm.Event(explicitEvent);
				}
			}
		}
	}
}

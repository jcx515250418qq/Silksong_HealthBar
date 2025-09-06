using UnityEngine;
using UnityEngine.UI;

namespace HutongGames.PlayMaker.Actions
{
	[ActionCategory(ActionCategory.UI)]
	[Tooltip("Sets the direction of a UI Scrollbar component.")]
	public class UiScrollbarSetDirection : ComponentAction<Scrollbar>
	{
		[RequiredField]
		[CheckForComponent(typeof(Scrollbar))]
		[Tooltip("The GameObject with the UI Scrollbar component.")]
		public FsmOwnerDefault gameObject;

		[RequiredField]
		[Tooltip("The direction of the UI Scrollbar.")]
		[ObjectType(typeof(Scrollbar.Direction))]
		public FsmEnum direction;

		[Tooltip("Include the  RectLayouts. Leave to none for no effect")]
		public FsmBool includeRectLayouts;

		[Tooltip("Reset when exiting this state.")]
		public FsmBool resetOnExit;

		private Scrollbar scrollbar;

		private Scrollbar.Direction originalValue;

		public override void Reset()
		{
			gameObject = null;
			direction = Scrollbar.Direction.LeftToRight;
			includeRectLayouts = new FsmBool
			{
				UseVariable = true
			};
			resetOnExit = null;
		}

		public override void OnEnter()
		{
			GameObject ownerDefaultTarget = base.Fsm.GetOwnerDefaultTarget(gameObject);
			if (UpdateCache(ownerDefaultTarget))
			{
				scrollbar = cachedComponent;
			}
			if (resetOnExit.Value)
			{
				originalValue = scrollbar.direction;
			}
			DoSetValue();
			Finish();
		}

		private void DoSetValue()
		{
			if (!(scrollbar == null))
			{
				if (includeRectLayouts.IsNone)
				{
					scrollbar.direction = (Scrollbar.Direction)(object)direction.Value;
				}
				else
				{
					scrollbar.SetDirection((Scrollbar.Direction)(object)direction.Value, includeRectLayouts.Value);
				}
			}
		}

		public override void OnExit()
		{
			if (!(scrollbar == null) && resetOnExit.Value)
			{
				if (includeRectLayouts.IsNone)
				{
					scrollbar.direction = originalValue;
				}
				else
				{
					scrollbar.SetDirection(originalValue, includeRectLayouts.Value);
				}
			}
		}
	}
}

using UnityEngine;
using UnityEngine.UI;

namespace HutongGames.PlayMaker.Actions
{
	[ActionCategory(ActionCategory.UI)]
	[Tooltip("Sets the UI ScrollRect vertical flag")]
	public class UiScrollRectSetVertical : ComponentAction<ScrollRect>
	{
		[RequiredField]
		[CheckForComponent(typeof(ScrollRect))]
		[Tooltip("The GameObject with the UI ScrollRect component.")]
		public FsmOwnerDefault gameObject;

		[Tooltip("The vertical flag")]
		public FsmBool vertical;

		[Tooltip("Reset when exiting this state.")]
		public FsmBool resetOnExit;

		[Tooltip("Repeats every frame")]
		public bool everyFrame;

		private ScrollRect scrollRect;

		private bool originalValue;

		public override void Reset()
		{
			gameObject = null;
			vertical = null;
			resetOnExit = null;
			everyFrame = false;
		}

		public override void OnEnter()
		{
			GameObject ownerDefaultTarget = base.Fsm.GetOwnerDefaultTarget(gameObject);
			if (UpdateCache(ownerDefaultTarget))
			{
				scrollRect = cachedComponent;
			}
			originalValue = scrollRect.vertical;
			DoSetValue();
			if (!everyFrame)
			{
				Finish();
			}
		}

		public override void OnUpdate()
		{
			DoSetValue();
		}

		private void DoSetValue()
		{
			if (scrollRect != null)
			{
				scrollRect.vertical = vertical.Value;
			}
		}

		public override void OnExit()
		{
			if (!(scrollRect == null) && resetOnExit.Value)
			{
				scrollRect.vertical = originalValue;
			}
		}
	}
}

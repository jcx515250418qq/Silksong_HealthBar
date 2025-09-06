using UnityEngine;
using UnityEngine.UI;

namespace HutongGames.PlayMaker.Actions
{
	[ActionCategory(ActionCategory.UI)]
	[Tooltip("Sets the raycast target of a UI Raw Image component.")]
	public class UiRawImageSetRaycastTarget : ComponentAction<RawImage>
	{
		[RequiredField]
		[CheckForComponent(typeof(Image))]
		[Tooltip("The GameObject with the Raw Image UI component.")]
		public FsmOwnerDefault gameObject;

		[RequiredField]
		[Tooltip("The raycast target value to be set")]
		public FsmBool raycastTarget;

		[Tooltip("Reset when exiting this state.")]
		public FsmBool resetOnExit;

		private bool originalBool;

		public override void Reset()
		{
			gameObject = null;
			raycastTarget = null;
			resetOnExit = false;
		}

		public override void OnEnter()
		{
			GameObject ownerDefaultTarget = base.Fsm.GetOwnerDefaultTarget(gameObject);
			if (UpdateCache(ownerDefaultTarget))
			{
				originalBool = cachedComponent.raycastTarget;
				DoSetRaycastTarget();
			}
			Finish();
		}

		private void DoSetRaycastTarget()
		{
			cachedComponent.raycastTarget = raycastTarget.Value;
		}

		public override void OnExit()
		{
			if (resetOnExit.Value)
			{
				cachedComponent.raycastTarget = originalBool;
			}
		}
	}
}

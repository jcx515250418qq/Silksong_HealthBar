using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
	[ActionCategory(ActionCategory.UI)]
	[Tooltip("Enable or disable Canvas Raycasting. Optionally reset on state exit")]
	public class UiCanvasEnableRaycast : ComponentAction<PlayMakerCanvasRaycastFilterProxy>
	{
		[RequiredField]
		[Tooltip("The GameObject to enable or disable Canvas Raycasting on.")]
		public FsmOwnerDefault gameObject;

		[Tooltip("Enable/disable raycasting. Uncheck to disable.")]
		public FsmBool enableRaycasting;

		[Tooltip("Reset when exiting this state.")]
		public FsmBool resetOnExit;

		[Tooltip("Repeat every frame.")]
		public bool everyFrame;

		[SerializeField]
		private PlayMakerCanvasRaycastFilterProxy raycastFilterProxy;

		private bool originalValue;

		public override void Reset()
		{
			gameObject = null;
			enableRaycasting = false;
			resetOnExit = null;
			everyFrame = false;
		}

		public override void OnPreprocess()
		{
			if (gameObject == null)
			{
				gameObject = new FsmOwnerDefault();
			}
			GameObject ownerDefaultTarget = base.Fsm.GetOwnerDefaultTarget(gameObject);
			if (UpdateCacheAddComponent(ownerDefaultTarget))
			{
				raycastFilterProxy = cachedComponent;
			}
		}

		public override void OnEnter()
		{
			GameObject ownerDefaultTarget = base.Fsm.GetOwnerDefaultTarget(gameObject);
			if (UpdateCacheAddComponent(ownerDefaultTarget))
			{
				raycastFilterProxy = cachedComponent;
				originalValue = raycastFilterProxy.RayCastingEnabled;
			}
			DoAction();
			if (!everyFrame)
			{
				Finish();
			}
		}

		public override void OnUpdate()
		{
			DoAction();
		}

		private void DoAction()
		{
			if (raycastFilterProxy != null)
			{
				raycastFilterProxy.RayCastingEnabled = enableRaycasting.Value;
			}
		}

		public override void OnExit()
		{
			if (!(raycastFilterProxy == null) && resetOnExit.Value)
			{
				raycastFilterProxy.RayCastingEnabled = originalValue;
			}
		}
	}
}

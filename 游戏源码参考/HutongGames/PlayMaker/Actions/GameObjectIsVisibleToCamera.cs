using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
	[ActionCategory(ActionCategory.Logic)]
	[ActionTarget(typeof(GameObject), "gameObject", false)]
	[Tooltip("Tests if a Game Object is visible to a specific camera. Note, using bounds is a little more expensive than using the center point.")]
	public class GameObjectIsVisibleToCamera : ComponentAction<Renderer, Camera>
	{
		[RequiredField]
		[CheckForComponent(typeof(Renderer))]
		[Tooltip("The GameObject to test.")]
		public FsmOwnerDefault gameObject;

		[Tooltip("The GameObject with the Camera component.")]
		public FsmGameObject camera;

		[Tooltip("Use the bounds of the GameObject. Otherwise uses just the center point.")]
		public FsmBool useBounds;

		[Tooltip("Event to send if the GameObject is visible.")]
		public FsmEvent trueEvent;

		[Tooltip("Event to send if the GameObject is NOT visible.")]
		public FsmEvent falseEvent;

		[UIHint(UIHint.Variable)]
		[Tooltip("Store the result in a bool variable.")]
		public FsmBool storeResult;

		[Tooltip("Perform this action every frame.")]
		public bool everyFrame;

		private Camera cameraComponent => cachedComponent2;

		public override void Reset()
		{
			gameObject = null;
			camera = null;
			useBounds = null;
			trueEvent = null;
			falseEvent = null;
			storeResult = null;
			everyFrame = false;
		}

		public override void OnEnter()
		{
			DoIsVisible();
			if (!everyFrame)
			{
				Finish();
			}
		}

		public override void OnUpdate()
		{
			DoIsVisible();
		}

		private void DoIsVisible()
		{
			GameObject ownerDefaultTarget = base.Fsm.GetOwnerDefaultTarget(gameObject);
			if (UpdateCache(ownerDefaultTarget, camera.Value))
			{
				bool flag = ActionHelpers.IsVisible(ownerDefaultTarget, cameraComponent, useBounds.Value);
				storeResult.Value = flag;
				base.Fsm.Event(flag ? trueEvent : falseEvent);
			}
		}
	}
}

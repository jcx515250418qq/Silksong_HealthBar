using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
	[ActionCategory(ActionCategory.Input)]
	[Tooltip("Perform a raycast into the 2d scene using screen coordinates and stores the results. Use Ray Distance to set how close the camera must be to pick the 2d object. NOTE: Uses the MainCamera!")]
	public class ScreenPick2d : FsmStateAction
	{
		[Tooltip("A Vector3 screen position. Commonly stored by other actions.")]
		public FsmVector3 screenVector;

		[Tooltip("X position on screen.")]
		public FsmFloat screenX;

		[Tooltip("Y position on screen.")]
		public FsmFloat screenY;

		[Tooltip("Are the supplied screen coordinates normalized (0-1), or in pixels.")]
		public FsmBool normalized;

		[UIHint(UIHint.Variable)]
		[Tooltip("Store whether the Screen pick did pick a GameObject")]
		public FsmBool storeDidPickObject;

		[UIHint(UIHint.Variable)]
		[Tooltip("Store the picked GameObject")]
		public FsmGameObject storeGameObject;

		[UIHint(UIHint.Variable)]
		[Tooltip("Store the picked position in world Space")]
		public FsmVector3 storePoint;

		[UIHint(UIHint.Layer)]
		[Tooltip("Pick only from these layers.")]
		public FsmInt[] layerMask;

		[Tooltip("Invert the mask, so you pick from all layers except those defined above.")]
		public FsmBool invertMask;

		[Tooltip("Repeat every frame.")]
		public bool everyFrame;

		public override void Reset()
		{
			screenVector = new FsmVector3
			{
				UseVariable = true
			};
			screenX = new FsmFloat
			{
				UseVariable = true
			};
			screenY = new FsmFloat
			{
				UseVariable = true
			};
			normalized = false;
			storeDidPickObject = null;
			storeGameObject = null;
			storePoint = null;
			layerMask = new FsmInt[0];
			invertMask = false;
			everyFrame = false;
		}

		public override void OnEnter()
		{
			DoScreenPick();
			if (!everyFrame)
			{
				Finish();
			}
		}

		public override void OnUpdate()
		{
			DoScreenPick();
		}

		private void DoScreenPick()
		{
			if (Camera.main == null)
			{
				LogError("No MainCamera defined!");
				Finish();
				return;
			}
			Vector3 pos = Vector3.zero;
			if (!screenVector.IsNone)
			{
				pos = screenVector.Value;
			}
			if (!screenX.IsNone)
			{
				pos.x = screenX.Value;
			}
			if (!screenY.IsNone)
			{
				pos.y = screenY.Value;
			}
			if (normalized.Value)
			{
				pos.x *= Screen.width;
				pos.y *= Screen.height;
			}
			RaycastHit2D rayIntersection = Physics2D.GetRayIntersection(Camera.main.ScreenPointToRay(pos), float.PositiveInfinity, ActionHelpers.LayerArrayToLayerMask(layerMask, invertMask.Value));
			bool flag = rayIntersection.collider != null;
			storeDidPickObject.Value = flag;
			if (flag)
			{
				storeGameObject.Value = rayIntersection.collider.gameObject;
				storePoint.Value = rayIntersection.point;
			}
			else
			{
				storeGameObject.Value = null;
				storePoint.Value = Vector3.zero;
			}
		}
	}
}

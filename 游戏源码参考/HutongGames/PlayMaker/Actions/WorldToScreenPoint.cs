using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
	[ActionCategory(ActionCategory.Camera)]
	[Tooltip("Transforms a position from world space into screen space. \nNote: Uses the Main Camera unless you specify a camera to use.")]
	public class WorldToScreenPoint : FsmStateAction
	{
		[Tooltip("Camera GameObject to use. Defaults to MainCamera if not defined.")]
		public FsmGameObject camera;

		[UIHint(UIHint.Variable)]
		[Tooltip("World position to transform into screen coordinates.")]
		public FsmVector3 worldPosition;

		[Tooltip("Override X coordinate.")]
		public FsmFloat worldX;

		[Tooltip("Override Y coordinate.")]
		public FsmFloat worldY;

		[Tooltip("Override Z coordinate.")]
		public FsmFloat worldZ;

		[UIHint(UIHint.Variable)]
		[Tooltip("Store the screen position in a Vector3 Variable. Z will equal zero.")]
		public FsmVector3 storeScreenPoint;

		[UIHint(UIHint.Variable)]
		[Tooltip("Store the screen X position in a Float Variable.")]
		public FsmFloat storeScreenX;

		[UIHint(UIHint.Variable)]
		[Tooltip("Store the screen Y position in a Float Variable.")]
		public FsmFloat storeScreenY;

		[Tooltip("Normalize screen coordinates (0-1). Otherwise coordinates are in pixels.")]
		public FsmBool normalize;

		[Tooltip("Repeat every frame")]
		public bool everyFrame;

		private GameObject cameraGameObject;

		private Camera screenCamera;

		public override void Reset()
		{
			worldPosition = null;
			worldX = new FsmFloat
			{
				UseVariable = true
			};
			worldY = new FsmFloat
			{
				UseVariable = true
			};
			worldZ = new FsmFloat
			{
				UseVariable = true
			};
			storeScreenPoint = null;
			storeScreenX = null;
			storeScreenY = null;
			everyFrame = false;
		}

		private void InitCamera()
		{
			if (!(screenCamera == null) && !(cameraGameObject != camera.Value))
			{
				return;
			}
			cameraGameObject = camera.Value;
			if (cameraGameObject != null)
			{
				screenCamera = camera.Value.GetComponent<Camera>();
				return;
			}
			screenCamera = Camera.main;
			if (screenCamera != null)
			{
				cameraGameObject = screenCamera.gameObject;
			}
		}

		public override void OnEnter()
		{
			DoWorldToScreenPoint();
			if (!everyFrame)
			{
				Finish();
			}
		}

		public override void OnUpdate()
		{
			DoWorldToScreenPoint();
		}

		private void DoWorldToScreenPoint()
		{
			if (PlayMakerFSM.ApplicationIsQuitting)
			{
				return;
			}
			InitCamera();
			if (screenCamera == null)
			{
				LogError("No camera defined!");
				Finish();
				return;
			}
			Vector3 position = Vector3.zero;
			if (!worldPosition.IsNone)
			{
				position = worldPosition.Value;
			}
			if (!worldX.IsNone)
			{
				position.x = worldX.Value;
			}
			if (!worldY.IsNone)
			{
				position.y = worldY.Value;
			}
			if (!worldZ.IsNone)
			{
				position.z = worldZ.Value;
			}
			position = Camera.main.WorldToScreenPoint(position);
			if (normalize.Value)
			{
				position.x /= Screen.width;
				position.y /= Screen.height;
			}
			storeScreenPoint.Value = position;
			storeScreenX.Value = position.x;
			storeScreenY.Value = position.y;
		}

		public override string ErrorCheck()
		{
			InitCamera();
			if (screenCamera != null)
			{
				return null;
			}
			if (camera.Value == null)
			{
				if (Camera.main == null)
				{
					return "@camera:No MainCamera Defined!";
				}
				return null;
			}
			return "@camera:GameObject has no Camera!";
		}
	}
}

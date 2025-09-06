using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
	[ActionCategory(ActionCategory.Transform)]
	[Tooltip("Clamps an orthographic camera's position to keep the view inside min/max ranges. Set any limit to None to leave that axis un-clamped.")]
	public class ClampOrthographicView : ComponentAction<Camera>
	{
		public enum ScreenPlane
		{
			XY = 0,
			XZ = 1
		}

		[RequiredField]
		[Tooltip("The GameObject with a Camera component.")]
		public FsmOwnerDefault gameObject;

		[Tooltip("Orientation of the view.")]
		public ScreenPlane view;

		[Tooltip("The left edge of the view to stay inside.")]
		public FsmFloat minX;

		[Tooltip("The right edge of the view to stay inside.")]
		public FsmFloat maxX;

		[Tooltip("The bottom edge of the view to stay inside.")]
		public FsmFloat minY;

		[Tooltip("The top edge of the view to stay inside.")]
		public FsmFloat maxY;

		[Tooltip("Repeat every frame")]
		public bool everyFrame;

		[Tooltip("Perform in LateUpdate. This is useful if you want to clamp the position of objects that are animated or otherwise positioned in Update.")]
		public bool lateUpdate;

		public override void Reset()
		{
			gameObject = null;
			minX = new FsmFloat
			{
				UseVariable = true
			};
			maxX = new FsmFloat
			{
				UseVariable = true
			};
			minY = new FsmFloat
			{
				UseVariable = true
			};
			maxY = new FsmFloat
			{
				UseVariable = true
			};
			everyFrame = false;
			lateUpdate = false;
		}

		public override void OnPreprocess()
		{
			if (lateUpdate)
			{
				base.Fsm.HandleLateUpdate = true;
			}
		}

		public override void OnEnter()
		{
			if (!everyFrame && !lateUpdate)
			{
				DoClampPosition();
				Finish();
			}
		}

		public override void OnUpdate()
		{
			if (!lateUpdate)
			{
				DoClampPosition();
			}
		}

		public override void OnLateUpdate()
		{
			DoClampPosition();
			if (lateUpdate)
			{
				DoClampPosition();
			}
			if (!everyFrame)
			{
				Finish();
			}
		}

		private void DoClampPosition()
		{
			if (!UpdateCacheAndTransform(base.Fsm.GetOwnerDefaultTarget(gameObject)))
			{
				return;
			}
			Vector3 position = base.cachedTransform.position;
			float orthographicSize = base.camera.orthographicSize;
			float num = base.camera.orthographicSize * (float)Screen.width / (float)Screen.height;
			if (!minX.IsNone)
			{
				position.x = Mathf.Max(minX.Value + num, position.x);
			}
			if (!maxX.IsNone)
			{
				position.x = Mathf.Min(maxX.Value - num, position.x);
			}
			if (view == ScreenPlane.XY)
			{
				if (!minY.IsNone)
				{
					position.y = Mathf.Max(minY.Value + orthographicSize, position.y);
				}
				if (!maxY.IsNone)
				{
					position.y = Mathf.Min(maxY.Value - orthographicSize, position.y);
				}
			}
			else
			{
				if (!minY.IsNone)
				{
					position.z = Mathf.Max(minY.Value + orthographicSize, position.z);
				}
				if (!maxY.IsNone)
				{
					position.z = Mathf.Min(maxY.Value - orthographicSize, position.z);
				}
			}
			base.camera.transform.position = position;
		}
	}
}

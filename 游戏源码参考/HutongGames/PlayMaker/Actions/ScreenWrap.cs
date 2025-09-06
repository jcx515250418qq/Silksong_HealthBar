using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
	[ActionCategory(ActionCategory.Transform)]
	[Tooltip("Wraps a GameObject's position across screen edges. For example, a GameObject that moves off the left side of the screen wraps to the right side. This is often used in 2d arcade style games like Asteroids.")]
	public class ScreenWrap : ComponentAction<Transform, Camera>
	{
		[RequiredField]
		[Tooltip("The GameObject to wrap.")]
		public FsmOwnerDefault gameObject;

		[CheckForComponent(typeof(Camera))]
		[Tooltip("GameObject with a Camera component used to render the view (or MainCamera if not set). The Viewport Rect is used for wrapping.")]
		public FsmGameObject camera;

		[Tooltip("Wrap the position of the GameObject if it moves off the left side of the screen.")]
		public FsmBool wrapLeft;

		[Tooltip("Wrap the position of the GameObject if it moves off the right side of the screen.")]
		public FsmBool wrapRight;

		[Tooltip("Wrap the position of the GameObject if it moves off the top of the screen.")]
		public FsmBool wrapTop;

		[Tooltip("Wrap the position of the GameObject if it moves off the top of the screen.")]
		public FsmBool wrapBottom;

		[Tooltip("Repeat every frame.")]
		public bool everyFrame;

		[Tooltip("Use LateUpdate. Useful if you want to wrap after any other operations in Update.")]
		public bool lateUpdate;

		private Camera cameraComponent => cachedComponent2;

		private Transform cameraTransform => cachedTransform2;

		private Transform gameObjectTransform => cachedComponent1;

		public override void Reset()
		{
			gameObject = null;
			wrapLeft = new FsmBool
			{
				Value = true
			};
			wrapRight = new FsmBool
			{
				Value = true
			};
			wrapTop = new FsmBool
			{
				Value = true
			};
			wrapBottom = new FsmBool
			{
				Value = true
			};
			everyFrame = true;
			lateUpdate = true;
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
				DoScreenWrap();
				Finish();
			}
		}

		public override void OnUpdate()
		{
			if (!lateUpdate)
			{
				DoScreenWrap();
			}
		}

		public override void OnLateUpdate()
		{
			if (lateUpdate)
			{
				DoScreenWrap();
			}
			if (!everyFrame)
			{
				Finish();
			}
		}

		private void DoScreenWrap()
		{
			if (camera.Value == null)
			{
				camera.Value = ((Camera.main != null) ? Camera.main.gameObject : null);
			}
			if (UpdateCache(base.Fsm.GetOwnerDefaultTarget(gameObject), camera.Value))
			{
				Vector3 position = cameraComponent.WorldToViewportPoint(gameObjectTransform.position);
				bool flag = false;
				if ((wrapLeft.Value && position.x < 0f) || (wrapRight.Value && position.x >= 1f))
				{
					position.x = Wrap01(position.x);
					flag = true;
				}
				if ((wrapTop.Value && position.y >= 1f) || (wrapBottom.Value && position.y < 0f))
				{
					position.y = Wrap01(position.y);
					flag = true;
				}
				if (flag)
				{
					position.z = cameraTransform.InverseTransformPoint(gameObjectTransform.position).z;
					gameObjectTransform.position = cameraComponent.ViewportToWorldPoint(position);
				}
			}
		}

		private static float Wrap01(float x)
		{
			return Wrap(x, 0f, 1f);
		}

		private static float Wrap(float x, float xMin, float xMax)
		{
			x = ((!(x < xMin)) ? (xMin + (x - xMin) % (xMax - xMin)) : (xMax - (xMin - x) % (xMax - xMin)));
			return x;
		}
	}
}

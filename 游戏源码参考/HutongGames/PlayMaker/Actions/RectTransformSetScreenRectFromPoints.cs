using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
	[ActionCategory("RectTransform")]
	[Tooltip("Set the screen rect of a RectTransform using 2 Vector2 points.")]
	public class RectTransformSetScreenRectFromPoints : BaseUpdateAction
	{
		[RequiredField]
		[CheckForComponent(typeof(RectTransform))]
		[Tooltip("The GameObject target.")]
		public FsmOwnerDefault gameObject;

		[RequiredField]
		[Tooltip("The screen position of the first point to define the rect.")]
		public FsmVector2 point1;

		[RequiredField]
		[Tooltip("The screen position of the second point to define the rect.")]
		public FsmVector2 point2;

		[Tooltip("Screen points use normalized coordinates (0-1).")]
		public FsmBool normalized;

		[UIHint(UIHint.Variable)]
		[Tooltip("Store the resulting screen rect.")]
		public FsmRect storeScreenRect;

		private GameObject cachedGameObject;

		private RectTransform _rt;

		private Canvas rootCanvas;

		private RectTransform rootRectTransform;

		private Camera canvasCamera;

		public override void Reset()
		{
			base.Reset();
			gameObject = null;
			point1 = new FsmVector2
			{
				UseVariable = true
			};
			point2 = new FsmVector2
			{
				UseVariable = true
			};
			normalized = null;
			storeScreenRect = null;
		}

		private bool UpdateCache()
		{
			GameObject ownerDefaultTarget = base.Fsm.GetOwnerDefaultTarget(gameObject);
			if (ownerDefaultTarget != cachedGameObject)
			{
				cachedGameObject = ownerDefaultTarget;
				_rt = ownerDefaultTarget.GetComponent<RectTransform>();
				rootCanvas = ownerDefaultTarget.transform.GetComponentInParent<Canvas>().rootCanvas;
				rootRectTransform = rootCanvas.GetComponent<RectTransform>();
				canvasCamera = ((rootCanvas.renderMode == RenderMode.ScreenSpaceOverlay) ? null : rootCanvas.worldCamera);
			}
			return _rt != null;
		}

		public override void OnEnter()
		{
			if (!UpdateCache())
			{
				Finish();
				return;
			}
			DoSetValues();
			if (!everyFrame)
			{
				Finish();
			}
		}

		public override void OnActionUpdate()
		{
			DoSetValues();
		}

		private void DoSetValues()
		{
			if (!UpdateCache())
			{
				Finish();
				return;
			}
			Rect rect = default(Rect);
			rect.x = Mathf.Min(point1.Value.x, point2.Value.x);
			rect.y = Mathf.Min(point1.Value.y, point2.Value.y);
			rect.width = Mathf.Abs(point2.Value.x - point1.Value.x);
			rect.height = Mathf.Abs(point2.Value.y - point1.Value.y);
			Rect value = rect;
			storeScreenRect.Value = value;
			Vector2 min = value.min;
			Vector2 size = value.size;
			if (normalized.Value)
			{
				min.x *= Screen.width;
				min.y *= Screen.height;
				size.x *= Screen.width;
				size.y *= Screen.height;
			}
			RectTransformUtility.ScreenPointToLocalPointInRectangle(rootRectTransform, min, canvasCamera, out var localPoint);
			_rt.localPosition = localPoint;
			_rt.sizeDelta = size;
		}
	}
}

using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
	[ActionCategory("RectTransform")]
	[Tooltip("Set the screen position of this RectTransform.")]
	public class RectTransformSetScreenPosition : BaseUpdateAction
	{
		[RequiredField]
		[CheckForComponent(typeof(RectTransform))]
		[Tooltip("The GameObject target.")]
		public FsmOwnerDefault gameObject;

		[Tooltip("The position in screen coordinates.")]
		public FsmVector2 screenPosition;

		[Tooltip("Set the x component of the position. Set to None for no effect.")]
		public FsmFloat x;

		[Tooltip("Set the y component of the position. Set to None for no effect.")]
		public FsmFloat y;

		[Tooltip("Screen coordinates are normalized (0-1).")]
		public FsmBool normalized;

		private GameObject cachedGameObject;

		private RectTransform _rt;

		private Canvas rootCanvas;

		private RectTransform rootRectTransform;

		private Camera canvasCamera;

		public override void Reset()
		{
			base.Reset();
			gameObject = null;
			screenPosition = new FsmVector2
			{
				UseVariable = true
			};
			x = new FsmFloat
			{
				UseVariable = true
			};
			y = new FsmFloat
			{
				UseVariable = true
			};
			normalized = null;
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
			DoSetScreenPosition();
			if (!everyFrame)
			{
				Finish();
			}
		}

		public override void OnActionUpdate()
		{
			DoSetScreenPosition();
		}

		private void DoSetScreenPosition()
		{
			if (!UpdateCache())
			{
				Finish();
				return;
			}
			Vector2 screenPoint = screenPosition.Value;
			if (screenPosition.IsNone && (x.IsNone || y.IsNone))
			{
				screenPoint = RectTransformUtility.WorldToScreenPoint(canvasCamera, _rt.position);
			}
			if (!x.IsNone)
			{
				screenPoint.x = x.Value;
			}
			if (!y.IsNone)
			{
				screenPoint.y = y.Value;
			}
			if (normalized.Value)
			{
				screenPoint.x *= Screen.width;
				screenPoint.y *= Screen.height;
			}
			RectTransformUtility.ScreenPointToLocalPointInRectangle(rootRectTransform, screenPoint, canvasCamera, out var localPoint);
			_rt.localPosition = localPoint;
		}
	}
}

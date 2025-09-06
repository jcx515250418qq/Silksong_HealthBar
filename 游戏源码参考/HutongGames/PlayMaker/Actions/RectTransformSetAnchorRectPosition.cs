using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
	[ActionCategory("RectTransform")]
	[Tooltip("The position ( normalized or not) in the parent RectTransform keeping the anchor rect size intact. This lets you position the whole Rect in one go. Use this to easily animate movement (like IOS sliding UIView)")]
	public class RectTransformSetAnchorRectPosition : BaseUpdateAction
	{
		public enum AnchorReference
		{
			TopLeft = 0,
			Top = 1,
			TopRight = 2,
			Right = 3,
			BottomRight = 4,
			Bottom = 5,
			BottomLeft = 6,
			Left = 7,
			Center = 8
		}

		[RequiredField]
		[CheckForComponent(typeof(RectTransform))]
		[Tooltip("The GameObject target.")]
		public FsmOwnerDefault gameObject;

		[Tooltip("The reference for the given position")]
		public AnchorReference anchorReference;

		[Tooltip("Are the supplied screen coordinates normalized (0-1), or in pixels.")]
		public FsmBool normalized;

		[Tooltip("The Vector2 position, and/or set individual axis below.")]
		public FsmVector2 anchor;

		[HasFloatSlider(0f, 1f)]
		[Tooltip("Anchor X position.")]
		public FsmFloat x;

		[HasFloatSlider(0f, 1f)]
		[Tooltip("Anchor Y position.")]
		public FsmFloat y;

		private RectTransform _rt;

		private Rect _anchorRect;

		public override void Reset()
		{
			base.Reset();
			normalized = true;
			gameObject = null;
			anchorReference = AnchorReference.BottomLeft;
			anchor = null;
			x = new FsmFloat
			{
				UseVariable = true
			};
			y = new FsmFloat
			{
				UseVariable = true
			};
		}

		public override void OnEnter()
		{
			GameObject ownerDefaultTarget = base.Fsm.GetOwnerDefaultTarget(gameObject);
			if (ownerDefaultTarget != null)
			{
				_rt = ownerDefaultTarget.GetComponent<RectTransform>();
			}
			DoSetAnchor();
			if (!everyFrame)
			{
				Finish();
			}
		}

		public override void OnActionUpdate()
		{
			DoSetAnchor();
		}

		private void DoSetAnchor()
		{
			_anchorRect = default(Rect);
			_anchorRect.min = _rt.anchorMin;
			_anchorRect.max = _rt.anchorMax;
			Vector2 zero = Vector2.zero;
			zero = _anchorRect.min;
			if (!anchor.IsNone)
			{
				if (normalized.Value)
				{
					zero = anchor.Value;
				}
				else
				{
					zero.x = anchor.Value.x / (float)Screen.width;
					zero.y = anchor.Value.y / (float)Screen.height;
				}
			}
			if (!x.IsNone)
			{
				if (normalized.Value)
				{
					zero.x = x.Value;
				}
				else
				{
					zero.x = x.Value / (float)Screen.width;
				}
			}
			if (!y.IsNone)
			{
				if (normalized.Value)
				{
					zero.y = y.Value;
				}
				else
				{
					zero.y = y.Value / (float)Screen.height;
				}
			}
			if (anchorReference == AnchorReference.BottomLeft)
			{
				_anchorRect.x = zero.x;
				_anchorRect.y = zero.y;
			}
			else if (anchorReference == AnchorReference.Left)
			{
				_anchorRect.x = zero.x;
				_anchorRect.y = zero.y - 0.5f;
			}
			else if (anchorReference == AnchorReference.TopLeft)
			{
				_anchorRect.x = zero.x;
				_anchorRect.y = zero.y - 1f;
			}
			else if (anchorReference == AnchorReference.Top)
			{
				_anchorRect.x = zero.x - 0.5f;
				_anchorRect.y = zero.y - 1f;
			}
			else if (anchorReference == AnchorReference.TopRight)
			{
				_anchorRect.x = zero.x - 1f;
				_anchorRect.y = zero.y - 1f;
			}
			else if (anchorReference == AnchorReference.Right)
			{
				_anchorRect.x = zero.x - 1f;
				_anchorRect.y = zero.y - 0.5f;
			}
			else if (anchorReference == AnchorReference.BottomRight)
			{
				_anchorRect.x = zero.x - 1f;
				_anchorRect.y = zero.y;
			}
			else if (anchorReference == AnchorReference.Bottom)
			{
				_anchorRect.x = zero.x - 0.5f;
				_anchorRect.y = zero.y;
			}
			else if (anchorReference == AnchorReference.Center)
			{
				_anchorRect.x = zero.x - 0.5f;
				_anchorRect.y = zero.y - 0.5f;
			}
			_rt.anchorMin = _anchorRect.min;
			_rt.anchorMax = _anchorRect.max;
		}
	}
}

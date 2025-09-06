using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
	[ActionCategory("RectTransform")]
	[Tooltip("The normalized position in the parent RectTransform that the upper right corner is anchored to. This is relative screen space, values ranges from 0 to 1")]
	public class RectTransformSetAnchorMinAndMax : BaseUpdateAction
	{
		[RequiredField]
		[CheckForComponent(typeof(RectTransform))]
		[Tooltip("The GameObject target.")]
		public FsmOwnerDefault gameObject;

		[Tooltip("The Vector2 anchor max. Set to none for no effect, and/or set individual axis below.")]
		public FsmVector2 anchorMax;

		[Tooltip("The Vector2 anchor min. Set to none for no effect, and/or set individual axis below.")]
		public FsmVector2 anchorMin;

		[HasFloatSlider(0f, 1f)]
		[Tooltip("Setting only the x value. Overrides anchorMax x value if set. Set to none for no effect")]
		public FsmFloat xMax;

		[HasFloatSlider(0f, 1f)]
		[Tooltip("Setting only the x value. Overrides anchorMax x value if set. Set to none for no effect")]
		public FsmFloat yMax;

		[HasFloatSlider(0f, 1f)]
		[Tooltip("Setting only the x value. Overrides anchorMin x value if set. Set to none for no effect")]
		public FsmFloat xMin;

		[HasFloatSlider(0f, 1f)]
		[Tooltip("Setting only the x value. Overrides anchorMin x value if set. Set to none for no effect")]
		public FsmFloat yMin;

		private RectTransform _rt;

		public override void Reset()
		{
			base.Reset();
			gameObject = null;
			anchorMax = null;
			anchorMin = null;
			xMax = new FsmFloat
			{
				UseVariable = true
			};
			yMax = new FsmFloat
			{
				UseVariable = true
			};
			xMin = new FsmFloat
			{
				UseVariable = true
			};
			yMin = new FsmFloat
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
			DoSetAnchorMax();
			if (!everyFrame)
			{
				Finish();
			}
		}

		public override void OnActionUpdate()
		{
			DoSetAnchorMax();
		}

		private void DoSetAnchorMax()
		{
			Vector2 value = _rt.anchorMax;
			Vector2 value2 = _rt.anchorMin;
			if (!anchorMax.IsNone)
			{
				value = anchorMax.Value;
				value2 = anchorMin.Value;
			}
			if (!xMax.IsNone)
			{
				value.x = xMax.Value;
			}
			if (!yMax.IsNone)
			{
				value.y = yMax.Value;
			}
			if (!xMin.IsNone)
			{
				value2.x = xMin.Value;
			}
			if (!yMin.IsNone)
			{
				value2.y = yMin.Value;
			}
			_rt.anchorMax = value;
			_rt.anchorMin = value2;
		}
	}
}

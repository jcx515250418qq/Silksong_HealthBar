using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
	[ActionCategory("RectTransform")]
	[Tooltip("The calculated rectangle in the local space of the Transform.")]
	public class RectTransformGetRect : BaseUpdateAction
	{
		[RequiredField]
		[CheckForComponent(typeof(RectTransform))]
		[Tooltip("The GameObject target.")]
		public FsmOwnerDefault gameObject;

		[UIHint(UIHint.Variable)]
		[Tooltip("Store the Rect in a Rect variable.")]
		public FsmRect rect;

		[UIHint(UIHint.Variable)]
		[Tooltip("Store the x coordinate in a float variable.")]
		public FsmFloat x;

		[UIHint(UIHint.Variable)]
		[Tooltip("Store the y\u00a0coordinate in a float variable.")]
		public FsmFloat y;

		[UIHint(UIHint.Variable)]
		[Tooltip("Store the width in a float variable.")]
		public FsmFloat width;

		[UIHint(UIHint.Variable)]
		[Tooltip("Store the height in a float variable.")]
		public FsmFloat height;

		private RectTransform _rt;

		public override void Reset()
		{
			base.Reset();
			gameObject = null;
			rect = null;
			x = new FsmFloat
			{
				UseVariable = true
			};
			y = new FsmFloat
			{
				UseVariable = true
			};
			width = new FsmFloat
			{
				UseVariable = true
			};
			height = new FsmFloat
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
			DoGetValues();
			if (!everyFrame)
			{
				Finish();
			}
		}

		public override void OnActionUpdate()
		{
			DoGetValues();
		}

		private void DoGetValues()
		{
			if (!rect.IsNone)
			{
				rect.Value = _rt.rect;
			}
			if (!x.IsNone)
			{
				x.Value = _rt.rect.x;
			}
			if (!y.IsNone)
			{
				y.Value = _rt.rect.y;
			}
			if (!width.IsNone)
			{
				width.Value = _rt.rect.width;
			}
			if (!height.IsNone)
			{
				height.Value = _rt.rect.height;
			}
		}
	}
}

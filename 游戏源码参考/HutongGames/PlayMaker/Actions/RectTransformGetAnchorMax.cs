using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
	[ActionCategory("RectTransform")]
	[Tooltip("Get the normalized position in the parent RectTransform that the upper right corner is anchored to.")]
	public class RectTransformGetAnchorMax : BaseUpdateAction
	{
		[RequiredField]
		[CheckForComponent(typeof(RectTransform))]
		[Tooltip("The GameObject target.")]
		public FsmOwnerDefault gameObject;

		[Tooltip("The anchorMax")]
		[UIHint(UIHint.Variable)]
		public FsmVector2 anchorMax;

		[Tooltip("The x component of the anchorMax")]
		[UIHint(UIHint.Variable)]
		public FsmFloat x;

		[Tooltip("The y component of the anchorMax")]
		[UIHint(UIHint.Variable)]
		public FsmFloat y;

		private RectTransform _rt;

		public override void Reset()
		{
			base.Reset();
			gameObject = null;
			anchorMax = null;
			x = null;
			y = null;
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
			if (!anchorMax.IsNone)
			{
				anchorMax.Value = _rt.anchorMax;
			}
			if (!x.IsNone)
			{
				x.Value = _rt.anchorMax.x;
			}
			if (!y.IsNone)
			{
				y.Value = _rt.anchorMax.y;
			}
		}
	}
}

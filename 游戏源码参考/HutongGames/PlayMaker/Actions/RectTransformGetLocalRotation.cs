using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
	[ActionCategory("RectTransform")]
	[Tooltip("Gets the local rotation of this RectTransform.")]
	public class RectTransformGetLocalRotation : BaseUpdateAction
	{
		[RequiredField]
		[CheckForComponent(typeof(RectTransform))]
		[Tooltip("The GameObject target.")]
		public FsmOwnerDefault gameObject;

		[Tooltip("The rotation")]
		public FsmVector3 rotation;

		[Tooltip("The x component of the rotation")]
		public FsmFloat x;

		[Tooltip("The y component of the rotation")]
		public FsmFloat y;

		[Tooltip("The z component of the rotation")]
		public FsmFloat z;

		private RectTransform _rt;

		public override void Reset()
		{
			base.Reset();
			gameObject = null;
			rotation = new FsmVector3
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
			z = new FsmFloat
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
			if (!(_rt == null))
			{
				if (!rotation.IsNone)
				{
					rotation.Value = _rt.eulerAngles;
				}
				if (!x.IsNone)
				{
					x.Value = _rt.eulerAngles.x;
				}
				if (!y.IsNone)
				{
					y.Value = _rt.eulerAngles.y;
				}
				if (!z.IsNone)
				{
					z.Value = _rt.eulerAngles.z;
				}
			}
		}
	}
}

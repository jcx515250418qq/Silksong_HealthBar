using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
	[ActionCategory("RectTransform")]
	[Tooltip("Set the local rotation of this RectTransform.")]
	public class RectTransformSetLocalRotation : BaseUpdateAction
	{
		[RequiredField]
		[CheckForComponent(typeof(RectTransform))]
		[Tooltip("The GameObject target.")]
		public FsmOwnerDefault gameObject;

		[Tooltip("The rotation. Set to none for no effect")]
		public FsmVector3 rotation;

		[Tooltip("The x component of the rotation. Set to none for no effect")]
		public FsmFloat x;

		[Tooltip("The y component of the rotation. Set to none for no effect")]
		public FsmFloat y;

		[Tooltip("The z component of the rotation. Set to none for no effect")]
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
			if (!(_rt == null))
			{
				Vector3 eulerAngles = _rt.eulerAngles;
				if (!rotation.IsNone)
				{
					eulerAngles = rotation.Value;
				}
				if (!x.IsNone)
				{
					eulerAngles.x = x.Value;
				}
				if (!y.IsNone)
				{
					eulerAngles.y = y.Value;
				}
				if (!z.IsNone)
				{
					eulerAngles.z = z.Value;
				}
				_rt.eulerAngles = eulerAngles;
			}
		}
	}
}

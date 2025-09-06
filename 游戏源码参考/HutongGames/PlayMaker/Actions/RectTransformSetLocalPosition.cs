using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
	[ActionCategory("RectTransform")]
	[Tooltip("Set the local position of this RectTransform.")]
	public class RectTransformSetLocalPosition : BaseUpdateAction
	{
		[RequiredField]
		[CheckForComponent(typeof(RectTransform))]
		[Tooltip("The GameObject target.")]
		public FsmOwnerDefault gameObject;

		[Tooltip("The position. Set to none for no effect")]
		public FsmVector2 position2d;

		[Tooltip("Or the 3d position. Set to none for no effect")]
		public FsmVector3 position;

		[Tooltip("The x component of the position. Set to none for no effect")]
		public FsmFloat x;

		[Tooltip("The y component of the position. Set to none for no effect")]
		public FsmFloat y;

		[Tooltip("The z component of the position. Set to none for no effect")]
		public FsmFloat z;

		private GameObject cachedGameObject;

		private RectTransform _rt;

		public override void Reset()
		{
			base.Reset();
			gameObject = null;
			position2d = new FsmVector2
			{
				UseVariable = true
			};
			position = new FsmVector3
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
			if (ownerDefaultTarget != cachedGameObject)
			{
				cachedGameObject = ownerDefaultTarget;
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
				Vector3 localPosition = _rt.localPosition;
				if (!position.IsNone)
				{
					localPosition = position.Value;
				}
				if (!position2d.IsNone)
				{
					localPosition.x = position2d.Value.x;
					localPosition.y = position2d.Value.y;
				}
				if (!x.IsNone)
				{
					localPosition.x = x.Value;
				}
				if (!y.IsNone)
				{
					localPosition.y = y.Value;
				}
				if (!z.IsNone)
				{
					localPosition.z = z.Value;
				}
				_rt.localPosition = localPosition;
			}
		}
	}
}

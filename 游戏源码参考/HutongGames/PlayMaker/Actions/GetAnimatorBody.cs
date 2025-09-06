using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
	[ActionCategory(ActionCategory.Animator)]
	[Tooltip("Gets the avatar body mass center position and rotation. Optionally accepts a GameObject to get the body transform. \nThe position and rotation are local to the GameObject")]
	public class GetAnimatorBody : FsmStateActionAnimatorBase
	{
		[RequiredField]
		[CheckForComponent(typeof(Animator))]
		[Tooltip("The target. An Animator component and a PlayMakerAnimatorProxy component are required.")]
		public FsmOwnerDefault gameObject;

		[ActionSection("Results")]
		[UIHint(UIHint.Variable)]
		[Tooltip("The avatar body mass center")]
		public FsmVector3 bodyPosition;

		[UIHint(UIHint.Variable)]
		[Tooltip("The avatar body mass center")]
		public FsmQuaternion bodyRotation;

		[Tooltip("If set, apply the body mass center position and rotation to this gameObject")]
		public FsmGameObject bodyGameObject;

		private GameObject cachedBodyGameObject;

		private Transform _transform;

		private Animator animator => cachedComponent;

		public override void Reset()
		{
			base.Reset();
			gameObject = null;
			bodyPosition = null;
			bodyRotation = null;
			bodyGameObject = null;
			everyFrame = false;
			everyFrameOption = AnimatorFrameUpdateSelector.OnAnimatorIK;
		}

		public override void OnEnter()
		{
			everyFrameOption = AnimatorFrameUpdateSelector.OnAnimatorIK;
		}

		public override void OnActionUpdate()
		{
			DoGetBodyPosition();
			if (!everyFrame)
			{
				Finish();
			}
		}

		private void DoGetBodyPosition()
		{
			if (!UpdateCache(base.Fsm.GetOwnerDefaultTarget(gameObject)))
			{
				Finish();
				return;
			}
			bodyPosition.Value = animator.bodyPosition;
			bodyRotation.Value = animator.bodyRotation;
			if (cachedBodyGameObject != bodyGameObject.Value)
			{
				cachedBodyGameObject = bodyGameObject.Value;
				_transform = ((cachedBodyGameObject != null) ? cachedBodyGameObject.transform : null);
			}
			if (_transform != null)
			{
				_transform.position = animator.bodyPosition;
				_transform.rotation = animator.bodyRotation;
			}
		}

		public override string ErrorCheck()
		{
			if (everyFrameOption != AnimatorFrameUpdateSelector.OnAnimatorIK)
			{
				return "Getting Body Position should only be done in OnAnimatorIK";
			}
			return string.Empty;
		}
	}
}

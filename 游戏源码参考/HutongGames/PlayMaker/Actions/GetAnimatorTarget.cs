using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
	[ActionCategory(ActionCategory.Animator)]
	[Tooltip("Gets the position and rotation of the target specified by SetTarget(AvatarTarget targetIndex, float targetNormalizedTime)).\nThe position and rotation are only valid when a frame has being evaluated after the SetTarget call")]
	public class GetAnimatorTarget : FsmStateActionAnimatorBase
	{
		[RequiredField]
		[CheckForComponent(typeof(Animator))]
		[Tooltip("The GameObject with an Animator Component.")]
		public FsmOwnerDefault gameObject;

		[ActionSection("Results")]
		[UIHint(UIHint.Variable)]
		[Tooltip("The target position")]
		public FsmVector3 targetPosition;

		[UIHint(UIHint.Variable)]
		[Tooltip("The target rotation")]
		public FsmQuaternion targetRotation;

		[Tooltip("If set, apply the position and rotation to this gameObject")]
		public FsmGameObject targetGameObject;

		private GameObject cachedTargetGameObject;

		private Transform _transform;

		private Animator animator => cachedComponent;

		public override void Reset()
		{
			base.Reset();
			gameObject = null;
			targetPosition = null;
			targetRotation = null;
			targetGameObject = null;
			everyFrame = false;
		}

		public override void OnEnter()
		{
			DoGetTarget();
			if (!everyFrame)
			{
				Finish();
			}
		}

		public override void OnActionUpdate()
		{
			DoGetTarget();
		}

		private void DoGetTarget()
		{
			if (!UpdateCache(base.Fsm.GetOwnerDefaultTarget(gameObject)))
			{
				Finish();
				return;
			}
			if (cachedTargetGameObject != targetGameObject.Value)
			{
				cachedTargetGameObject = targetGameObject.Value;
				_transform = ((cachedTargetGameObject != null) ? cachedTargetGameObject.transform : null);
			}
			targetPosition.Value = animator.targetPosition;
			targetRotation.Value = animator.targetRotation;
			if (_transform != null)
			{
				_transform.position = animator.targetPosition;
				_transform.rotation = animator.targetRotation;
			}
		}
	}
}

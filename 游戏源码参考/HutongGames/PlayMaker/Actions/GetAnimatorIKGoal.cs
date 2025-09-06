using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
	[ActionCategory(ActionCategory.Animator)]
	[Tooltip("Gets the position, rotation and weights of an IK goal. A GameObject can be set to use for the position and rotation")]
	public class GetAnimatorIKGoal : FsmStateActionAnimatorBase
	{
		[RequiredField]
		[CheckForComponent(typeof(Animator))]
		[Tooltip("The GameObject with an Animator Component.")]
		public FsmOwnerDefault gameObject;

		[Tooltip("The IK goal")]
		[ObjectType(typeof(AvatarIKGoal))]
		public FsmEnum iKGoal;

		[ActionSection("Results")]
		[UIHint(UIHint.Variable)]
		[Tooltip("The gameObject to apply ik goal position and rotation to.")]
		public FsmGameObject goal;

		[UIHint(UIHint.Variable)]
		[Tooltip("Gets The position of the ik goal. If Goal GameObject is defined, position is used as an offset from Goal")]
		public FsmVector3 position;

		[UIHint(UIHint.Variable)]
		[Tooltip("Gets The rotation of the ik goal.If Goal GameObject define, rotation is used as an offset from Goal")]
		public FsmQuaternion rotation;

		[UIHint(UIHint.Variable)]
		[Tooltip("Gets The translative weight of an IK goal (0 = at the original animation before IK, 1 = at the goal)")]
		public FsmFloat positionWeight;

		[UIHint(UIHint.Variable)]
		[Tooltip("Gets the rotational weight of an IK goal (0 = rotation before IK, 1 = rotation at the IK goal)")]
		public FsmFloat rotationWeight;

		private GameObject cachedGoal;

		private Transform _transform;

		private AvatarIKGoal _iKGoal;

		private Animator animator => cachedComponent;

		public override void Reset()
		{
			base.Reset();
			gameObject = null;
			iKGoal = null;
			goal = null;
			position = null;
			rotation = null;
			positionWeight = null;
			rotationWeight = null;
		}

		public override void OnEnter()
		{
		}

		public override void OnActionUpdate()
		{
			DoGetIKGoal();
			if (!everyFrame)
			{
				Finish();
			}
		}

		private void DoGetIKGoal()
		{
			if (!UpdateCache(base.Fsm.GetOwnerDefaultTarget(gameObject)))
			{
				Finish();
				return;
			}
			if (cachedGoal != goal.Value)
			{
				cachedGoal = goal.Value;
				_transform = ((cachedGoal != null) ? cachedGoal.transform : null);
			}
			_iKGoal = (AvatarIKGoal)(object)iKGoal.Value;
			if (_transform != null)
			{
				_transform.position = animator.GetIKPosition(_iKGoal);
				_transform.rotation = animator.GetIKRotation(_iKGoal);
			}
			if (!position.IsNone)
			{
				position.Value = animator.GetIKPosition(_iKGoal);
			}
			if (!rotation.IsNone)
			{
				rotation.Value = animator.GetIKRotation(_iKGoal);
			}
			if (!positionWeight.IsNone)
			{
				positionWeight.Value = animator.GetIKPositionWeight(_iKGoal);
			}
			if (!rotationWeight.IsNone)
			{
				rotationWeight.Value = animator.GetIKRotationWeight(_iKGoal);
			}
		}
	}
}

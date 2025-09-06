using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
	[ActionCategory(ActionCategory.Animator)]
	[Tooltip("Sets the position and rotation of the body. A GameObject can be set to control the position and rotation, or it can be manually expressed.")]
	public class SetAnimatorBody : ComponentAction<Animator>
	{
		[RequiredField]
		[CheckForComponent(typeof(Animator))]
		[Tooltip("The GameObject with an Animator Component.")]
		public FsmOwnerDefault gameObject;

		[Tooltip("The gameObject target of the ik goal")]
		public FsmGameObject target;

		[Tooltip("The position of the ik goal. If Goal GameObject set, position is used as an offset from Goal")]
		public FsmVector3 position;

		[Tooltip("The rotation of the ik goal.If Goal GameObject set, rotation is used as an offset from Goal")]
		public FsmQuaternion rotation;

		[Tooltip("Repeat every frame.")]
		public bool everyFrame;

		private GameObject cachedTarget;

		private Transform _transform;

		private Animator animator => cachedComponent;

		public override void Reset()
		{
			gameObject = null;
			target = null;
			position = new FsmVector3
			{
				UseVariable = true
			};
			rotation = new FsmQuaternion
			{
				UseVariable = true
			};
			everyFrame = false;
		}

		public override void OnPreprocess()
		{
			base.Fsm.HandleAnimatorIK = true;
		}

		public override void DoAnimatorIK(int layerIndex)
		{
			DoSetBody();
			if (!everyFrame)
			{
				Finish();
			}
		}

		private void DoSetBody()
		{
			if (!UpdateCache(base.Fsm.GetOwnerDefaultTarget(gameObject)))
			{
				Finish();
				return;
			}
			if (cachedTarget != target.Value)
			{
				cachedTarget = target.Value;
				_transform = ((cachedTarget != null) ? cachedTarget.transform : null);
			}
			if (_transform != null)
			{
				if (position.IsNone)
				{
					animator.bodyPosition = _transform.position;
				}
				else
				{
					animator.bodyPosition = _transform.position + position.Value;
				}
				if (rotation.IsNone)
				{
					animator.bodyRotation = _transform.rotation;
				}
				else
				{
					animator.bodyRotation = _transform.rotation * rotation.Value;
				}
			}
			else
			{
				if (!position.IsNone)
				{
					animator.bodyPosition = position.Value;
				}
				if (!rotation.IsNone)
				{
					animator.bodyRotation = rotation.Value;
				}
			}
		}
	}
}

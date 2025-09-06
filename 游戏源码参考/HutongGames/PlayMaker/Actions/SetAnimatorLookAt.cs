using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
	[ActionCategory(ActionCategory.Animator)]
	[Tooltip("Sets look at position and weights. You can use a target GameObject or position.")]
	public class SetAnimatorLookAt : ComponentAction<Animator>
	{
		[RequiredField]
		[CheckForComponent(typeof(Animator))]
		[Tooltip("The GameObject with an Animator Component.")]
		public FsmOwnerDefault gameObject;

		[Tooltip("The GameObject to look at. Set to None to use a position instead.")]
		public FsmGameObject target;

		[Tooltip("The look-at position. If Target GameObject is set, this is used as an offset from the Target's position.")]
		public FsmVector3 targetPosition;

		[HasFloatSlider(0f, 1f)]
		[Tooltip("The global weight of the LookAt, multiplier for other parameters. Range from 0 to 1")]
		public FsmFloat weight;

		[HasFloatSlider(0f, 1f)]
		[Tooltip("Determines how much the body is involved in the LookAt. Range from 0 to 1")]
		public FsmFloat bodyWeight;

		[HasFloatSlider(0f, 1f)]
		[Tooltip("Determines how much the head is involved in the LookAt. Range from 0 to 1")]
		public FsmFloat headWeight;

		[HasFloatSlider(0f, 1f)]
		[Tooltip("Determines how much the eyes are involved in the LookAt. Range from 0 to 1")]
		public FsmFloat eyesWeight;

		[HasFloatSlider(0f, 1f)]
		[Tooltip("0.0 means the character is completely unrestrained in motion, 1.0 means he's completely clamped (look at becomes impossible), and 0.5 means he'll be able to move on half of the possible range (180 degrees).")]
		public FsmFloat clampWeight;

		[Tooltip("Repeat every frame during OnAnimatorIK(). This would normally be true.")]
		public bool everyFrame;

		private GameObject cachedTarget;

		private Transform _transform;

		private Animator animator => cachedComponent;

		public override void Reset()
		{
			gameObject = null;
			target = null;
			targetPosition = new FsmVector3
			{
				UseVariable = true
			};
			weight = 1f;
			bodyWeight = 0.3f;
			headWeight = 0.6f;
			eyesWeight = 1f;
			clampWeight = 0.5f;
			everyFrame = false;
		}

		public override void OnPreprocess()
		{
			base.Fsm.HandleAnimatorIK = true;
		}

		public override void OnEnter()
		{
		}

		public override void DoAnimatorIK(int layerIndex)
		{
			DoSetLookAt();
			if (!everyFrame)
			{
				Finish();
			}
		}

		private void DoSetLookAt()
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
				if (targetPosition.IsNone)
				{
					animator.SetLookAtPosition(_transform.position);
				}
				else
				{
					animator.SetLookAtPosition(_transform.position + targetPosition.Value);
				}
			}
			else if (!targetPosition.IsNone)
			{
				animator.SetLookAtPosition(targetPosition.Value);
			}
			if (!clampWeight.IsNone)
			{
				animator.SetLookAtWeight(weight.Value, bodyWeight.Value, headWeight.Value, eyesWeight.Value, clampWeight.Value);
			}
			else if (!eyesWeight.IsNone)
			{
				animator.SetLookAtWeight(weight.Value, bodyWeight.Value, headWeight.Value, eyesWeight.Value);
			}
			else if (!headWeight.IsNone)
			{
				animator.SetLookAtWeight(weight.Value, bodyWeight.Value, headWeight.Value);
			}
			else if (!bodyWeight.IsNone)
			{
				animator.SetLookAtWeight(weight.Value, bodyWeight.Value);
			}
			else if (!weight.IsNone)
			{
				animator.SetLookAtWeight(weight.Value);
			}
		}
	}
}

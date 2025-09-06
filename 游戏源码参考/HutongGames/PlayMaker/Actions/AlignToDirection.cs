using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
	[ActionCategory(ActionCategory.Transform)]
	[Tooltip("Align a GameObject to the specified Direction.")]
	public class AlignToDirection : ComponentAction<Transform>
	{
		public enum AlignAxis
		{
			x = 0,
			y = 1,
			z = 2
		}

		[RequiredField]
		[Tooltip("The GameObject to rotate.")]
		public FsmOwnerDefault gameObject;

		[RequiredField]
		[Tooltip("The direction to look at. E.g. the Hit Normal from a Raycast.")]
		public FsmVector3 targetDirection;

		[RequiredField]
		[Tooltip("The GameObject axis to align to the direction.")]
		[ObjectType(typeof(AlignAxis))]
		public FsmEnum alignAxis;

		[Tooltip("Flip the alignment axis. So x becomes -x.")]
		public FsmBool flipAxis;

		[Tooltip("Repeat every update.")]
		public bool everyFrame;

		[Tooltip("Perform in LateUpdate. This can help eliminate jitters in some situations.")]
		public bool lateUpdate;

		public override void Reset()
		{
			gameObject = null;
			targetDirection = new FsmVector3
			{
				UseVariable = true
			};
			alignAxis = null;
			flipAxis = null;
			everyFrame = false;
			lateUpdate = false;
		}

		public override void OnPreprocess()
		{
			base.Fsm.HandleLateUpdate = lateUpdate;
		}

		public override void OnEnter()
		{
			DoAlignToDirection();
			if (!everyFrame)
			{
				Finish();
			}
		}

		public override void OnUpdate()
		{
			if (!lateUpdate)
			{
				DoAlignToDirection();
			}
		}

		public override void OnLateUpdate()
		{
			if (lateUpdate)
			{
				DoAlignToDirection();
			}
		}

		private void DoAlignToDirection()
		{
			if (!targetDirection.IsNone && UpdateCachedTransform(base.Fsm.GetOwnerDefaultTarget(gameObject)))
			{
				Vector3 fromDirection = default(Vector3);
				switch ((AlignAxis)(object)alignAxis.Value)
				{
				case AlignAxis.x:
					fromDirection = base.cachedTransform.right;
					break;
				case AlignAxis.y:
					fromDirection = base.cachedTransform.up;
					break;
				case AlignAxis.z:
					fromDirection = base.cachedTransform.forward;
					break;
				}
				if (flipAxis.Value)
				{
					fromDirection *= -1f;
				}
				base.cachedTransform.rotation = Quaternion.FromToRotation(fromDirection, targetDirection.Value) * base.cachedTransform.rotation;
			}
		}
	}
}

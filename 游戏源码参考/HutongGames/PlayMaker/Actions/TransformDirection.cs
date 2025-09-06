using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
	[ActionCategory(ActionCategory.Transform)]
	[Tooltip("Transforms a Direction from a Game Object's local space to world space.")]
	public class TransformDirection : FsmStateAction
	{
		[RequiredField]
		[Tooltip("The Game Object that defines local space.")]
		public FsmOwnerDefault gameObject;

		[RequiredField]
		[Tooltip("A direction vector in the object's local space.")]
		public FsmVector3 localDirection;

		[RequiredField]
		[UIHint(UIHint.Variable)]
		[Tooltip("Store the transformed direction vector, now in world space, in a Vector3 Variable.")]
		public FsmVector3 storeResult;

		[Tooltip("Repeat every frame.")]
		public bool everyFrame;

		public override void Reset()
		{
			gameObject = null;
			localDirection = null;
			storeResult = null;
			everyFrame = false;
		}

		public override void OnEnter()
		{
			DoTransformDirection();
			if (!everyFrame)
			{
				Finish();
			}
		}

		public override void OnUpdate()
		{
			DoTransformDirection();
		}

		private void DoTransformDirection()
		{
			GameObject ownerDefaultTarget = base.Fsm.GetOwnerDefaultTarget(gameObject);
			if (!(ownerDefaultTarget == null))
			{
				storeResult.Value = ownerDefaultTarget.transform.TransformDirection(localDirection.Value);
			}
		}
	}
}

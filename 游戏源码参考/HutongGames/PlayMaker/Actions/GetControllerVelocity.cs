using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
	[ActionCategory(ActionCategory.Character)]
	[Tooltip("Gets a CharacterController's velocity.")]
	public class GetControllerVelocity : ComponentAction<CharacterController>
	{
		[RequiredField]
		[CheckForComponent(typeof(CharacterController))]
		[Tooltip("The GameObject with a CharacterController.")]
		public FsmOwnerDefault gameObject;

		[UIHint(UIHint.Variable)]
		[Tooltip("Store the velocity in Vector3 variable.")]
		public FsmVector3 storeVelocity;

		[UIHint(UIHint.Variable)]
		[Tooltip("Store the x component of the velocity in a Float variable.")]
		public FsmFloat storeX;

		[UIHint(UIHint.Variable)]
		[Tooltip("Store the y component of the velocity in a Float variable.")]
		public FsmFloat storeY;

		[UIHint(UIHint.Variable)]
		[Tooltip("Store the z component of the velocity in a Float variable.")]
		public FsmFloat storeZ;

		[Tooltip("Repeat every frame.")]
		public bool everyFrame;

		private CharacterController controller => cachedComponent;

		public override void Reset()
		{
			gameObject = null;
			storeVelocity = null;
			storeX = null;
			storeY = null;
			storeZ = null;
		}

		public override void OnEnter()
		{
			DoGetControllerVelocity();
			if (!everyFrame)
			{
				Finish();
			}
		}

		public override void OnUpdate()
		{
			DoGetControllerVelocity();
		}

		private void DoGetControllerVelocity()
		{
			if (UpdateCache(base.Fsm.GetOwnerDefaultTarget(gameObject)))
			{
				Vector3 velocity = controller.velocity;
				storeVelocity.Value = velocity;
				storeX.Value = controller.velocity.x;
				storeY.Value = controller.velocity.y;
				storeZ.Value = controller.velocity.z;
			}
		}
	}
}

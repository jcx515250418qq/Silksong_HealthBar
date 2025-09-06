using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
	[ActionCategory(ActionCategory.Character)]
	[Tooltip("Checks the height clearance for a CharacterController, or, in other words, if a CharacterController can be set to a height without collisions. Often used while crouching to check if the controller has room to stand up.")]
	public class ControllerCheckHeight : ComponentAction<CharacterController>
	{
		[RequiredField]
		[CheckForComponent(typeof(CharacterController))]
		[Tooltip("The GameObject that owns the CharacterController component.")]
		public FsmOwnerDefault gameObject;

		[UIHint(UIHint.LayerMask)]
		[Tooltip("Layers to check collisions against.")]
		public FsmInt layerMask;

		[Tooltip("Height to check. The action will use a capsule of this height to check for collisions.")]
		public FsmFloat checkHeight;

		[Tooltip("Set how often to check. 0 = once, don't repeat; 1 = everyFrame; 2 = every other frame... \nBecause collision checks can get expensive use the highest repeat interval you can get away with.")]
		public FsmInt repeatInterval;

		[ActionSection("Output")]
		[Tooltip("Store if any collisions were found.")]
		public FsmBool didPass;

		[Tooltip("Event to send if no collisions were found.")]
		public FsmEvent clearEvent;

		[Tooltip("Event to send if collisions were found.")]
		public FsmEvent blockedEvent;

		private int repeat;

		private Collider[] colliders = new Collider[1];

		private CharacterController controller => cachedComponent;

		public override void Reset()
		{
			gameObject = null;
			repeatInterval = new FsmInt
			{
				Value = 1
			};
			checkHeight = null;
			layerMask = null;
			didPass = null;
			clearEvent = null;
			blockedEvent = null;
		}

		public override void OnEnter()
		{
			if (!UpdateCacheAndTransform(base.Fsm.GetOwnerDefaultTarget(gameObject)))
			{
				Finish();
				return;
			}
			DoCheck();
			if (repeatInterval.Value == 0)
			{
				Finish();
			}
		}

		public override void OnUpdate()
		{
			repeat--;
			if (repeat == 0)
			{
				DoCheck();
			}
		}

		private void DoCheck()
		{
			repeat = repeatInterval.Value;
			DoCapsuleOverlap();
			base.Fsm.Event(didPass.Value ? clearEvent : blockedEvent);
		}

		private void DoCapsuleOverlap()
		{
			float radius = controller.radius;
			Vector3 point = base.cachedTransform.TransformPoint(-(Vector3.up * (controller.height / 2f - radius)));
			Vector3 point2 = base.cachedTransform.TransformPoint(Vector3.up * (checkHeight.Value - controller.height * 0.5f - radius));
			int num = Physics.OverlapCapsuleNonAlloc(point, point2, radius, colliders, layerMask.Value);
			didPass.Value = num == 0;
		}
	}
}

using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
	[ActionCategory(ActionCategory.Character)]
	[Tooltip("Modify various character controller settings.\n'None' leaves the setting unchanged.")]
	public class ControllerSettings : ComponentAction<CharacterController>
	{
		[RequiredField]
		[CheckForComponent(typeof(CharacterController))]
		[Tooltip("The GameObject that owns the CharacterController.")]
		public FsmOwnerDefault gameObject;

		[Tooltip("The height of the character's capsule.")]
		public FsmFloat height;

		[Tooltip("The radius of the character's capsule.")]
		public FsmFloat radius;

		[Tooltip("The character controllers slope limit in degrees.")]
		public FsmFloat slopeLimit;

		[Tooltip("The character controllers step offset in meters.")]
		public FsmFloat stepOffset;

		[Tooltip("The center of the character's capsule relative to the transform's position")]
		public FsmVector3 center;

		[Tooltip("Should other RigidBodies or CharacterControllers collide with this character controller (By default always enabled).")]
		public FsmBool detectCollisions;

		[Tooltip("Repeat every frame while the state is active.")]
		public bool everyFrame;

		private CharacterController controller => cachedComponent;

		public override void Reset()
		{
			gameObject = null;
			height = new FsmFloat
			{
				UseVariable = true
			};
			radius = new FsmFloat
			{
				UseVariable = true
			};
			slopeLimit = new FsmFloat
			{
				UseVariable = true
			};
			stepOffset = new FsmFloat
			{
				UseVariable = true
			};
			center = new FsmVector3
			{
				UseVariable = true
			};
			detectCollisions = new FsmBool
			{
				UseVariable = true
			};
			everyFrame = false;
		}

		public override void OnEnter()
		{
			DoControllerSettings();
			if (!everyFrame)
			{
				Finish();
			}
		}

		public override void OnUpdate()
		{
			DoControllerSettings();
		}

		private void DoControllerSettings()
		{
			if (UpdateCache(base.Fsm.GetOwnerDefaultTarget(gameObject)))
			{
				if (!height.IsNone)
				{
					controller.height = height.Value;
				}
				if (!radius.IsNone)
				{
					controller.radius = radius.Value;
				}
				if (!slopeLimit.IsNone)
				{
					controller.slopeLimit = slopeLimit.Value;
				}
				if (!stepOffset.IsNone)
				{
					controller.stepOffset = stepOffset.Value;
				}
				if (!center.IsNone)
				{
					controller.center = center.Value;
				}
				if (!detectCollisions.IsNone)
				{
					controller.detectCollisions = detectCollisions.Value;
				}
			}
		}
	}
}

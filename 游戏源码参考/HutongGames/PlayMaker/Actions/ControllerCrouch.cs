using System.Collections.Generic;
using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
	[ActionCategory(ActionCategory.Character)]
	[Tooltip("Makes a CharacterController Crouch. Handles scaling the collider and transitions between standing and crouching.")]
	public class ControllerCrouch : ComponentAction<CharacterController>
	{
		private enum CrouchState
		{
			stand = 0,
			standToCrouch = 1,
			crouch = 2,
			crouchToStand = 3
		}

		[RequiredField]
		[CheckForComponent(typeof(CharacterController))]
		[Tooltip("The GameObject that owns the CharacterController component.")]
		public FsmOwnerDefault gameObject;

		[RequiredField]
		[Tooltip("Crouch while this to true. Normally set by an Input action like Get Key.\n\nNOTE: The controller might not be able to stand up when this is false if there's not enough headroom.")]
		public FsmBool isCrouching;

		[RequiredField]
		[Tooltip("Height of capsule when crouching.")]
		public FsmFloat crouchHeight;

		[Tooltip("Move children so their height scales with capsule. This is useful for weapon attach points etc.")]
		public FsmBool adjustChildren;

		[RequiredField]
		[Tooltip("How long it takes to crouch/stand in seconds.")]
		public FsmFloat transitionTime;

		[Tooltip("Always complete the full transition to crouching, even if the input is brief.")]
		public FsmBool completeTransition;

		[Tooltip("Can the CharacterController stand if isCrouching is false (e.g. if the crouch button is released). Usually set by a some kind of raycast checking the headroom above the controller,but could also be set to false to prevent standing for other reasons, e.g., crouch because the ground is shaking.")]
		public FsmBool canStand;

		[UIHint(UIHint.Variable)]
		[Tooltip("Try to stand if true. Useful if want to toggle crouch with a button.")]
		public FsmBool standToggle;

		[Tooltip("Event to send when crouch button is released AND there is enough headroom.")]
		public FsmEvent standEvent;

		[Tooltip("Reset the controller height if the State exits before crouch has finished. Also restores children to original offsets if Adjust Children was used.\n\nNOTE: You probably want to keep this checked most of the time.")]
		public FsmBool resetHeightOnExit;

		private float originalHeight;

		private float startTransitionHeight;

		private float transitionTimeElapsed;

		private Dictionary<Transform, float> childOffsets = new Dictionary<Transform, float>();

		private CrouchState crouchState;

		private CharacterController controller => cachedComponent;

		public override void Reset()
		{
			gameObject = null;
			isCrouching = new FsmBool
			{
				UseVariable = true
			};
			crouchHeight = new FsmFloat
			{
				Value = 0.5f
			};
			adjustChildren = new FsmBool
			{
				Value = true
			};
			transitionTime = new FsmFloat
			{
				Value = 0.2f
			};
			completeTransition = null;
			canStand = new FsmBool
			{
				Value = true
			};
			standToggle = null;
			standEvent = null;
			resetHeightOnExit = new FsmBool
			{
				Value = true
			};
		}

		public override void OnEnter()
		{
			if (!UpdateCacheAndTransform(base.Fsm.GetOwnerDefaultTarget(gameObject)))
			{
				Finish();
				return;
			}
			originalHeight = controller.height;
			crouchState = CrouchState.stand;
			transitionTimeElapsed = 0f;
			childOffsets.Clear();
			foreach (Transform item in base.cachedTransform)
			{
				childOffsets.Add(item, item.localPosition.y);
			}
		}

		public override void OnUpdate()
		{
			if (!UpdateCacheAndTransform(base.Fsm.GetOwnerDefaultTarget(gameObject)))
			{
				Finish();
				return;
			}
			switch (crouchState)
			{
			case CrouchState.stand:
				if (isCrouching.Value)
				{
					crouchState = CrouchState.standToCrouch;
					startTransitionHeight = controller.height;
					transitionTimeElapsed = 0f;
				}
				break;
			case CrouchState.standToCrouch:
			{
				float height;
				if (transitionTimeElapsed < transitionTime.Value)
				{
					height = Mathf.Lerp(startTransitionHeight, crouchHeight.Value, transitionTimeElapsed / transitionTime.Value);
					transitionTimeElapsed += Time.deltaTime;
				}
				else
				{
					height = crouchHeight.Value;
					crouchState = CrouchState.crouch;
				}
				SetHeight(height);
				if (!completeTransition.Value && !isCrouching.Value)
				{
					crouchState = CrouchState.crouchToStand;
					startTransitionHeight = controller.height;
					transitionTimeElapsed = 0f;
				}
				break;
			}
			case CrouchState.crouch:
				if (canStand.Value && (!isCrouching.Value || standToggle.Value))
				{
					crouchState = CrouchState.crouchToStand;
					startTransitionHeight = controller.height;
					transitionTimeElapsed = 0f;
				}
				break;
			case CrouchState.crouchToStand:
			{
				float height;
				if (transitionTimeElapsed < transitionTime.Value)
				{
					height = Mathf.Lerp(startTransitionHeight, originalHeight, transitionTimeElapsed / transitionTime.Value);
					transitionTimeElapsed += Time.deltaTime;
				}
				else
				{
					height = originalHeight;
					crouchState = CrouchState.stand;
				}
				SetHeight(height);
				if (crouchState == CrouchState.stand)
				{
					base.Fsm.Event(standEvent);
				}
				break;
			}
			}
		}

		private void SetHeight(float newHeight)
		{
			float num = controller.height - newHeight;
			if (controller.isGrounded)
			{
				base.cachedTransform.Translate(0f, (0f - num) * 0.5f, 0f);
			}
			controller.height = newHeight;
			if (!adjustChildren.Value)
			{
				return;
			}
			float num2 = controller.height / originalHeight;
			foreach (KeyValuePair<Transform, float> childOffset in childOffsets)
			{
				Vector3 localPosition = childOffset.Key.localPosition;
				childOffset.Key.localPosition = new Vector3(localPosition.x, childOffset.Value * num2, localPosition.z);
			}
		}

		public override void OnExit()
		{
			if (resetHeightOnExit.Value)
			{
				SetHeight(originalHeight);
				foreach (KeyValuePair<Transform, float> childOffset in childOffsets)
				{
					Vector3 localPosition = childOffset.Key.localPosition;
					childOffset.Key.localPosition = new Vector3(localPosition.x, localPosition.y, localPosition.z);
				}
			}
			childOffsets.Clear();
		}
	}
}

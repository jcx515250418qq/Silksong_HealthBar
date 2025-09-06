using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
	[ActionCategory(ActionCategory.Input)]
	[ActionTarget(typeof(GameObject), "GameObject", false)]
	[Tooltip("Sends Events based on mouse interactions with a Game Object: MouseOver, MouseDown, MouseUp, MouseOff. Use Ray Distance to set how close the camera must be to pick the object.\nNOTE: Picking uses the Main Camera, so you must have a Camera in the scene tagged as Main Camera.")]
	public class MousePickEvent : FsmStateAction
	{
		[CheckForComponent(typeof(Collider))]
		[Tooltip("The game object to test for mouse picking.")]
		public FsmOwnerDefault GameObject;

		[Tooltip("Length of the ray to cast from the camera.")]
		public FsmFloat rayDistance = 100f;

		[Tooltip("Event to send when the mouse is over the GameObject.")]
		public FsmEvent mouseOver;

		[Tooltip("Event to send when the mouse is pressed while over the GameObject.")]
		public FsmEvent mouseDown;

		[Tooltip("Event to send when the mouse is released while over the GameObject.")]
		public FsmEvent mouseUp;

		[Tooltip("Event to send when the mouse moves off the GameObject.")]
		public FsmEvent mouseOff;

		[Tooltip("Pick only from these layers.")]
		[UIHint(UIHint.Layer)]
		public FsmInt[] layerMask;

		[Tooltip("Invert the mask, so you pick from all layers except those defined above.")]
		public FsmBool invertMask;

		[Tooltip("Repeat every frame.")]
		public bool everyFrame;

		public override void Reset()
		{
			GameObject = null;
			rayDistance = 100f;
			mouseOver = null;
			mouseDown = null;
			mouseUp = null;
			mouseOff = null;
			layerMask = new FsmInt[0];
			invertMask = false;
			everyFrame = true;
		}

		public override void OnEnter()
		{
			DoMousePickEvent();
			if (!everyFrame)
			{
				Finish();
			}
		}

		public override void OnUpdate()
		{
			DoMousePickEvent();
		}

		private void DoMousePickEvent()
		{
			bool num = DoRaycast();
			base.Fsm.RaycastHitInfo = ActionHelpers.mousePickInfo;
			if (num)
			{
				if (mouseDown != null && Input.GetMouseButtonDown(0))
				{
					base.Fsm.Event(mouseDown);
				}
				if (mouseOver != null)
				{
					base.Fsm.Event(mouseOver);
				}
				if (mouseUp != null && Input.GetMouseButtonUp(0))
				{
					base.Fsm.Event(mouseUp);
				}
			}
			else if (mouseOff != null)
			{
				base.Fsm.Event(mouseOff);
			}
		}

		private bool DoRaycast()
		{
			return ActionHelpers.IsMouseOver((GameObject.OwnerOption == OwnerDefaultOption.UseOwner) ? base.Owner : GameObject.GameObject.Value, rayDistance.Value, ActionHelpers.LayerArrayToLayerMask(layerMask, invertMask.Value));
		}

		public override string ErrorCheck()
		{
			return string.Concat("" + ActionHelpers.CheckRayDistance(rayDistance.Value), ActionHelpers.CheckPhysicsSetup(base.Fsm.GetOwnerDefaultTarget(GameObject)));
		}
	}
}

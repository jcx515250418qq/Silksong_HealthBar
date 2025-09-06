using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
	[ActionCategory(ActionCategory.Input)]
	[Tooltip("Sends Events based on mouse interactions with a 2d Game Object: MouseOver, MouseDown, MouseUp, MouseOff.")]
	public class MousePick2dEvent : FsmStateAction
	{
		[CheckForComponent(typeof(Collider2D))]
		[Tooltip("The GameObject with a Collider2D attached.")]
		public FsmOwnerDefault GameObject;

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
			if (DoRaycast())
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
			GameObject gameObject = ((GameObject.OwnerOption == OwnerDefaultOption.UseOwner) ? base.Owner : GameObject.GameObject.Value);
			RaycastHit2D rayIntersection = Physics2D.GetRayIntersection(Camera.main.ScreenPointToRay(ActionHelpers.GetMousePosition()), float.PositiveInfinity, ActionHelpers.LayerArrayToLayerMask(layerMask, invertMask.Value));
			Fsm.RecordLastRaycastHit2DInfo(base.Fsm, rayIntersection);
			if (rayIntersection.transform != null && rayIntersection.transform.gameObject == gameObject)
			{
				return true;
			}
			return false;
		}
	}
}

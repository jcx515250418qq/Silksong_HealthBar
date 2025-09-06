using UnityEngine.EventSystems;

namespace HutongGames.PlayMaker.Actions
{
	[ActionCategory(ActionCategory.UI)]
	[Tooltip("Gets pointer data on the last System Event.\nHINT: Use {{Hide Unused}} in the {{State Inspector}} to hide the unused parameters after selecting the ones you need.")]
	public class UiGetLastPointerDataInfo : FsmStateAction
	{
		public static PointerEventData lastPointerEventData;

		[Tooltip("Number of clicks in a row.")]
		[UIHint(UIHint.Variable)]
		public FsmInt clickCount;

		[Tooltip("The last time a click event was sent.")]
		[UIHint(UIHint.Variable)]
		public FsmFloat clickTime;

		[Tooltip("Pointer delta since last update.")]
		[UIHint(UIHint.Variable)]
		public FsmVector2 delta;

		[Tooltip("Is a drag operation currently occuring.")]
		[UIHint(UIHint.Variable)]
		public FsmBool dragging;

		[Tooltip("The InputButton for this event.")]
		[UIHint(UIHint.Variable)]
		[ObjectType(typeof(PointerEventData.InputButton))]
		public FsmEnum inputButton;

		[Tooltip("Is the pointer being pressed? (Not documented by Unity)")]
		[UIHint(UIHint.Variable)]
		public FsmBool eligibleForClick;

		[Tooltip("The camera associated with the last OnPointerEnter event.")]
		[UIHint(UIHint.Variable)]
		public FsmGameObject enterEventCamera;

		[Tooltip("The camera associated with the last OnPointerPress event.")]
		[UIHint(UIHint.Variable)]
		public FsmGameObject pressEventCamera;

		[Tooltip("Is the pointer moving.")]
		[UIHint(UIHint.Variable)]
		public FsmBool isPointerMoving;

		[Tooltip("Is scroll being used on the input device.")]
		[UIHint(UIHint.Variable)]
		public FsmBool isScrolling;

		[Tooltip("The GameObject for the last press event.")]
		[UIHint(UIHint.Variable)]
		public FsmGameObject lastPress;

		[Tooltip("The object that is receiving OnDrag.")]
		[UIHint(UIHint.Variable)]
		public FsmGameObject pointerDrag;

		[Tooltip("The object that received 'OnPointerEnter'.")]
		[UIHint(UIHint.Variable)]
		public FsmGameObject pointerEnter;

		[Tooltip("Id of the pointer (touch id).")]
		[UIHint(UIHint.Variable)]
		public FsmInt pointerId;

		[Tooltip("The GameObject that received the OnPointerDown.")]
		[UIHint(UIHint.Variable)]
		public FsmGameObject pointerPress;

		[Tooltip("Current pointer position.")]
		[UIHint(UIHint.Variable)]
		public FsmVector2 position;

		[Tooltip("Position of the press.")]
		[UIHint(UIHint.Variable)]
		public FsmVector2 pressPosition;

		[Tooltip("The object that the press happened on even if it can not handle the press event.")]
		[UIHint(UIHint.Variable)]
		public FsmGameObject rawPointerPress;

		[Tooltip("The amount of scroll since the last update.")]
		[UIHint(UIHint.Variable)]
		public FsmVector2 scrollDelta;

		[Tooltip("Is the event used?")]
		[UIHint(UIHint.Variable)]
		public FsmBool used;

		[Tooltip("Should a drag threshold be used?")]
		[UIHint(UIHint.Variable)]
		public FsmBool useDragThreshold;

		[Tooltip("The normal of the last raycast in world coordinates.")]
		[UIHint(UIHint.Variable)]
		public FsmVector3 worldNormal;

		[Tooltip("The world position of the last raycast.")]
		[UIHint(UIHint.Variable)]
		public FsmVector3 worldPosition;

		public override void Reset()
		{
			clickCount = null;
			clickTime = null;
			delta = null;
			dragging = null;
			inputButton = PointerEventData.InputButton.Left;
			eligibleForClick = null;
			enterEventCamera = null;
			pressEventCamera = null;
			isPointerMoving = null;
			isScrolling = null;
			lastPress = null;
			pointerDrag = null;
			pointerEnter = null;
			pointerId = null;
			pointerPress = null;
			position = null;
			pressPosition = null;
			rawPointerPress = null;
			scrollDelta = null;
			used = null;
			useDragThreshold = null;
			worldNormal = null;
			worldPosition = null;
		}

		public override void OnEnter()
		{
			if (lastPointerEventData == null)
			{
				Finish();
				return;
			}
			if (!clickCount.IsNone)
			{
				clickCount.Value = lastPointerEventData.clickCount;
			}
			if (!clickTime.IsNone)
			{
				clickTime.Value = lastPointerEventData.clickTime;
			}
			if (!delta.IsNone)
			{
				delta.Value = lastPointerEventData.delta;
			}
			if (!dragging.IsNone)
			{
				dragging.Value = lastPointerEventData.dragging;
			}
			if (!inputButton.IsNone)
			{
				inputButton.Value = lastPointerEventData.button;
			}
			if (!eligibleForClick.IsNone)
			{
				eligibleForClick.Value = lastPointerEventData.eligibleForClick;
			}
			if (!enterEventCamera.IsNone)
			{
				enterEventCamera.Value = lastPointerEventData.enterEventCamera.gameObject;
			}
			if (!isPointerMoving.IsNone)
			{
				isPointerMoving.Value = lastPointerEventData.IsPointerMoving();
			}
			if (!isScrolling.IsNone)
			{
				isScrolling.Value = lastPointerEventData.IsScrolling();
			}
			if (!lastPress.IsNone)
			{
				lastPress.Value = lastPointerEventData.lastPress;
			}
			if (!pointerDrag.IsNone)
			{
				pointerDrag.Value = lastPointerEventData.pointerDrag;
			}
			if (!pointerEnter.IsNone)
			{
				pointerEnter.Value = lastPointerEventData.pointerEnter;
			}
			if (!pointerId.IsNone)
			{
				pointerId.Value = lastPointerEventData.pointerId;
			}
			if (!pointerPress.IsNone)
			{
				pointerPress.Value = lastPointerEventData.pointerPress;
			}
			if (!position.IsNone)
			{
				position.Value = lastPointerEventData.position;
			}
			if (!pressEventCamera.IsNone)
			{
				pressEventCamera.Value = lastPointerEventData.pressEventCamera.gameObject;
			}
			if (!pressPosition.IsNone)
			{
				pressPosition.Value = lastPointerEventData.pressPosition;
			}
			if (!rawPointerPress.IsNone)
			{
				rawPointerPress.Value = lastPointerEventData.rawPointerPress;
			}
			if (!scrollDelta.IsNone)
			{
				scrollDelta.Value = lastPointerEventData.scrollDelta;
			}
			if (!used.IsNone)
			{
				used.Value = lastPointerEventData.used;
			}
			if (!useDragThreshold.IsNone)
			{
				useDragThreshold.Value = lastPointerEventData.useDragThreshold;
			}
			if (!worldNormal.IsNone)
			{
				worldNormal.Value = lastPointerEventData.pointerCurrentRaycast.worldNormal;
			}
			if (!worldPosition.IsNone)
			{
				worldPosition.Value = lastPointerEventData.pointerCurrentRaycast.worldPosition;
			}
			Finish();
		}
	}
}

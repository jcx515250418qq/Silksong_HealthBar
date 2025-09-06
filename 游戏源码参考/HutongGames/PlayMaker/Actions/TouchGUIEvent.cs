using System;
using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
	[ActionCategory(ActionCategory.Device)]
	[Tooltip("Sends events when a GUI Texture or GUI Text is touched. Optionally filter by a fingerID.")]
	[Obsolete("GUIElement is part of the legacy UI system removed in 2019.3")]
	public class TouchGUIEvent : FsmStateAction
	{
		public enum OffsetOptions
		{
			TopLeft = 0,
			Center = 1,
			TouchStart = 2
		}

		[RequiredField]
		[ActionSection("Obsolete. Use Unity UI instead.")]
		[Tooltip("The Game Object that owns the GUI Texture or GUI Text.")]
		public FsmOwnerDefault gameObject;

		[Tooltip("Only detect touches that match this fingerID, or set to None.")]
		public FsmInt fingerId;

		[ActionSection("Events")]
		[Tooltip("Event to send on touch began.")]
		public FsmEvent touchBegan;

		[Tooltip("Event to send on touch moved.")]
		public FsmEvent touchMoved;

		[Tooltip("Event to send on stationary touch.")]
		public FsmEvent touchStationary;

		[Tooltip("Event to send on touch ended.")]
		public FsmEvent touchEnded;

		[Tooltip("Event to send on touch cancel.")]
		public FsmEvent touchCanceled;

		[Tooltip("Event to send if not touching (finger down but not over the GUI element)")]
		public FsmEvent notTouching;

		[ActionSection("Store Results")]
		[UIHint(UIHint.Variable)]
		[Tooltip("Store the fingerId of the touch.")]
		public FsmInt storeFingerId;

		[UIHint(UIHint.Variable)]
		[Tooltip("Store the screen position where the GUI element was touched.")]
		public FsmVector3 storeHitPoint;

		[Tooltip("Normalize the hit point screen coordinates (0-1).")]
		public FsmBool normalizeHitPoint;

		[UIHint(UIHint.Variable)]
		[Tooltip("Store the offset position of the hit.")]
		public FsmVector3 storeOffset;

		[Tooltip("How to measure the offset.")]
		public OffsetOptions relativeTo;

		[Tooltip("Normalize the offset.")]
		public FsmBool normalizeOffset;

		[ActionSection("")]
		[Tooltip("Repeat every frame.")]
		public bool everyFrame;

		private Vector3 touchStartPos;

		public override void Reset()
		{
			gameObject = null;
			fingerId = new FsmInt
			{
				UseVariable = true
			};
			touchBegan = null;
			touchMoved = null;
			touchStationary = null;
			touchEnded = null;
			touchCanceled = null;
			storeFingerId = null;
			storeHitPoint = null;
			normalizeHitPoint = false;
			storeOffset = null;
			relativeTo = OffsetOptions.Center;
			normalizeOffset = true;
			everyFrame = true;
		}

		public override void OnEnter()
		{
			Finish();
		}
	}
}

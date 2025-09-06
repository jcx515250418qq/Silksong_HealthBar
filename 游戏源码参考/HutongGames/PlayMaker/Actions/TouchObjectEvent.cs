using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
	[ActionCategory(ActionCategory.Device)]
	[ActionTarget(typeof(GameObject), "gameObject", false)]
	[Tooltip("Sends events when an object is touched. Optionally filter by a fingerID. NOTE: Uses the MainCamera!")]
	public class TouchObjectEvent : FsmStateAction
	{
		[RequiredField]
		[CheckForComponent(typeof(Collider))]
		[Tooltip("The Game Object to detect touches on.")]
		public FsmOwnerDefault gameObject;

		[RequiredField]
		[Tooltip("How far from the camera is the Game Object pickable.")]
		public FsmFloat pickDistance;

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

		[ActionSection("Store Results")]
		[UIHint(UIHint.Variable)]
		[Tooltip("Store the fingerId of the touch.")]
		public FsmInt storeFingerId;

		[UIHint(UIHint.Variable)]
		[Tooltip("Store the world position where the object was touched.")]
		public FsmVector3 storeHitPoint;

		[UIHint(UIHint.Variable)]
		[Tooltip("Store the surface normal vector where the object was touched. \nNote, this is a direction vector not a rotation. Use Look At Direction to rotate a GameObject to this direction.")]
		public FsmVector3 storeHitNormal;

		public override void Reset()
		{
			gameObject = null;
			pickDistance = 100f;
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
			storeHitNormal = null;
		}

		public override void OnUpdate()
		{
			if (Camera.main == null)
			{
				LogError("No MainCamera defined!");
				Finish();
			}
			else
			{
				if (Input.touchCount <= 0)
				{
					return;
				}
				GameObject ownerDefaultTarget = base.Fsm.GetOwnerDefaultTarget(gameObject);
				if (ownerDefaultTarget == null)
				{
					return;
				}
				Touch[] touches = Input.touches;
				for (int i = 0; i < touches.Length; i++)
				{
					Touch touch = touches[i];
					if (!fingerId.IsNone && touch.fingerId != fingerId.Value)
					{
						continue;
					}
					Vector2 position = touch.position;
					Physics.Raycast(Camera.main.ScreenPointToRay(position), out var hitInfo, pickDistance.Value);
					base.Fsm.RaycastHitInfo = hitInfo;
					if (hitInfo.transform != null && hitInfo.transform.gameObject == ownerDefaultTarget)
					{
						storeFingerId.Value = touch.fingerId;
						storeHitPoint.Value = hitInfo.point;
						storeHitNormal.Value = hitInfo.normal;
						switch (touch.phase)
						{
						case TouchPhase.Began:
							base.Fsm.Event(touchBegan);
							return;
						case TouchPhase.Moved:
							base.Fsm.Event(touchMoved);
							return;
						case TouchPhase.Stationary:
							base.Fsm.Event(touchStationary);
							return;
						case TouchPhase.Ended:
							base.Fsm.Event(touchEnded);
							return;
						case TouchPhase.Canceled:
							base.Fsm.Event(touchCanceled);
							return;
						}
					}
				}
			}
		}
	}
}

using UnityEngine;

namespace InControl
{
	public class TouchStickControl : TouchControl
	{
		[Header("Position")]
		[SerializeField]
		private TouchControlAnchor anchor = TouchControlAnchor.BottomLeft;

		[SerializeField]
		private TouchUnitType offsetUnitType;

		[SerializeField]
		private Vector2 offset = new Vector2(20f, 20f);

		[SerializeField]
		private TouchUnitType areaUnitType;

		[SerializeField]
		private Rect activeArea = new Rect(0f, 0f, 50f, 100f);

		[Header("Options")]
		public AnalogTarget target = AnalogTarget.LeftStick;

		public SnapAngles snapAngles;

		public LockAxis lockToAxis;

		[Range(0f, 1f)]
		public float lowerDeadZone = 0.1f;

		[Range(0f, 1f)]
		public float upperDeadZone = 0.9f;

		public AnimationCurve inputCurve = AnimationCurve.Linear(0f, 0f, 1f, 1f);

		public bool allowDragging;

		public DragAxis allowDraggingAxis;

		public bool snapToInitialTouch = true;

		public bool resetWhenDone = true;

		public float resetDuration = 0.1f;

		[Header("Sprites")]
		public TouchSprite ring = new TouchSprite(20f);

		public TouchSprite knob = new TouchSprite(10f);

		public float knobRange = 7.5f;

		private Vector3 resetPosition;

		private Vector3 beganPosition;

		private Vector3 movedPosition;

		private float ringResetSpeed;

		private float knobResetSpeed;

		private Rect worldActiveArea;

		private float worldKnobRange;

		private Vector3 value;

		private Touch currentTouch;

		private bool dirty;

		public bool IsActive => currentTouch != null;

		public bool IsNotActive => currentTouch == null;

		public Vector3 RingPosition
		{
			get
			{
				if (!ring.Ready)
				{
					return base.transform.position;
				}
				return ring.Position;
			}
			set
			{
				if (ring.Ready)
				{
					ring.Position = value;
				}
			}
		}

		public Vector3 KnobPosition
		{
			get
			{
				if (!knob.Ready)
				{
					return base.transform.position;
				}
				return knob.Position;
			}
			set
			{
				if (knob.Ready)
				{
					knob.Position = value;
				}
			}
		}

		public TouchControlAnchor Anchor
		{
			get
			{
				return anchor;
			}
			set
			{
				if (anchor != value)
				{
					anchor = value;
					dirty = true;
				}
			}
		}

		public Vector2 Offset
		{
			get
			{
				return offset;
			}
			set
			{
				if (offset != value)
				{
					offset = value;
					dirty = true;
				}
			}
		}

		public TouchUnitType OffsetUnitType
		{
			get
			{
				return offsetUnitType;
			}
			set
			{
				if (offsetUnitType != value)
				{
					offsetUnitType = value;
					dirty = true;
				}
			}
		}

		public Rect ActiveArea
		{
			get
			{
				return activeArea;
			}
			set
			{
				if (activeArea != value)
				{
					activeArea = value;
					dirty = true;
				}
			}
		}

		public TouchUnitType AreaUnitType
		{
			get
			{
				return areaUnitType;
			}
			set
			{
				if (areaUnitType != value)
				{
					areaUnitType = value;
					dirty = true;
				}
			}
		}

		public override void CreateControl()
		{
			ring.Create("Ring", base.transform, 1000);
			knob.Create("Knob", base.transform, 1001);
		}

		public override void DestroyControl()
		{
			ring.Delete();
			knob.Delete();
			if (currentTouch != null)
			{
				TouchEnded(currentTouch);
				currentTouch = null;
			}
		}

		public override void ConfigureControl()
		{
			resetPosition = OffsetToWorldPosition(anchor, offset, offsetUnitType, lockAspectRatio: true);
			base.transform.position = resetPosition;
			ring.Update(forceUpdate: true);
			knob.Update(forceUpdate: true);
			worldActiveArea = TouchManager.ConvertToWorld(activeArea, areaUnitType);
			worldKnobRange = TouchManager.ConvertToWorld(knobRange, knob.SizeUnitType);
		}

		public override void DrawGizmos()
		{
			ring.DrawGizmos(RingPosition, Color.yellow);
			knob.DrawGizmos(KnobPosition, Color.yellow);
			Utility.DrawCircleGizmo(RingPosition, worldKnobRange, Color.red);
			Utility.DrawRectGizmo(worldActiveArea, Color.green);
		}

		private void Update()
		{
			if (dirty)
			{
				ConfigureControl();
				dirty = false;
			}
			else
			{
				ring.Update();
				knob.Update();
			}
			if (IsNotActive)
			{
				if (resetWhenDone && KnobPosition != resetPosition)
				{
					Vector3 vector = KnobPosition - RingPosition;
					RingPosition = Vector3.MoveTowards(RingPosition, resetPosition, ringResetSpeed * Time.unscaledDeltaTime);
					KnobPosition = RingPosition + vector;
				}
				if (KnobPosition != RingPosition)
				{
					KnobPosition = Vector3.MoveTowards(KnobPosition, RingPosition, knobResetSpeed * Time.unscaledDeltaTime);
				}
			}
		}

		public override void SubmitControlState(ulong updateTick, float deltaTime)
		{
			SubmitAnalogValue(target, value, lowerDeadZone, upperDeadZone, updateTick, deltaTime);
		}

		public override void CommitControlState(ulong updateTick, float deltaTime)
		{
			CommitAnalog(target);
		}

		public override void TouchBegan(Touch touch)
		{
			if (!IsActive)
			{
				beganPosition = TouchManager.ScreenToWorldPoint(touch.position);
				bool flag = worldActiveArea.Contains(beganPosition);
				bool flag2 = ring.Contains(beganPosition);
				if (snapToInitialTouch && (flag || flag2))
				{
					RingPosition = beganPosition;
					KnobPosition = beganPosition;
					currentTouch = touch;
				}
				else if (flag2)
				{
					KnobPosition = beganPosition;
					beganPosition = RingPosition;
					currentTouch = touch;
				}
				if (IsActive)
				{
					TouchMoved(touch);
					ring.State = true;
					knob.State = true;
				}
			}
		}

		public override void TouchMoved(Touch touch)
		{
			if (currentTouch != touch)
			{
				return;
			}
			movedPosition = TouchManager.ScreenToWorldPoint(touch.position);
			if (lockToAxis == LockAxis.Horizontal && allowDraggingAxis == DragAxis.Horizontal)
			{
				movedPosition.y = beganPosition.y;
			}
			else if (lockToAxis == LockAxis.Vertical && allowDraggingAxis == DragAxis.Vertical)
			{
				movedPosition.x = beganPosition.x;
			}
			Vector3 vector = movedPosition - beganPosition;
			Vector3 normalized = vector.normalized;
			float magnitude = vector.magnitude;
			if (allowDragging)
			{
				float num = magnitude - worldKnobRange;
				if (num < 0f)
				{
					num = 0f;
				}
				Vector3 vector2 = num * normalized;
				if (allowDraggingAxis == DragAxis.Horizontal)
				{
					vector2.y = 0f;
				}
				else if (allowDraggingAxis == DragAxis.Vertical)
				{
					vector2.x = 0f;
				}
				beganPosition += vector2;
				RingPosition = beganPosition;
			}
			movedPosition = beganPosition + Mathf.Clamp(magnitude, 0f, worldKnobRange) * normalized;
			if (lockToAxis == LockAxis.Horizontal)
			{
				movedPosition.y = beganPosition.y;
			}
			else if (lockToAxis == LockAxis.Vertical)
			{
				movedPosition.x = beganPosition.x;
			}
			if (snapAngles != 0)
			{
				movedPosition = TouchControl.SnapTo(movedPosition - beganPosition, snapAngles) + beganPosition;
			}
			RingPosition = beganPosition;
			KnobPosition = movedPosition;
			value = (movedPosition - beganPosition) / worldKnobRange;
			value.x = inputCurve.Evaluate(Utility.Abs(value.x)) * Mathf.Sign(value.x);
			value.y = inputCurve.Evaluate(Utility.Abs(value.y)) * Mathf.Sign(value.y);
		}

		public override void TouchEnded(Touch touch)
		{
			if (currentTouch == touch)
			{
				value = Vector3.zero;
				float magnitude = (resetPosition - RingPosition).magnitude;
				ringResetSpeed = (Utility.IsZero(resetDuration) ? magnitude : (magnitude / resetDuration));
				float magnitude2 = (RingPosition - KnobPosition).magnitude;
				knobResetSpeed = (Utility.IsZero(resetDuration) ? knobRange : (magnitude2 / resetDuration));
				currentTouch = null;
				ring.State = false;
				knob.State = false;
			}
		}
	}
}

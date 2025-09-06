using UnityEngine;

namespace InControl
{
	public class TouchSwipeControl : TouchControl
	{
		[Header("Position")]
		[SerializeField]
		private TouchUnitType areaUnitType;

		[SerializeField]
		private Rect activeArea = new Rect(25f, 25f, 50f, 50f);

		[Header("Options")]
		[Range(0f, 1f)]
		public float sensitivity = 0.1f;

		public bool oneSwipePerTouch;

		[Header("Analog Target")]
		public AnalogTarget target;

		public SnapAngles snapAngles;

		[Header("Button Targets")]
		public ButtonTarget upTarget;

		public ButtonTarget downTarget;

		public ButtonTarget leftTarget;

		public ButtonTarget rightTarget;

		public ButtonTarget tapTarget;

		private Rect worldActiveArea;

		private Vector3 currentVector;

		private bool currentVectorIsSet;

		private Vector3 beganPosition;

		private Vector3 lastPosition;

		private Touch currentTouch;

		private bool fireButtonTarget;

		private ButtonTarget nextButtonTarget;

		private ButtonTarget lastButtonTarget;

		private bool dirty;

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
		}

		public override void DestroyControl()
		{
			if (currentTouch != null)
			{
				TouchEnded(currentTouch);
				currentTouch = null;
			}
		}

		public override void ConfigureControl()
		{
			worldActiveArea = TouchManager.ConvertToWorld(activeArea, areaUnitType);
		}

		public override void DrawGizmos()
		{
			Utility.DrawRectGizmo(worldActiveArea, Color.yellow);
		}

		private void Update()
		{
			if (dirty)
			{
				ConfigureControl();
				dirty = false;
			}
		}

		public override void SubmitControlState(ulong updateTick, float deltaTime)
		{
			Vector3 vector = TouchControl.SnapTo(currentVector, snapAngles);
			SubmitAnalogValue(target, vector, 0f, 1f, updateTick, deltaTime);
			SubmitButtonState(upTarget, fireButtonTarget && nextButtonTarget == upTarget, updateTick, deltaTime);
			SubmitButtonState(downTarget, fireButtonTarget && nextButtonTarget == downTarget, updateTick, deltaTime);
			SubmitButtonState(leftTarget, fireButtonTarget && nextButtonTarget == leftTarget, updateTick, deltaTime);
			SubmitButtonState(rightTarget, fireButtonTarget && nextButtonTarget == rightTarget, updateTick, deltaTime);
			SubmitButtonState(tapTarget, fireButtonTarget && nextButtonTarget == tapTarget, updateTick, deltaTime);
			if (fireButtonTarget && nextButtonTarget != 0)
			{
				fireButtonTarget = !oneSwipePerTouch;
				lastButtonTarget = nextButtonTarget;
				nextButtonTarget = ButtonTarget.None;
			}
		}

		public override void CommitControlState(ulong updateTick, float deltaTime)
		{
			CommitAnalog(target);
			CommitButton(upTarget);
			CommitButton(downTarget);
			CommitButton(leftTarget);
			CommitButton(rightTarget);
			CommitButton(tapTarget);
		}

		public override void TouchBegan(Touch touch)
		{
			if (currentTouch == null)
			{
				beganPosition = TouchManager.ScreenToWorldPoint(touch.position);
				if (worldActiveArea.Contains(beganPosition))
				{
					lastPosition = beganPosition;
					currentTouch = touch;
					currentVector = Vector2.zero;
					currentVectorIsSet = false;
					fireButtonTarget = true;
					nextButtonTarget = ButtonTarget.None;
					lastButtonTarget = ButtonTarget.None;
				}
			}
		}

		public override void TouchMoved(Touch touch)
		{
			if (currentTouch != touch)
			{
				return;
			}
			Vector3 vector = TouchManager.ScreenToWorldPoint(touch.position);
			Vector3 vector2 = vector - lastPosition;
			if (!(vector2.magnitude >= sensitivity))
			{
				return;
			}
			lastPosition = vector;
			if (!oneSwipePerTouch || !currentVectorIsSet)
			{
				currentVector = vector2.normalized;
				currentVectorIsSet = true;
			}
			if (fireButtonTarget)
			{
				ButtonTarget buttonTargetForVector = GetButtonTargetForVector(currentVector);
				if (buttonTargetForVector != lastButtonTarget)
				{
					nextButtonTarget = buttonTargetForVector;
				}
			}
		}

		public override void TouchEnded(Touch touch)
		{
			if (currentTouch == touch)
			{
				currentTouch = null;
				currentVector = Vector2.zero;
				currentVectorIsSet = false;
				Vector3 vector = TouchManager.ScreenToWorldPoint(touch.position);
				if ((beganPosition - vector).magnitude < sensitivity)
				{
					fireButtonTarget = true;
					nextButtonTarget = tapTarget;
					lastButtonTarget = ButtonTarget.None;
				}
				else
				{
					fireButtonTarget = false;
					nextButtonTarget = ButtonTarget.None;
					lastButtonTarget = ButtonTarget.None;
				}
			}
		}

		private ButtonTarget GetButtonTargetForVector(Vector2 vector)
		{
			Vector2 vector2 = TouchControl.SnapTo(vector, SnapAngles.Four);
			if (vector2 == Vector2.up)
			{
				return upTarget;
			}
			if (vector2 == Vector2.right)
			{
				return rightTarget;
			}
			if (vector2 == -Vector2.up)
			{
				return downTarget;
			}
			if (vector2 == -Vector2.right)
			{
				return leftTarget;
			}
			return ButtonTarget.None;
		}
	}
}

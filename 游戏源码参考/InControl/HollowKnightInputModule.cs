using UnityEngine;
using UnityEngine.EventSystems;

namespace InControl
{
	[AddComponentMenu("Event/Hollow Knight Input Module")]
	public class HollowKnightInputModule : StandaloneInputModule
	{
		[Range(0.1f, 0.9f)]
		public float analogMoveThreshold = 0.5f;

		public float moveRepeatFirstDuration = 0.8f;

		public float moveRepeatDelayDuration = 0.1f;

		public bool allowMouseInput = true;

		public bool focusOnMouseHover;

		private InputDevice inputDevice;

		private Vector3 thisMousePosition;

		private Vector3 lastMousePosition;

		private Vector2 thisVectorState;

		private Vector2 lastVectorState;

		private float nextMoveRepeatTime;

		private float lastVectorPressedTime;

		private readonly TwoAxisInputControl direction;

		public PlayerAction SubmitAction { get; set; }

		public PlayerAction CancelAction { get; set; }

		public PlayerAction JumpAction { get; set; }

		public PlayerAction CastAction { get; set; }

		public PlayerAction AttackAction { get; set; }

		public PlayerTwoAxisAction MoveAction { get; set; }

		public InputDevice Device
		{
			get
			{
				return inputDevice ?? InputManager.ActiveDevice;
			}
			set
			{
				inputDevice = value;
			}
		}

		private bool VectorIsPressed => thisVectorState != Vector2.zero;

		private bool VectorIsReleased => thisVectorState == Vector2.zero;

		private bool VectorHasChanged => thisVectorState != lastVectorState;

		private bool VectorWasPressed
		{
			get
			{
				if (VectorIsPressed && Time.realtimeSinceStartup > nextMoveRepeatTime)
				{
					return true;
				}
				if (VectorIsPressed)
				{
					return lastVectorState == Vector2.zero;
				}
				return false;
			}
		}

		private bool MouseHasMoved => (thisMousePosition - lastMousePosition).sqrMagnitude > 0f;

		private bool MouseButtonIsPressed => Input.GetMouseButtonDown(0);

		protected HollowKnightInputModule()
		{
			direction = new TwoAxisInputControl
			{
				StateThreshold = analogMoveThreshold
			};
		}

		public override void UpdateModule()
		{
			lastMousePosition = thisMousePosition;
			thisMousePosition = Input.mousePosition;
		}

		public override bool ShouldActivateModule()
		{
			if (!base.enabled || !base.gameObject.activeInHierarchy)
			{
				return false;
			}
			UpdateInputState();
			bool flag = false;
			flag |= SubmitAction.WasPressed;
			flag |= CancelAction.WasPressed;
			flag |= JumpAction.WasPressed;
			flag |= CastAction.WasPressed;
			flag |= AttackAction.WasPressed;
			flag |= VectorWasPressed;
			if (allowMouseInput)
			{
				flag |= MouseHasMoved;
				flag |= MouseButtonIsPressed;
			}
			if (Input.touchCount > 0)
			{
				flag = true;
			}
			return flag;
		}

		public override void ActivateModule()
		{
			base.ActivateModule();
			thisMousePosition = Input.mousePosition;
			lastMousePosition = Input.mousePosition;
			GameObject gameObject = base.eventSystem.currentSelectedGameObject;
			if (gameObject == null)
			{
				gameObject = base.eventSystem.firstSelectedGameObject;
			}
			base.eventSystem.SetSelectedGameObject(gameObject, GetBaseEventData());
		}

		public override void Process()
		{
			bool flag = SendUpdateEventToSelectedObject();
			if (base.eventSystem.sendNavigationEvents)
			{
				if (!flag)
				{
					flag = SendVectorEventToSelectedObject();
				}
				if (!flag)
				{
					SendButtonEventToSelectedObject();
				}
			}
			if (allowMouseInput && Cursor.visible)
			{
				ProcessMouseEvent();
			}
		}

		private void SendButtonEventToSelectedObject()
		{
			if (base.eventSystem.currentSelectedGameObject == null || UIManager.instance.IsFadingMenu)
			{
				return;
			}
			BaseEventData baseEventData = GetBaseEventData();
			switch (Platform.Current.GetMenuAction(SubmitAction.WasPressed, CancelAction.WasPressed, JumpAction.WasPressed, AttackAction.WasPressed, CastAction.WasPressed))
			{
			case Platform.MenuActions.Submit:
				ExecuteEvents.Execute(base.eventSystem.currentSelectedGameObject, baseEventData, ExecuteEvents.submitHandler);
				break;
			case Platform.MenuActions.Cancel:
			{
				PlayerAction playerAction = (AttackAction.WasPressed ? AttackAction : CastAction);
				if (!playerAction.WasPressed || playerAction.FindBinding(new MouseBindingSource(Mouse.LeftButton)) == null)
				{
					ExecuteEvents.Execute(base.eventSystem.currentSelectedGameObject, baseEventData, ExecuteEvents.cancelHandler);
				}
				break;
			}
			}
		}

		private bool SendVectorEventToSelectedObject()
		{
			if (!VectorWasPressed)
			{
				return false;
			}
			AxisEventData axisEventData = GetAxisEventData(thisVectorState.x, thisVectorState.y, 0.5f);
			if (axisEventData.moveDir != MoveDirection.None)
			{
				if (base.eventSystem.currentSelectedGameObject == null)
				{
					base.eventSystem.SetSelectedGameObject(base.eventSystem.firstSelectedGameObject, GetBaseEventData());
				}
				else
				{
					ExecuteEvents.Execute(base.eventSystem.currentSelectedGameObject, axisEventData, ExecuteEvents.moveHandler);
				}
				SetVectorRepeatTimer();
			}
			return axisEventData.used;
		}

		protected override void ProcessMove(PointerEventData pointerEvent)
		{
			GameObject pointerEnter = pointerEvent.pointerEnter;
			base.ProcessMove(pointerEvent);
			if (focusOnMouseHover && pointerEnter != pointerEvent.pointerEnter)
			{
				GameObject eventHandler = ExecuteEvents.GetEventHandler<ISelectHandler>(pointerEvent.pointerEnter);
				base.eventSystem.SetSelectedGameObject(eventHandler, pointerEvent);
			}
		}

		private void Update()
		{
			direction.Filter(Device.Direction, Time.deltaTime);
		}

		private void UpdateInputState()
		{
			lastVectorState = thisVectorState;
			thisVectorState = Vector2.zero;
			TwoAxisInputControl twoAxisInputControl = MoveAction ?? direction;
			if (Utility.AbsoluteIsOverThreshold(twoAxisInputControl.X, analogMoveThreshold))
			{
				thisVectorState.x = Mathf.Sign(twoAxisInputControl.X);
			}
			if (Utility.AbsoluteIsOverThreshold(twoAxisInputControl.Y, analogMoveThreshold))
			{
				thisVectorState.y = Mathf.Sign(twoAxisInputControl.Y);
			}
			if (VectorIsReleased)
			{
				nextMoveRepeatTime = 0f;
			}
			if (!VectorIsPressed)
			{
				return;
			}
			if (lastVectorState == Vector2.zero)
			{
				if (Time.realtimeSinceStartup > lastVectorPressedTime + 0.1f)
				{
					nextMoveRepeatTime = Time.realtimeSinceStartup + moveRepeatFirstDuration;
				}
				else
				{
					nextMoveRepeatTime = Time.realtimeSinceStartup + moveRepeatDelayDuration;
				}
			}
			lastVectorPressedTime = Time.realtimeSinceStartup;
		}

		private void SetVectorRepeatTimer()
		{
			nextMoveRepeatTime = Mathf.Max(nextMoveRepeatTime, Time.realtimeSinceStartup + moveRepeatDelayDuration);
		}
	}
}

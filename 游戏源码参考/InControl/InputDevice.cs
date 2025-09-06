using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using UnityEngine;

namespace InControl
{
	public class InputDevice
	{
		protected struct AnalogSnapshotEntry
		{
			public float value;

			public float maxValue;

			public float minValue;

			public void TrackMinMaxValue(float currentValue)
			{
				maxValue = Mathf.Max(maxValue, currentValue);
				minValue = Mathf.Min(minValue, currentValue);
			}
		}

		public static readonly InputDevice Null = new InputDevice("None");

		private readonly List<InputControl> controls;

		private bool hasLeftCommandControl;

		private InputControl leftCommandSource;

		private bool hasRightCommandControl;

		private InputControl rightCommandSource;

		public bool Passive;

		private InputControl cachedLeftStickUp;

		private InputControl cachedLeftStickDown;

		private InputControl cachedLeftStickLeft;

		private InputControl cachedLeftStickRight;

		private InputControl cachedRightStickUp;

		private InputControl cachedRightStickDown;

		private InputControl cachedRightStickLeft;

		private InputControl cachedRightStickRight;

		private InputControl cachedDPadUp;

		private InputControl cachedDPadDown;

		private InputControl cachedDPadLeft;

		private InputControl cachedDPadRight;

		private InputControl cachedAction1;

		private InputControl cachedAction2;

		private InputControl cachedAction3;

		private InputControl cachedAction4;

		private InputControl cachedLeftTrigger;

		private InputControl cachedRightTrigger;

		private InputControl cachedLeftBumper;

		private InputControl cachedRightBumper;

		private InputControl cachedLeftStickButton;

		private InputControl cachedRightStickButton;

		private InputControl cachedLeftStickX;

		private InputControl cachedLeftStickY;

		private InputControl cachedRightStickX;

		private InputControl cachedRightStickY;

		private InputControl cachedDPadX;

		private InputControl cachedDPadY;

		private InputControl cachedCommand;

		private InputControl cachedLeftCommand;

		private InputControl cachedRightCommand;

		public int CustomPlayerID { get; set; } = -1;

		public string Name { get; protected set; }

		public string Meta { get; protected set; }

		public int SortOrder { get; protected set; }

		public InputDeviceClass DeviceClass { get; protected set; }

		public InputDeviceStyle DeviceStyle { get; protected set; }

		public Guid GUID { get; private set; }

		public ulong LastInputTick { get; private set; }

		public bool IsActive { get; private set; }

		public bool IsAttached { get; private set; }

		protected bool RawSticks { get; private set; }

		public ReadOnlyCollection<InputControl> Controls { get; protected set; }

		protected InputControl[] ControlsByTarget { get; private set; }

		public TwoAxisInputControl LeftStick { get; private set; }

		public TwoAxisInputControl RightStick { get; private set; }

		public TwoAxisInputControl DPad { get; private set; }

		public InputControlType LeftCommandControl { get; private set; }

		public InputControlType RightCommandControl { get; private set; }

		protected AnalogSnapshotEntry[] AnalogSnapshot { get; set; }

		public InputControl this[InputControlType controlType] => GetControl(controlType);

		public virtual bool IsSupportedOnThisPlatform => true;

		public virtual bool IsKnown => true;

		public bool IsUnknown => !IsKnown;

		[Obsolete("Use InputDevice.CommandIsPressed instead.", false)]
		public bool MenuIsPressed
		{
			get
			{
				if (IsKnown)
				{
					return Command.IsPressed;
				}
				return false;
			}
		}

		[Obsolete("Use InputDevice.CommandWasPressed instead.", false)]
		public bool MenuWasPressed
		{
			get
			{
				if (IsKnown)
				{
					return Command.WasPressed;
				}
				return false;
			}
		}

		[Obsolete("Use InputDevice.CommandWasReleased instead.", false)]
		public bool MenuWasReleased
		{
			get
			{
				if (IsKnown)
				{
					return Command.WasReleased;
				}
				return false;
			}
		}

		public bool CommandIsPressed
		{
			get
			{
				if (IsKnown)
				{
					return Command.IsPressed;
				}
				return false;
			}
		}

		public bool CommandWasPressed
		{
			get
			{
				if (IsKnown)
				{
					return Command.WasPressed;
				}
				return false;
			}
		}

		public bool CommandWasReleased
		{
			get
			{
				if (IsKnown)
				{
					return Command.WasReleased;
				}
				return false;
			}
		}

		public InputControl AnyButton
		{
			get
			{
				int count = Controls.Count;
				for (int i = 0; i < count; i++)
				{
					InputControl inputControl = Controls[i];
					if (inputControl != null && inputControl.IsButton && inputControl.IsPressed)
					{
						return inputControl;
					}
				}
				return InputControl.Null;
			}
		}

		public bool AnyButtonIsPressed
		{
			get
			{
				int count = Controls.Count;
				for (int i = 0; i < count; i++)
				{
					InputControl inputControl = Controls[i];
					if (inputControl != null && inputControl.IsButton && inputControl.IsPressed)
					{
						return true;
					}
				}
				return false;
			}
		}

		public bool AnyButtonWasPressed
		{
			get
			{
				int count = Controls.Count;
				for (int i = 0; i < count; i++)
				{
					InputControl inputControl = Controls[i];
					if (inputControl != null && inputControl.IsButton && inputControl.WasPressed)
					{
						return true;
					}
				}
				return false;
			}
		}

		public bool AnyButtonWasReleased
		{
			get
			{
				int count = Controls.Count;
				for (int i = 0; i < count; i++)
				{
					InputControl inputControl = Controls[i];
					if (inputControl != null && inputControl.IsButton && inputControl.WasReleased)
					{
						return true;
					}
				}
				return false;
			}
		}

		public TwoAxisInputControl Direction
		{
			get
			{
				if (DPad.UpdateTick <= LeftStick.UpdateTick)
				{
					return LeftStick;
				}
				return DPad;
			}
		}

		public InputControl LeftStickUp => cachedLeftStickUp ?? (cachedLeftStickUp = GetControl(InputControlType.LeftStickUp));

		public InputControl LeftStickDown => cachedLeftStickDown ?? (cachedLeftStickDown = GetControl(InputControlType.LeftStickDown));

		public InputControl LeftStickLeft => cachedLeftStickLeft ?? (cachedLeftStickLeft = GetControl(InputControlType.LeftStickLeft));

		public InputControl LeftStickRight => cachedLeftStickRight ?? (cachedLeftStickRight = GetControl(InputControlType.LeftStickRight));

		public InputControl RightStickUp => cachedRightStickUp ?? (cachedRightStickUp = GetControl(InputControlType.RightStickUp));

		public InputControl RightStickDown => cachedRightStickDown ?? (cachedRightStickDown = GetControl(InputControlType.RightStickDown));

		public InputControl RightStickLeft => cachedRightStickLeft ?? (cachedRightStickLeft = GetControl(InputControlType.RightStickLeft));

		public InputControl RightStickRight => cachedRightStickRight ?? (cachedRightStickRight = GetControl(InputControlType.RightStickRight));

		public InputControl DPadUp => cachedDPadUp ?? (cachedDPadUp = GetControl(InputControlType.DPadUp));

		public InputControl DPadDown => cachedDPadDown ?? (cachedDPadDown = GetControl(InputControlType.DPadDown));

		public InputControl DPadLeft => cachedDPadLeft ?? (cachedDPadLeft = GetControl(InputControlType.DPadLeft));

		public InputControl DPadRight => cachedDPadRight ?? (cachedDPadRight = GetControl(InputControlType.DPadRight));

		public InputControl Action1 => cachedAction1 ?? (cachedAction1 = GetControl(InputControlType.Action1));

		public InputControl Action2 => cachedAction2 ?? (cachedAction2 = GetControl(InputControlType.Action2));

		public InputControl Action3 => cachedAction3 ?? (cachedAction3 = GetControl(InputControlType.Action3));

		public InputControl Action4 => cachedAction4 ?? (cachedAction4 = GetControl(InputControlType.Action4));

		public InputControl LeftTrigger => cachedLeftTrigger ?? (cachedLeftTrigger = GetControl(InputControlType.LeftTrigger));

		public InputControl RightTrigger => cachedRightTrigger ?? (cachedRightTrigger = GetControl(InputControlType.RightTrigger));

		public InputControl LeftBumper => cachedLeftBumper ?? (cachedLeftBumper = GetControl(InputControlType.LeftBumper));

		public InputControl RightBumper => cachedRightBumper ?? (cachedRightBumper = GetControl(InputControlType.RightBumper));

		public InputControl LeftStickButton => cachedLeftStickButton ?? (cachedLeftStickButton = GetControl(InputControlType.LeftStickButton));

		public InputControl RightStickButton => cachedRightStickButton ?? (cachedRightStickButton = GetControl(InputControlType.RightStickButton));

		public InputControl LeftStickX => cachedLeftStickX ?? (cachedLeftStickX = GetControl(InputControlType.LeftStickX));

		public InputControl LeftStickY => cachedLeftStickY ?? (cachedLeftStickY = GetControl(InputControlType.LeftStickY));

		public InputControl RightStickX => cachedRightStickX ?? (cachedRightStickX = GetControl(InputControlType.RightStickX));

		public InputControl RightStickY => cachedRightStickY ?? (cachedRightStickY = GetControl(InputControlType.RightStickY));

		public InputControl DPadX => cachedDPadX ?? (cachedDPadX = GetControl(InputControlType.DPadX));

		public InputControl DPadY => cachedDPadY ?? (cachedDPadY = GetControl(InputControlType.DPadY));

		public InputControl Command => cachedCommand ?? (cachedCommand = GetControl(InputControlType.Command));

		public InputControl LeftCommand => cachedLeftCommand ?? (cachedLeftCommand = GetControl(InputControlType.LeftCommand));

		public InputControl RightCommand => cachedRightCommand ?? (cachedRightCommand = GetControl(InputControlType.RightCommand));

		public virtual int NumUnknownAnalogs => 0;

		public virtual int NumUnknownButtons => 0;

		public InputDevice()
			: this("")
		{
		}

		public InputDevice(string name)
			: this(name, rawSticks: false)
		{
		}

		public InputDevice(string name, bool rawSticks)
		{
			Name = name;
			RawSticks = rawSticks;
			Meta = "";
			GUID = Guid.NewGuid();
			LastInputTick = 0uL;
			SortOrder = int.MaxValue;
			DeviceClass = InputDeviceClass.Unknown;
			DeviceStyle = InputDeviceStyle.Unknown;
			Passive = false;
			ControlsByTarget = new InputControl[531];
			controls = new List<InputControl>(32);
			Controls = new ReadOnlyCollection<InputControl>(controls);
			RemoveAliasControls();
		}

		internal void OnAttached()
		{
			IsAttached = true;
			AddAliasControls();
		}

		internal void OnDetached()
		{
			IsAttached = false;
			StopVibration();
			RemoveAliasControls();
		}

		private void AddAliasControls()
		{
			RemoveAliasControls();
			if (IsKnown)
			{
				LeftStick = new TwoAxisInputControl();
				RightStick = new TwoAxisInputControl();
				DPad = new TwoAxisInputControl();
				DPad.DeadZoneFunc = DeadZone.Separate;
				AddControl(InputControlType.LeftStickX, "Left Stick X");
				AddControl(InputControlType.LeftStickY, "Left Stick Y");
				AddControl(InputControlType.RightStickX, "Right Stick X");
				AddControl(InputControlType.RightStickY, "Right Stick Y");
				AddControl(InputControlType.DPadX, "DPad X");
				AddControl(InputControlType.DPadY, "DPad Y");
				AddControl(InputControlType.Command, "Command");
				LeftCommandControl = DeviceStyle.LeftCommandControl();
				leftCommandSource = GetControl(LeftCommandControl);
				hasLeftCommandControl = !leftCommandSource.IsNullControl;
				if (hasLeftCommandControl)
				{
					AddControl(InputControlType.LeftCommand, leftCommandSource.Handle);
				}
				RightCommandControl = DeviceStyle.RightCommandControl();
				rightCommandSource = GetControl(RightCommandControl);
				hasRightCommandControl = !rightCommandSource.IsNullControl;
				if (hasRightCommandControl)
				{
					AddControl(InputControlType.RightCommand, rightCommandSource.Handle);
				}
				ExpireControlCache();
			}
		}

		private void RemoveAliasControls()
		{
			LeftStick = TwoAxisInputControl.Null;
			RightStick = TwoAxisInputControl.Null;
			DPad = TwoAxisInputControl.Null;
			RemoveControl(InputControlType.LeftStickX);
			RemoveControl(InputControlType.LeftStickY);
			RemoveControl(InputControlType.RightStickX);
			RemoveControl(InputControlType.RightStickY);
			RemoveControl(InputControlType.DPadX);
			RemoveControl(InputControlType.DPadY);
			RemoveControl(InputControlType.Command);
			RemoveControl(InputControlType.LeftCommand);
			RemoveControl(InputControlType.RightCommand);
			leftCommandSource = null;
			hasLeftCommandControl = false;
			rightCommandSource = null;
			hasRightCommandControl = false;
			ExpireControlCache();
		}

		protected void ClearControls()
		{
			Array.Clear(ControlsByTarget, 0, ControlsByTarget.Length);
			controls.Clear();
			ExpireControlCache();
		}

		public bool HasControl(InputControlType controlType)
		{
			return ControlsByTarget[(int)controlType] != null;
		}

		public InputControl GetControl(InputControlType controlType)
		{
			return ControlsByTarget[(int)controlType] ?? InputControl.Null;
		}

		public static InputControlType GetInputControlTypeByName(string inputControlName)
		{
			return (InputControlType)Enum.Parse(typeof(InputControlType), inputControlName);
		}

		public InputControl GetControlByName(string controlName)
		{
			InputControlType inputControlTypeByName = GetInputControlTypeByName(controlName);
			return GetControl(inputControlTypeByName);
		}

		public InputControl AddControl(InputControlType controlType, string handle)
		{
			InputControl inputControl = ControlsByTarget[(int)controlType];
			if (inputControl == null)
			{
				inputControl = new InputControl(handle, controlType);
				ControlsByTarget[(int)controlType] = inputControl;
				controls.Add(inputControl);
				ExpireControlCache();
			}
			return inputControl;
		}

		public InputControl AddControl(InputControlType controlType, string handle, float lowerDeadZone, float upperDeadZone)
		{
			InputControl inputControl = AddControl(controlType, handle);
			inputControl.LowerDeadZone = lowerDeadZone;
			inputControl.UpperDeadZone = upperDeadZone;
			return inputControl;
		}

		private void RemoveControl(InputControlType controlType)
		{
			InputControl inputControl = ControlsByTarget[(int)controlType];
			if (inputControl != null)
			{
				ControlsByTarget[(int)controlType] = null;
				controls.Remove(inputControl);
				ExpireControlCache();
			}
		}

		public void ClearInputState()
		{
			LeftStick.ClearInputState();
			RightStick.ClearInputState();
			DPad.ClearInputState();
			int count = Controls.Count;
			for (int i = 0; i < count; i++)
			{
				Controls[i]?.ClearInputState();
			}
		}

		protected void UpdateWithState(InputControlType controlType, bool state, ulong updateTick, float deltaTime)
		{
			GetControl(controlType).UpdateWithState(state, updateTick, deltaTime);
		}

		protected void UpdateWithValue(InputControlType controlType, float value, ulong updateTick, float deltaTime)
		{
			GetControl(controlType).UpdateWithValue(value, updateTick, deltaTime);
		}

		protected void UpdateWithRawValue(InputControlType controlType, float value, ulong updateTick, float deltaTime)
		{
			GetControl(controlType).UpdateWithRawValue(value, updateTick, deltaTime);
		}

		public void UpdateLeftStickWithValue(Vector2 value, ulong updateTick, float deltaTime)
		{
			LeftStickLeft.UpdateWithValue(Mathf.Max(0f, 0f - value.x), updateTick, deltaTime);
			LeftStickRight.UpdateWithValue(Mathf.Max(0f, value.x), updateTick, deltaTime);
			if (InputManager.InvertYAxis)
			{
				LeftStickUp.UpdateWithValue(Mathf.Max(0f, 0f - value.y), updateTick, deltaTime);
				LeftStickDown.UpdateWithValue(Mathf.Max(0f, value.y), updateTick, deltaTime);
			}
			else
			{
				LeftStickUp.UpdateWithValue(Mathf.Max(0f, value.y), updateTick, deltaTime);
				LeftStickDown.UpdateWithValue(Mathf.Max(0f, 0f - value.y), updateTick, deltaTime);
			}
		}

		public void UpdateLeftStickWithRawValue(Vector2 value, ulong updateTick, float deltaTime)
		{
			LeftStickLeft.UpdateWithRawValue(Mathf.Max(0f, 0f - value.x), updateTick, deltaTime);
			LeftStickRight.UpdateWithRawValue(Mathf.Max(0f, value.x), updateTick, deltaTime);
			if (InputManager.InvertYAxis)
			{
				LeftStickUp.UpdateWithRawValue(Mathf.Max(0f, 0f - value.y), updateTick, deltaTime);
				LeftStickDown.UpdateWithRawValue(Mathf.Max(0f, value.y), updateTick, deltaTime);
			}
			else
			{
				LeftStickUp.UpdateWithRawValue(Mathf.Max(0f, value.y), updateTick, deltaTime);
				LeftStickDown.UpdateWithRawValue(Mathf.Max(0f, 0f - value.y), updateTick, deltaTime);
			}
		}

		public void CommitLeftStick()
		{
			LeftStickUp.Commit();
			LeftStickDown.Commit();
			LeftStickLeft.Commit();
			LeftStickRight.Commit();
		}

		public void UpdateRightStickWithValue(Vector2 value, ulong updateTick, float deltaTime)
		{
			RightStickLeft.UpdateWithValue(Mathf.Max(0f, 0f - value.x), updateTick, deltaTime);
			RightStickRight.UpdateWithValue(Mathf.Max(0f, value.x), updateTick, deltaTime);
			if (InputManager.InvertYAxis)
			{
				RightStickUp.UpdateWithValue(Mathf.Max(0f, 0f - value.y), updateTick, deltaTime);
				RightStickDown.UpdateWithValue(Mathf.Max(0f, value.y), updateTick, deltaTime);
			}
			else
			{
				RightStickUp.UpdateWithValue(Mathf.Max(0f, value.y), updateTick, deltaTime);
				RightStickDown.UpdateWithValue(Mathf.Max(0f, 0f - value.y), updateTick, deltaTime);
			}
		}

		public void UpdateRightStickWithRawValue(Vector2 value, ulong updateTick, float deltaTime)
		{
			RightStickLeft.UpdateWithRawValue(Mathf.Max(0f, 0f - value.x), updateTick, deltaTime);
			RightStickRight.UpdateWithRawValue(Mathf.Max(0f, value.x), updateTick, deltaTime);
			if (InputManager.InvertYAxis)
			{
				RightStickUp.UpdateWithRawValue(Mathf.Max(0f, 0f - value.y), updateTick, deltaTime);
				RightStickDown.UpdateWithRawValue(Mathf.Max(0f, value.y), updateTick, deltaTime);
			}
			else
			{
				RightStickUp.UpdateWithRawValue(Mathf.Max(0f, value.y), updateTick, deltaTime);
				RightStickDown.UpdateWithRawValue(Mathf.Max(0f, 0f - value.y), updateTick, deltaTime);
			}
		}

		public void CommitRightStick()
		{
			RightStickUp.Commit();
			RightStickDown.Commit();
			RightStickLeft.Commit();
			RightStickRight.Commit();
		}

		public virtual void Update(ulong updateTick, float deltaTime)
		{
		}

		private void ProcessLeftStick(ulong updateTick, float deltaTime)
		{
			float x = Utility.ValueFromSides(LeftStickLeft.NextRawValue, LeftStickRight.NextRawValue);
			float y = Utility.ValueFromSides(LeftStickDown.NextRawValue, LeftStickUp.NextRawValue, InputManager.InvertYAxis);
			Vector2 vector;
			if (RawSticks || LeftStickLeft.Raw || LeftStickRight.Raw || LeftStickUp.Raw || LeftStickDown.Raw)
			{
				vector = new Vector2(x, y);
			}
			else
			{
				float lowerDeadZone = Utility.Max(LeftStickLeft.LowerDeadZone, LeftStickRight.LowerDeadZone, LeftStickUp.LowerDeadZone, LeftStickDown.LowerDeadZone);
				float upperDeadZone = Utility.Min(LeftStickLeft.UpperDeadZone, LeftStickRight.UpperDeadZone, LeftStickUp.UpperDeadZone, LeftStickDown.UpperDeadZone);
				vector = LeftStick.DeadZoneFunc(x, y, lowerDeadZone, upperDeadZone);
			}
			LeftStick.Raw = true;
			LeftStick.UpdateWithAxes(vector.x, vector.y, updateTick, deltaTime);
			LeftStickX.Raw = true;
			LeftStickX.CommitWithValue(vector.x, updateTick, deltaTime);
			LeftStickY.Raw = true;
			LeftStickY.CommitWithValue(vector.y, updateTick, deltaTime);
			LeftStickLeft.SetValue(LeftStick.Left.Value, updateTick);
			LeftStickRight.SetValue(LeftStick.Right.Value, updateTick);
			LeftStickUp.SetValue(LeftStick.Up.Value, updateTick);
			LeftStickDown.SetValue(LeftStick.Down.Value, updateTick);
		}

		private void ProcessRightStick(ulong updateTick, float deltaTime)
		{
			float x = Utility.ValueFromSides(RightStickLeft.NextRawValue, RightStickRight.NextRawValue);
			float y = Utility.ValueFromSides(RightStickDown.NextRawValue, RightStickUp.NextRawValue, InputManager.InvertYAxis);
			Vector2 vector;
			if (RawSticks || RightStickLeft.Raw || RightStickRight.Raw || RightStickUp.Raw || RightStickDown.Raw)
			{
				vector = new Vector2(x, y);
			}
			else
			{
				float lowerDeadZone = Utility.Max(RightStickLeft.LowerDeadZone, RightStickRight.LowerDeadZone, RightStickUp.LowerDeadZone, RightStickDown.LowerDeadZone);
				float upperDeadZone = Utility.Min(RightStickLeft.UpperDeadZone, RightStickRight.UpperDeadZone, RightStickUp.UpperDeadZone, RightStickDown.UpperDeadZone);
				vector = RightStick.DeadZoneFunc(x, y, lowerDeadZone, upperDeadZone);
			}
			RightStick.Raw = true;
			RightStick.UpdateWithAxes(vector.x, vector.y, updateTick, deltaTime);
			RightStickX.Raw = true;
			RightStickX.CommitWithValue(vector.x, updateTick, deltaTime);
			RightStickY.Raw = true;
			RightStickY.CommitWithValue(vector.y, updateTick, deltaTime);
			RightStickLeft.SetValue(RightStick.Left.Value, updateTick);
			RightStickRight.SetValue(RightStick.Right.Value, updateTick);
			RightStickUp.SetValue(RightStick.Up.Value, updateTick);
			RightStickDown.SetValue(RightStick.Down.Value, updateTick);
		}

		private void ProcessDPad(ulong updateTick, float deltaTime)
		{
			float x = Utility.ValueFromSides(DPadLeft.NextRawValue, DPadRight.NextRawValue);
			float y = Utility.ValueFromSides(DPadDown.NextRawValue, DPadUp.NextRawValue, InputManager.InvertYAxis);
			Vector2 vector;
			if (RawSticks || DPadLeft.Raw || DPadRight.Raw || DPadUp.Raw || DPadDown.Raw)
			{
				vector = new Vector2(x, y);
			}
			else
			{
				float lowerDeadZone = Utility.Max(DPadLeft.LowerDeadZone, DPadRight.LowerDeadZone, DPadUp.LowerDeadZone, DPadDown.LowerDeadZone);
				float upperDeadZone = Utility.Min(DPadLeft.UpperDeadZone, DPadRight.UpperDeadZone, DPadUp.UpperDeadZone, DPadDown.UpperDeadZone);
				vector = DPad.DeadZoneFunc(x, y, lowerDeadZone, upperDeadZone);
			}
			DPad.Raw = true;
			DPad.UpdateWithAxes(vector.x, vector.y, updateTick, deltaTime);
			DPadX.Raw = true;
			DPadX.CommitWithValue(vector.x, updateTick, deltaTime);
			DPadY.Raw = true;
			DPadY.CommitWithValue(vector.y, updateTick, deltaTime);
			DPadLeft.SetValue(DPad.Left.Value, updateTick);
			DPadRight.SetValue(DPad.Right.Value, updateTick);
			DPadUp.SetValue(DPad.Up.Value, updateTick);
			DPadDown.SetValue(DPad.Down.Value, updateTick);
		}

		public void Commit(ulong updateTick, float deltaTime)
		{
			if (IsKnown)
			{
				ProcessLeftStick(updateTick, deltaTime);
				ProcessRightStick(updateTick, deltaTime);
				ProcessDPad(updateTick, deltaTime);
			}
			int count = Controls.Count;
			for (int i = 0; i < count; i++)
			{
				Controls[i]?.Commit();
			}
			if (IsKnown)
			{
				bool passive = true;
				bool state = false;
				for (int j = 100; j <= 116; j++)
				{
					InputControl inputControl = ControlsByTarget[j];
					if (inputControl != null && inputControl.IsPressed)
					{
						state = true;
						if (!inputControl.Passive)
						{
							passive = false;
						}
					}
				}
				Command.Passive = passive;
				Command.CommitWithState(state, updateTick, deltaTime);
				if (hasLeftCommandControl)
				{
					LeftCommand.Passive = leftCommandSource.Passive;
					LeftCommand.CommitWithState(leftCommandSource.IsPressed, updateTick, deltaTime);
				}
				if (hasRightCommandControl)
				{
					RightCommand.Passive = rightCommandSource.Passive;
					RightCommand.CommitWithState(rightCommandSource.IsPressed, updateTick, deltaTime);
				}
			}
			IsActive = false;
			for (int k = 0; k < count; k++)
			{
				InputControl inputControl2 = Controls[k];
				if (inputControl2 != null && inputControl2.HasInput && !inputControl2.Passive)
				{
					LastInputTick = updateTick;
					IsActive = true;
				}
			}
		}

		public bool LastInputAfter(InputDevice device)
		{
			if (device != null)
			{
				return LastInputTick > device.LastInputTick;
			}
			return true;
		}

		public void RequestActivation()
		{
			LastInputTick = InputManager.CurrentTick;
			IsActive = true;
		}

		public virtual void Vibrate(float leftSpeed, float rightSpeed)
		{
		}

		public void Vibrate(float intensity)
		{
			Vibrate(intensity, intensity);
		}

		public virtual void VibrateTriggers(float leftTriggerSpeed, float rightTriggerSpeed)
		{
		}

		public void StopVibration()
		{
			Vibrate(0f);
		}

		public virtual void SetLightColor(float red, float green, float blue)
		{
		}

		public void SetLightColor(Color color)
		{
			SetLightColor(color.r * color.a, color.g * color.a, color.b * color.a);
		}

		public virtual void SetLightFlash(float flashOnDuration, float flashOffDuration)
		{
		}

		public void StopLightFlash()
		{
			SetLightFlash(1f, 0f);
		}

		private void ExpireControlCache()
		{
			cachedLeftStickUp = null;
			cachedLeftStickDown = null;
			cachedLeftStickLeft = null;
			cachedLeftStickRight = null;
			cachedRightStickUp = null;
			cachedRightStickDown = null;
			cachedRightStickLeft = null;
			cachedRightStickRight = null;
			cachedDPadUp = null;
			cachedDPadDown = null;
			cachedDPadLeft = null;
			cachedDPadRight = null;
			cachedAction1 = null;
			cachedAction2 = null;
			cachedAction3 = null;
			cachedAction4 = null;
			cachedLeftTrigger = null;
			cachedRightTrigger = null;
			cachedLeftBumper = null;
			cachedRightBumper = null;
			cachedLeftStickButton = null;
			cachedRightStickButton = null;
			cachedLeftStickX = null;
			cachedLeftStickY = null;
			cachedRightStickX = null;
			cachedRightStickY = null;
			cachedDPadX = null;
			cachedDPadY = null;
			cachedCommand = null;
			cachedLeftCommand = null;
			cachedRightCommand = null;
		}

		public virtual bool ReadRawButtonState(int index)
		{
			return false;
		}

		public virtual float ReadRawAnalogValue(int index)
		{
			return 0f;
		}

		public void TakeSnapshot()
		{
			if (AnalogSnapshot == null)
			{
				AnalogSnapshot = new AnalogSnapshotEntry[NumUnknownAnalogs];
			}
			for (int i = 0; i < NumUnknownAnalogs; i++)
			{
				float value = Utility.ApplySnapping(ReadRawAnalogValue(i), 0.5f);
				AnalogSnapshot[i].value = value;
			}
		}

		public UnknownDeviceControl GetFirstPressedAnalog()
		{
			if (AnalogSnapshot != null)
			{
				for (int i = 0; i < NumUnknownAnalogs; i++)
				{
					InputControlType control = (InputControlType)(400 + i);
					float num = Utility.ApplySnapping(ReadRawAnalogValue(i), 0.5f);
					float num2 = num - AnalogSnapshot[i].value;
					AnalogSnapshot[i].TrackMinMaxValue(num);
					if (num2 > 0.1f)
					{
						num2 = AnalogSnapshot[i].maxValue - AnalogSnapshot[i].value;
					}
					if (num2 < -0.1f)
					{
						num2 = AnalogSnapshot[i].minValue - AnalogSnapshot[i].value;
					}
					if (num2 > 1.9f)
					{
						return new UnknownDeviceControl(control, InputRangeType.MinusOneToOne);
					}
					if (num2 < -0.9f)
					{
						return new UnknownDeviceControl(control, InputRangeType.ZeroToMinusOne);
					}
					if (num2 > 0.9f)
					{
						return new UnknownDeviceControl(control, InputRangeType.ZeroToOne);
					}
				}
			}
			return UnknownDeviceControl.None;
		}

		public UnknownDeviceControl GetFirstPressedButton()
		{
			for (int i = 0; i < NumUnknownButtons; i++)
			{
				if (ReadRawButtonState(i))
				{
					return new UnknownDeviceControl((InputControlType)(500 + i), InputRangeType.ZeroToOne);
				}
			}
			return UnknownDeviceControl.None;
		}
	}
}

using System;
using GlobalEnums;
using InControl;

[Serializable]
public class ControllerMapping
{
	public GamepadType gamepadType;

	public InputControlType jump = InputControlType.Action1;

	public InputControlType attack = InputControlType.Action3;

	public InputControlType dash = InputControlType.RightTrigger;

	public InputControlType cast = InputControlType.Action2;

	public InputControlType superDash = InputControlType.LeftTrigger;

	public InputControlType dreamNail = InputControlType.Action4;

	public InputControlType quickMap = InputControlType.LeftBumper;

	public InputControlType quickCast = InputControlType.RightBumper;

	public InputControlType taunt = InputControlType.RightStickButton;
}

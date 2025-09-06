using System.Collections.Generic;
using UnityEngine;

namespace InControl
{
	public class TestInputManager : MonoBehaviour
	{
		public Font font;

		private readonly GUIStyle style = new GUIStyle();

		private readonly List<LogMessage> logMessages = new List<LogMessage>();

		private bool isPaused;

		private void OnEnable()
		{
			Application.targetFrameRate = -1;
			QualitySettings.vSyncCount = 0;
			isPaused = false;
			Time.timeScale = 1f;
			Logger.OnLogMessage += delegate(LogMessage logMessage)
			{
				logMessages.Add(logMessage);
			};
			InputManager.OnDeviceAttached += delegate(InputDevice inputDevice)
			{
				Debug.Log("Attached: " + inputDevice.Name);
			};
			InputManager.OnDeviceDetached += delegate(InputDevice inputDevice)
			{
				Debug.Log("Detached: " + inputDevice.Name);
			};
			InputManager.OnActiveDeviceChanged += delegate(InputDevice inputDevice)
			{
				Debug.Log("Active device changed to: " + inputDevice.Name);
			};
			InputManager.OnUpdate += HandleInputUpdate;
		}

		private void HandleInputUpdate(ulong updateTick, float deltaTime)
		{
			CheckForPauseButton();
			int count = InputManager.Devices.Count;
			for (int i = 0; i < count; i++)
			{
				InputDevice inputDevice = InputManager.Devices[i];
				if ((bool)inputDevice.LeftBumper || (bool)inputDevice.RightBumper)
				{
					inputDevice.VibrateTriggers(inputDevice.LeftTrigger, inputDevice.RightTrigger);
					inputDevice.Vibrate(0f, 0f);
				}
				else
				{
					inputDevice.Vibrate(inputDevice.LeftTrigger, inputDevice.RightTrigger);
					inputDevice.VibrateTriggers(0f, 0f);
				}
				Color color = Color.HSVToRGB(Mathf.Repeat(Time.realtimeSinceStartup * 0.1f, 1f), 1f, 1f);
				inputDevice.SetLightColor(color.r, color.g, color.b);
			}
		}

		private void Start()
		{
		}

		private void Update()
		{
			if (Input.GetKeyDown(KeyCode.R))
			{
				Utility.LoadScene("TestInputManager");
			}
			if (Input.GetKeyDown(KeyCode.E))
			{
				InputManager.Enabled = !InputManager.Enabled;
			}
		}

		private void CheckForPauseButton()
		{
			if (Input.GetKeyDown(KeyCode.P) || InputManager.CommandWasPressed)
			{
				Time.timeScale = (isPaused ? 1f : 0f);
				isPaused = !isPaused;
			}
		}

		private void SetColor(Color color)
		{
			style.normal.textColor = color;
		}

		private void DrawUnityInputDebugger()
		{
			int num = 300;
			int num2 = Screen.width / 2;
			int num3 = 10;
			int num4 = 20;
			SetColor(Color.white);
			string[] joystickNames = Input.GetJoystickNames();
			int num5 = joystickNames.Length;
			for (int i = 0; i < num5; i++)
			{
				string text = joystickNames[i];
				int num6 = i + 1;
				GUI.Label(new Rect(num2, num3, num2 + num, num3 + 10), "Joystick " + num6 + ": \"" + text + "\"", style);
				num3 += num4;
				string text2 = "Buttons: ";
				for (int j = 0; j < 20; j++)
				{
					if (Input.GetKey("joystick " + num6 + " button " + j))
					{
						text2 = text2 + "B" + j + "  ";
					}
				}
				GUI.Label(new Rect(num2, num3, num2 + num, num3 + 10), text2, style);
				num3 += num4;
				string text3 = "Analogs: ";
				for (int k = 0; k < 20; k++)
				{
					float axisRaw = Input.GetAxisRaw("joystick " + num6 + " analog " + k);
					if (Utility.AbsoluteIsOverThreshold(axisRaw, 0.2f))
					{
						text3 = text3 + "A" + k + ": " + axisRaw.ToString("0.00") + "  ";
					}
				}
				GUI.Label(new Rect(num2, num3, num2 + num, num3 + 10), text3, style);
				num3 += num4;
				num3 += 25;
			}
		}

		private void OnDrawGizmos()
		{
			InputDevice activeDevice = InputManager.ActiveDevice;
			Vector2 vector = activeDevice.Direction.Vector;
			Gizmos.color = Color.blue;
			Vector2 vector2 = new Vector2(-3f, -1f);
			Vector2 vector3 = vector2 + vector * 2f;
			Gizmos.DrawSphere(vector2, 0.1f);
			Gizmos.DrawLine(vector2, vector3);
			Gizmos.DrawSphere(vector3, 1f);
			Gizmos.color = Color.red;
			Vector2 vector4 = new Vector2(3f, -1f);
			Vector2 vector5 = vector4 + activeDevice.RightStick.Vector * 2f;
			Gizmos.DrawSphere(vector4, 0.1f);
			Gizmos.DrawLine(vector4, vector5);
			Gizmos.DrawSphere(vector5, 1f);
		}
	}
}

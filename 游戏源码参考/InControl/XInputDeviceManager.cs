using System;
using System.Collections.Generic;
using System.Threading;
using InControl.Internal;
using InControl.UnityDeviceProfiles;
using UnityEngine;
using XInputDotNetPure;

namespace InControl
{
	public class XInputDeviceManager : InputDeviceManager
	{
		private readonly bool[] deviceConnected = new bool[4];

		private const int maxDevices = 4;

		private readonly RingBuffer<GamePadState>[] gamePadState = new RingBuffer<GamePadState>[4];

		private Thread thread;

		private readonly int timeStep;

		private int bufferSize;

		public XInputDeviceManager()
		{
			if (InputManager.XInputUpdateRate == 0)
			{
				timeStep = Mathf.FloorToInt(Time.fixedDeltaTime * 1000f);
			}
			else
			{
				timeStep = Mathf.FloorToInt(1f / (float)InputManager.XInputUpdateRate * 1000f);
			}
			bufferSize = (int)Math.Max(InputManager.XInputBufferSize, 1u);
			for (int i = 0; i < 4; i++)
			{
				gamePadState[i] = new RingBuffer<GamePadState>(bufferSize);
			}
			StartWorker();
			for (int j = 0; j < 4; j++)
			{
				devices.Add(new XInputDevice(j, this));
			}
			Update(0uL, 0f);
		}

		private void StartWorker()
		{
			if (thread == null)
			{
				thread = new Thread(Worker);
				thread.IsBackground = true;
				thread.Start();
			}
		}

		private void StopWorker()
		{
			if (thread != null)
			{
				thread.Abort();
				thread.Join();
				thread = null;
			}
		}

		private void Worker()
		{
			while (true)
			{
				for (int i = 0; i < 4; i++)
				{
					gamePadState[i].Enqueue(GamePad.GetState((PlayerIndex)i));
				}
				Thread.Sleep(timeStep);
			}
		}

		internal GamePadState GetState(int deviceIndex)
		{
			return gamePadState[deviceIndex].Dequeue();
		}

		public override void Update(ulong updateTick, float deltaTime)
		{
			for (int i = 0; i < 4; i++)
			{
				XInputDevice xInputDevice = devices[i] as XInputDevice;
				if (!xInputDevice.IsConnected)
				{
					xInputDevice.GetState();
				}
				if (xInputDevice.IsConnected != deviceConnected[i])
				{
					if (xInputDevice.IsConnected)
					{
						InputManager.AttachDevice(xInputDevice);
					}
					else
					{
						InputManager.DetachDevice(xInputDevice);
					}
					deviceConnected[i] = xInputDevice.IsConnected;
				}
			}
		}

		public override void Destroy()
		{
			StopWorker();
		}

		public static bool CheckPlatformSupport(ICollection<string> errors)
		{
			if (Application.platform != RuntimePlatform.WindowsPlayer && Application.platform != RuntimePlatform.WindowsEditor)
			{
				return false;
			}
			try
			{
				GamePad.GetState(PlayerIndex.One);
			}
			catch (DllNotFoundException ex)
			{
				errors?.Add(ex.Message + ".dll could not be found or is missing a dependency.");
				return false;
			}
			return true;
		}

		internal static void Enable()
		{
			List<string> list = new List<string>();
			if (CheckPlatformSupport(list))
			{
				InputManager.HideDevicesWithProfile(typeof(Xbox360WindowsUnityProfile));
				InputManager.HideDevicesWithProfile(typeof(XboxOneWindowsUnityProfile));
				InputManager.HideDevicesWithProfile(typeof(XboxOneWindows10UnityProfile));
				InputManager.HideDevicesWithProfile(typeof(XboxOneWindows10AEUnityProfile));
				InputManager.HideDevicesWithProfile(typeof(LogitechF310ModeXWindowsUnityProfile));
				InputManager.HideDevicesWithProfile(typeof(LogitechF510ModeXWindowsUnityProfile));
				InputManager.HideDevicesWithProfile(typeof(LogitechF710ModeXWindowsUnityProfile));
				InputManager.AddDeviceManager<XInputDeviceManager>();
				return;
			}
			foreach (string item in list)
			{
				Logger.LogError(item);
			}
		}
	}
}

namespace InControl
{
	public class UnknownDeviceBindingSourceListener : BindingSourceListener
	{
		private enum DetectPhase
		{
			WaitForInitialRelease = 0,
			WaitForControlPress = 1,
			WaitForControlRelease = 2
		}

		private UnknownDeviceControl detectFound;

		private DetectPhase detectPhase;

		public void Reset()
		{
			detectFound = UnknownDeviceControl.None;
			detectPhase = DetectPhase.WaitForInitialRelease;
			TakeSnapshotOnUnknownDevices();
		}

		private void TakeSnapshotOnUnknownDevices()
		{
			int count = InputManager.Devices.Count;
			for (int i = 0; i < count; i++)
			{
				InputDevice inputDevice = InputManager.Devices[i];
				if (inputDevice.IsUnknown)
				{
					inputDevice.TakeSnapshot();
				}
			}
		}

		public BindingSource Listen(BindingListenOptions listenOptions, InputDevice device)
		{
			if (!listenOptions.IncludeUnknownControllers || device.IsKnown)
			{
				return null;
			}
			if (detectPhase == DetectPhase.WaitForControlRelease && (bool)detectFound && !IsPressed(detectFound, device))
			{
				UnknownDeviceBindingSource result = new UnknownDeviceBindingSource(detectFound);
				Reset();
				return result;
			}
			UnknownDeviceControl unknownDeviceControl = ListenForControl(listenOptions, device);
			if ((bool)unknownDeviceControl)
			{
				if (detectPhase == DetectPhase.WaitForControlPress)
				{
					detectFound = unknownDeviceControl;
					detectPhase = DetectPhase.WaitForControlRelease;
				}
			}
			else if (detectPhase == DetectPhase.WaitForInitialRelease)
			{
				detectPhase = DetectPhase.WaitForControlPress;
			}
			return null;
		}

		private bool IsPressed(UnknownDeviceControl control, InputDevice device)
		{
			return Utility.AbsoluteIsOverThreshold(control.GetValue(device), 0.5f);
		}

		private UnknownDeviceControl ListenForControl(BindingListenOptions listenOptions, InputDevice device)
		{
			if (device.IsUnknown)
			{
				UnknownDeviceControl firstPressedButton = device.GetFirstPressedButton();
				if ((bool)firstPressedButton)
				{
					return firstPressedButton;
				}
				UnknownDeviceControl firstPressedAnalog = device.GetFirstPressedAnalog();
				if ((bool)firstPressedAnalog)
				{
					return firstPressedAnalog;
				}
			}
			return UnknownDeviceControl.None;
		}
	}
}

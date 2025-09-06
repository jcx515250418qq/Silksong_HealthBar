namespace InControl
{
	public class DeviceBindingSourceListener : BindingSourceListener
	{
		private InputControlType detectFound;

		private int detectPhase;

		public void Reset()
		{
			detectFound = InputControlType.None;
			detectPhase = 0;
		}

		public BindingSource Listen(BindingListenOptions listenOptions, InputDevice device)
		{
			if (!listenOptions.IncludeControllers || device.IsUnknown)
			{
				return null;
			}
			if (detectFound != 0 && !IsPressed(detectFound, device) && detectPhase == 2)
			{
				DeviceBindingSource result = new DeviceBindingSource(detectFound);
				Reset();
				return result;
			}
			InputControlType inputControlType = ListenForControl(listenOptions, device);
			if (inputControlType != 0)
			{
				if (detectPhase == 1)
				{
					detectFound = inputControlType;
					detectPhase = 2;
				}
			}
			else if (detectPhase == 0)
			{
				detectPhase = 1;
			}
			return null;
		}

		private bool IsPressed(InputControl control)
		{
			return Utility.AbsoluteIsOverThreshold(control.Value, 0.5f);
		}

		private bool IsPressed(InputControlType control, InputDevice device)
		{
			return IsPressed(device.GetControl(control));
		}

		private InputControlType ListenForControl(BindingListenOptions listenOptions, InputDevice device)
		{
			if (device.IsKnown)
			{
				int count = device.Controls.Count;
				for (int i = 0; i < count; i++)
				{
					InputControl inputControl = device.Controls[i];
					if (inputControl != null && IsPressed(inputControl) && (listenOptions.IncludeNonStandardControls || inputControl.IsStandard))
					{
						InputControlType target = inputControl.Target;
						if (target != InputControlType.Command || !listenOptions.IncludeNonStandardControls)
						{
							return target;
						}
					}
				}
			}
			return InputControlType.None;
		}
	}
}

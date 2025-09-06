namespace InControl
{
	public class MouseBindingSourceListener : BindingSourceListener
	{
		public static float ScrollWheelThreshold = 0.001f;

		private Mouse detectFound;

		private int detectPhase;

		public void Reset()
		{
			detectFound = Mouse.None;
			detectPhase = 0;
		}

		public BindingSource Listen(BindingListenOptions listenOptions, InputDevice device)
		{
			if (detectFound != 0 && !IsPressed(detectFound) && detectPhase == 2)
			{
				MouseBindingSource result = new MouseBindingSource(detectFound);
				Reset();
				return result;
			}
			Mouse mouse = ListenForControl(listenOptions);
			if (mouse != 0)
			{
				if (detectPhase == 1)
				{
					detectFound = mouse;
					detectPhase = 2;
				}
			}
			else if (detectPhase == 0)
			{
				detectPhase = 1;
			}
			return null;
		}

		private bool IsPressed(Mouse control)
		{
			return control switch
			{
				Mouse.NegativeScrollWheel => MouseBindingSource.NegativeScrollWheelIsActive(ScrollWheelThreshold), 
				Mouse.PositiveScrollWheel => MouseBindingSource.PositiveScrollWheelIsActive(ScrollWheelThreshold), 
				_ => MouseBindingSource.ButtonIsPressed(control), 
			};
		}

		private Mouse ListenForControl(BindingListenOptions listenOptions)
		{
			if (listenOptions.IncludeMouseButtons)
			{
				for (Mouse mouse = Mouse.None; mouse <= Mouse.Button7; mouse++)
				{
					if (MouseBindingSource.ButtonIsPressed(mouse))
					{
						return mouse;
					}
				}
			}
			if (listenOptions.IncludeMouseScrollWheel)
			{
				if (MouseBindingSource.NegativeScrollWheelIsActive(ScrollWheelThreshold))
				{
					return Mouse.NegativeScrollWheel;
				}
				if (MouseBindingSource.PositiveScrollWheelIsActive(ScrollWheelThreshold))
				{
					return Mouse.PositiveScrollWheel;
				}
			}
			return Mouse.None;
		}
	}
}

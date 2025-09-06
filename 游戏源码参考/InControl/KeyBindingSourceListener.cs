namespace InControl
{
	public class KeyBindingSourceListener : BindingSourceListener
	{
		private KeyCombo detectFound;

		private int detectPhase;

		public void Reset()
		{
			detectFound.Clear();
			detectPhase = 0;
		}

		public BindingSource Listen(BindingListenOptions listenOptions, InputDevice device)
		{
			if (!listenOptions.IncludeKeys)
			{
				return null;
			}
			if (detectFound.IncludeCount > 0 && !detectFound.IsPressed && detectPhase == 2)
			{
				KeyBindingSource result = new KeyBindingSource(detectFound);
				Reset();
				return result;
			}
			KeyCombo keyCombo = KeyCombo.Detect(listenOptions.IncludeModifiersAsFirstClassKeys);
			if (keyCombo.IncludeCount > 0)
			{
				if (detectPhase == 1)
				{
					detectFound = keyCombo;
					detectPhase = 2;
				}
			}
			else if (detectPhase == 0)
			{
				detectPhase = 1;
			}
			return null;
		}
	}
}

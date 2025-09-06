namespace InControl
{
	public struct InputControlState
	{
		public bool State;

		public float Value;

		public float RawValue;

		public void Reset()
		{
			State = false;
			Value = 0f;
			RawValue = 0f;
		}

		public void Set(float value)
		{
			Value = value;
			State = Utility.IsNotZero(value);
		}

		public void Set(float value, float threshold)
		{
			Value = value;
			State = Utility.AbsoluteIsOverThreshold(value, threshold);
		}

		public void Set(bool state)
		{
			State = state;
			Value = (state ? 1f : 0f);
			RawValue = Value;
		}

		public static implicit operator bool(InputControlState state)
		{
			return state.State;
		}

		public static implicit operator float(InputControlState state)
		{
			return state.Value;
		}

		public static bool operator ==(InputControlState a, InputControlState b)
		{
			if (a.State == b.State)
			{
				return Utility.Approximately(a.Value, b.Value);
			}
			return false;
		}

		public static bool operator !=(InputControlState a, InputControlState b)
		{
			if (a.State == b.State)
			{
				return !Utility.Approximately(a.Value, b.Value);
			}
			return true;
		}
	}
}

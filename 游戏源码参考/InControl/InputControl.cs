namespace InControl
{
	public class InputControl : OneAxisInputControl
	{
		public static readonly InputControl Null = new InputControl
		{
			isNullControl = true
		};

		public bool Passive;

		private ulong zeroTick;

		public string Handle { get; protected set; }

		public InputControlType Target { get; protected set; }

		public bool IsButton { get; protected set; }

		public bool IsAnalog { get; protected set; }

		internal bool IsOnZeroTick => base.UpdateTick == zeroTick;

		public bool IsStandard => Utility.TargetIsStandard(Target);

		private InputControl()
		{
			Handle = "None";
			Target = InputControlType.None;
			Passive = false;
			IsButton = false;
			IsAnalog = false;
		}

		public InputControl(string handle, InputControlType target)
		{
			Handle = handle;
			Target = target;
			Passive = false;
			IsButton = Utility.TargetIsButton(target);
			IsAnalog = !IsButton;
		}

		public InputControl(string handle, InputControlType target, bool passive)
			: this(handle, target)
		{
			Passive = passive;
		}

		internal void SetZeroTick()
		{
			zeroTick = base.UpdateTick;
		}
	}
}

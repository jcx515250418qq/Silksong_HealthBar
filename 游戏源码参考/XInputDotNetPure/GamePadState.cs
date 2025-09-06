namespace XInputDotNetPure
{
	public struct GamePadState
	{
		internal struct RawState
		{
			public struct GamePad
			{
				public ushort dwButtons;

				public byte bLeftTrigger;

				public byte bRightTrigger;

				public short sThumbLX;

				public short sThumbLY;

				public short sThumbRX;

				public short sThumbRY;
			}

			public uint dwPacketNumber;

			public GamePad Gamepad;
		}

		private enum ButtonsConstants
		{
			DPadUp = 1,
			DPadDown = 2,
			DPadLeft = 4,
			DPadRight = 8,
			Start = 0x10,
			Back = 0x20,
			LeftThumb = 0x40,
			RightThumb = 0x80,
			LeftShoulder = 0x100,
			RightShoulder = 0x200,
			A = 0x1000,
			B = 0x2000,
			X = 0x4000,
			Y = 0x8000
		}

		private bool isConnected;

		private uint packetNumber;

		private GamePadButtons buttons;

		private GamePadDPad dPad;

		private GamePadThumbSticks thumbSticks;

		private GamePadTriggers triggers;

		public uint PacketNumber => packetNumber;

		public bool IsConnected => isConnected;

		public GamePadButtons Buttons => buttons;

		public GamePadDPad DPad => dPad;

		public GamePadTriggers Triggers => triggers;

		public GamePadThumbSticks ThumbSticks => thumbSticks;

		internal GamePadState(bool isConnected, RawState rawState)
		{
			this.isConnected = isConnected;
			if (!isConnected)
			{
				rawState.dwPacketNumber = 0u;
				rawState.Gamepad.dwButtons = 0;
				rawState.Gamepad.bLeftTrigger = 0;
				rawState.Gamepad.bRightTrigger = 0;
				rawState.Gamepad.sThumbLX = 0;
				rawState.Gamepad.sThumbLY = 0;
				rawState.Gamepad.sThumbRX = 0;
				rawState.Gamepad.sThumbRY = 0;
			}
			packetNumber = rawState.dwPacketNumber;
			buttons = new GamePadButtons(((rawState.Gamepad.dwButtons & 0x10) == 0) ? ButtonState.Released : ButtonState.Pressed, ((rawState.Gamepad.dwButtons & 0x20) == 0) ? ButtonState.Released : ButtonState.Pressed, ((rawState.Gamepad.dwButtons & 0x40) == 0) ? ButtonState.Released : ButtonState.Pressed, ((rawState.Gamepad.dwButtons & 0x80) == 0) ? ButtonState.Released : ButtonState.Pressed, ((rawState.Gamepad.dwButtons & 0x100) == 0) ? ButtonState.Released : ButtonState.Pressed, ((rawState.Gamepad.dwButtons & 0x200) == 0) ? ButtonState.Released : ButtonState.Pressed, ((rawState.Gamepad.dwButtons & 0x1000) == 0) ? ButtonState.Released : ButtonState.Pressed, ((rawState.Gamepad.dwButtons & 0x2000) == 0) ? ButtonState.Released : ButtonState.Pressed, ((rawState.Gamepad.dwButtons & 0x4000) == 0) ? ButtonState.Released : ButtonState.Pressed, ((rawState.Gamepad.dwButtons & 0x8000) == 0) ? ButtonState.Released : ButtonState.Pressed);
			dPad = new GamePadDPad(((rawState.Gamepad.dwButtons & 1) == 0) ? ButtonState.Released : ButtonState.Pressed, ((rawState.Gamepad.dwButtons & 2) == 0) ? ButtonState.Released : ButtonState.Pressed, ((rawState.Gamepad.dwButtons & 4) == 0) ? ButtonState.Released : ButtonState.Pressed, ((rawState.Gamepad.dwButtons & 8) == 0) ? ButtonState.Released : ButtonState.Pressed);
			thumbSticks = new GamePadThumbSticks(new GamePadThumbSticks.StickValue((float)rawState.Gamepad.sThumbLX / 32767f, (float)rawState.Gamepad.sThumbLY / 32767f), new GamePadThumbSticks.StickValue((float)rawState.Gamepad.sThumbRX / 32767f, (float)rawState.Gamepad.sThumbRY / 32767f));
			triggers = new GamePadTriggers((float)(int)rawState.Gamepad.bLeftTrigger / 255f, (float)(int)rawState.Gamepad.bRightTrigger / 255f);
		}
	}
}

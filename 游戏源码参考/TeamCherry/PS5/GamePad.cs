using UnityEngine;

namespace TeamCherry.PS5
{
	public sealed class GamePad
	{
		public struct PS4GamePad
		{
			public Vector2 thumbstick_left;

			public Vector2 thumbstick_right;

			public bool cross;

			public bool circle;

			public bool triangle;

			public bool square;

			public bool dpad_down;

			public bool dpad_right;

			public bool dpad_up;

			public bool dpad_left;

			public bool L1;

			public bool L2;

			public bool L3;

			public bool R1;

			public bool R2;

			public bool R3;

			public bool options;

			public bool touchpad;

			public bool isTouching;

			public int touchNum;

			public int touch0X;

			public int touch0Y;

			public int touch0ID;

			public int touch1X;

			public int touch1Y;

			public int touch1ID;
		}

		public sealed class SwipeInput
		{
			private readonly GamePad gamePad;

			public bool Up => gamePad.up;

			public bool Right => gamePad.right;

			public bool Down => gamePad.down;

			public bool Left => gamePad.left;

			public SwipeInput(GamePad gamePad)
			{
				this.gamePad = gamePad;
			}
		}

		public static GamePad activeGamePad;

		public static GamePad[] gamePads;

		private static bool initialised;

		private static bool enableInput;

		private static float timeout;

		public int playerId = -1;

		public int refreshInterval = 10;

		private const float SWIPE_THRESHOLD = 300f;

		private const float SWIPE_THRESHOLD_SQR = 90000f;

		private bool up;

		private bool right;

		private bool down;

		private bool left;

		public Vector2 touchStart = Vector2.zero;

		public PS4GamePad previousFrame;

		public PS4GamePad currentFrame;

		private bool hasSetupGamepad;

		private PSInputBase input;

		public static bool IsInputEnabled => enableInput;

		public bool IsSquarePressed
		{
			get
			{
				if (!previousFrame.square)
				{
					return currentFrame.square;
				}
				return false;
			}
		}

		public bool IsCirclePressed
		{
			get
			{
				if (!previousFrame.circle)
				{
					return currentFrame.circle;
				}
				return false;
			}
		}

		public bool IsTrianglePressed
		{
			get
			{
				if (!previousFrame.triangle)
				{
					return currentFrame.triangle;
				}
				return false;
			}
		}

		public bool IsCrossPressed
		{
			get
			{
				if (!previousFrame.cross)
				{
					return currentFrame.cross;
				}
				return false;
			}
		}

		public bool IsDpadDownPressed
		{
			get
			{
				if (!previousFrame.dpad_down)
				{
					return currentFrame.dpad_down;
				}
				return false;
			}
		}

		public bool IsDpadRightPressed
		{
			get
			{
				if (!previousFrame.dpad_right)
				{
					return currentFrame.dpad_right;
				}
				return false;
			}
		}

		public bool IsDpadUpPressed
		{
			get
			{
				if (!previousFrame.dpad_up)
				{
					return currentFrame.dpad_up;
				}
				return false;
			}
		}

		public bool IsDpadLeftPressed
		{
			get
			{
				if (!previousFrame.dpad_left)
				{
					return currentFrame.dpad_left;
				}
				return false;
			}
		}

		public bool IsR3Pressed
		{
			get
			{
				if (!previousFrame.R3)
				{
					return currentFrame.R3;
				}
				return false;
			}
		}

		public Vector2 GetThumbstickLeft => currentFrame.thumbstick_left;

		public Vector2 GetThumbstickRight => currentFrame.thumbstick_right;

		public SwipeInput Swipes { get; private set; }

		public bool IsTouchpadPressed
		{
			get
			{
				if (!previousFrame.touchpad)
				{
					return currentFrame.touchpad;
				}
				return false;
			}
		}

		private bool AnyInput
		{
			get
			{
				if (currentFrame.cross || currentFrame.circle || currentFrame.triangle || currentFrame.square || currentFrame.dpad_down || currentFrame.dpad_right || currentFrame.dpad_up || currentFrame.dpad_left || currentFrame.L1 || currentFrame.L2 || currentFrame.L3 || currentFrame.R1 || currentFrame.R2 || currentFrame.R3)
				{
					return true;
				}
				if (currentFrame.options || currentFrame.touchpad)
				{
					return true;
				}
				if (currentFrame.touchNum > 0)
				{
					return true;
				}
				if (Vector2.SqrMagnitude(currentFrame.thumbstick_left) > 0f)
				{
					return true;
				}
				if (Vector2.SqrMagnitude(currentFrame.thumbstick_right) > 0f)
				{
					return true;
				}
				return false;
			}
		}

		public bool IsConnected => false;

		public GamePad()
		{
			Swipes = new SwipeInput(this);
		}

		static GamePad()
		{
			activeGamePad = null;
			enableInput = true;
			timeout = 0f;
			gamePads = new GamePad[4];
			for (int i = 0; i < gamePads.Length; i++)
			{
				gamePads[i] = new GamePad();
				gamePads[i].playerId = i;
				gamePads[i].refreshInterval = i + 10;
				gamePads[i].RefreshUserDetails();
			}
			activeGamePad = gamePads[0];
		}

		public static void Initialize()
		{
			if (!initialised)
			{
				initialised = true;
				if ((bool)Platform.Current)
				{
					Platform.Current.gameObject.AddComponent<PlaystationGamePadManager>();
					return;
				}
				GameObject gameObject = new GameObject("Playstation Gamepads");
				Object.DontDestroyOnLoad(gameObject);
				gameObject.AddComponent<PlaystationGamePadManager>();
			}
		}

		public static void EnableInput(bool enable)
		{
			if (enable != enableInput)
			{
				enableInput = enable;
				if (enable)
				{
					timeout = 1f;
				}
			}
		}

		public static GamePad GetGamepad(int playerID)
		{
			if (playerID < 0 || playerID > 4)
			{
				return null;
			}
			return gamePads[playerID];
		}

		public void InitGamepad()
		{
			ToggleGamePad(active: false);
			input = PSInputBase.GetPSInput();
			input.Init(this);
		}

		public void RefreshUserDetails()
		{
		}

		public void Update()
		{
			if (timeout > 0f)
			{
				timeout -= Time.deltaTime;
			}
			if (!enableInput || timeout > 0f)
			{
				previousFrame = default(PS4GamePad);
				currentFrame = default(PS4GamePad);
			}
		}

		private void ToggleGamePad(bool active)
		{
			if (active)
			{
				RefreshUserDetails();
				hasSetupGamepad = true;
			}
			else
			{
				hasSetupGamepad = false;
			}
		}

		private void Thumbsticks()
		{
			currentFrame.thumbstick_left = input.GetThumbStickLeft();
			currentFrame.thumbstick_right = input.GetThumbStickRight();
			currentFrame.L3 = input.GetL3();
			currentFrame.R3 = input.GetR3();
		}

		private void InputButtons()
		{
			currentFrame.cross = input.GetCross();
			currentFrame.circle = input.GetCircle();
			currentFrame.square = input.GetSquare();
			currentFrame.triangle = input.GetTriangle();
		}

		private void DPadButtons()
		{
			currentFrame.dpad_right = input.GetDpadRight();
			currentFrame.dpad_left = input.GetDpadLeft();
			currentFrame.dpad_up = input.GetDpadUp();
			currentFrame.dpad_down = input.GetDpadDown();
		}

		private void TriggerShoulderButtons()
		{
			currentFrame.L2 = input.GetL2();
			currentFrame.R2 = input.GetR2();
			currentFrame.L1 = input.GetL1();
			currentFrame.R1 = input.GetR1();
		}

		private void TouchPad()
		{
			currentFrame.touchpad = input.TouchPadButton();
			currentFrame.isTouching = currentFrame.touchNum > 0 || currentFrame.touchpad;
			if (previousFrame.touchNum == 0 && currentFrame.touchNum > 0)
			{
				touchStart = new Vector2(currentFrame.touch0X, currentFrame.touch0Y);
			}
			up = false;
			right = false;
			down = false;
			left = false;
			if (previousFrame.touchNum <= 0 || currentFrame.touchNum != 0)
			{
				return;
			}
			Vector2 vector = new Vector2(previousFrame.touch0X, previousFrame.touch0Y) - touchStart;
			if (!(vector.sqrMagnitude >= 90000f))
			{
				return;
			}
			if (Mathf.Abs(vector.x) > Mathf.Abs(vector.y))
			{
				if (vector.x < 0f)
				{
					left = true;
				}
				else
				{
					right = true;
				}
			}
			else if (vector.y < 0f)
			{
				up = true;
			}
			else
			{
				down = true;
			}
		}
	}
}

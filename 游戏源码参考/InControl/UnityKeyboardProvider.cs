using UnityEngine;

namespace InControl
{
	public class UnityKeyboardProvider : IKeyboardProvider
	{
		public readonly struct KeyMapping
		{
			private readonly Key source;

			private readonly KeyCode target0;

			private readonly KeyCode target1;

			private readonly string name;

			private readonly string macName;

			public bool IsPressed
			{
				get
				{
					if (target0 != 0 && Input.GetKey(target0))
					{
						return true;
					}
					if (target1 != 0 && Input.GetKey(target1))
					{
						return true;
					}
					return false;
				}
			}

			public string Name
			{
				get
				{
					if (Application.platform == RuntimePlatform.OSXEditor || Application.platform == RuntimePlatform.OSXPlayer)
					{
						return macName;
					}
					return name;
				}
			}

			public KeyMapping(Key source, string name, KeyCode target)
			{
				this.source = source;
				this.name = name;
				macName = name;
				target0 = target;
				target1 = KeyCode.None;
			}

			public KeyMapping(Key source, string name, KeyCode target0, KeyCode target1)
			{
				this.source = source;
				this.name = name;
				macName = name;
				this.target0 = target0;
				this.target1 = target1;
			}

			public KeyMapping(Key source, string name, string macName, KeyCode target)
			{
				this.source = source;
				this.name = name;
				this.macName = macName;
				target0 = target;
				target1 = KeyCode.None;
			}

			public KeyMapping(Key source, string name, string macName, KeyCode target0, KeyCode target1)
			{
				this.source = source;
				this.name = name;
				this.macName = macName;
				this.target0 = target0;
				this.target1 = target1;
			}
		}

		public static readonly KeyMapping[] KeyMappings = new KeyMapping[132]
		{
			new KeyMapping(Key.None, "None", KeyCode.None),
			new KeyMapping(Key.Shift, "Shift", KeyCode.LeftShift, KeyCode.RightShift),
			new KeyMapping(Key.Alt, "Alt", "Option", KeyCode.LeftAlt, KeyCode.RightAlt),
			new KeyMapping(Key.Command, "Command", KeyCode.LeftMeta, KeyCode.RightMeta),
			new KeyMapping(Key.Control, "Control", KeyCode.LeftControl, KeyCode.RightControl),
			new KeyMapping(Key.LeftShift, "Left Shift", KeyCode.LeftShift),
			new KeyMapping(Key.LeftAlt, "Left Alt", "Left Option", KeyCode.LeftAlt),
			new KeyMapping(Key.LeftCommand, "Left Command", KeyCode.LeftMeta),
			new KeyMapping(Key.LeftControl, "Left Control", KeyCode.LeftControl),
			new KeyMapping(Key.RightShift, "Right Shift", KeyCode.RightShift),
			new KeyMapping(Key.RightAlt, "Right Alt", "Right Option", KeyCode.RightAlt),
			new KeyMapping(Key.RightCommand, "Right Command", KeyCode.RightMeta),
			new KeyMapping(Key.RightControl, "Right Control", KeyCode.RightControl),
			new KeyMapping(Key.Escape, "Escape", KeyCode.Escape),
			new KeyMapping(Key.F1, "F1", KeyCode.F1),
			new KeyMapping(Key.F2, "F2", KeyCode.F2),
			new KeyMapping(Key.F3, "F3", KeyCode.F3),
			new KeyMapping(Key.F4, "F4", KeyCode.F4),
			new KeyMapping(Key.F5, "F5", KeyCode.F5),
			new KeyMapping(Key.F6, "F6", KeyCode.F6),
			new KeyMapping(Key.F7, "F7", KeyCode.F7),
			new KeyMapping(Key.F8, "F8", KeyCode.F8),
			new KeyMapping(Key.F9, "F9", KeyCode.F9),
			new KeyMapping(Key.F10, "F10", KeyCode.F10),
			new KeyMapping(Key.F11, "F11", KeyCode.F11),
			new KeyMapping(Key.F12, "F12", KeyCode.F12),
			new KeyMapping(Key.Key0, "Num 0", KeyCode.Alpha0),
			new KeyMapping(Key.Key1, "Num 1", KeyCode.Alpha1),
			new KeyMapping(Key.Key2, "Num 2", KeyCode.Alpha2),
			new KeyMapping(Key.Key3, "Num 3", KeyCode.Alpha3),
			new KeyMapping(Key.Key4, "Num 4", KeyCode.Alpha4),
			new KeyMapping(Key.Key5, "Num 5", KeyCode.Alpha5),
			new KeyMapping(Key.Key6, "Num 6", KeyCode.Alpha6),
			new KeyMapping(Key.Key7, "Num 7", KeyCode.Alpha7),
			new KeyMapping(Key.Key8, "Num 8", KeyCode.Alpha8),
			new KeyMapping(Key.Key9, "Num 9", KeyCode.Alpha9),
			new KeyMapping(Key.A, "A", KeyCode.A),
			new KeyMapping(Key.B, "B", KeyCode.B),
			new KeyMapping(Key.C, "C", KeyCode.C),
			new KeyMapping(Key.D, "D", KeyCode.D),
			new KeyMapping(Key.E, "E", KeyCode.E),
			new KeyMapping(Key.F, "F", KeyCode.F),
			new KeyMapping(Key.G, "G", KeyCode.G),
			new KeyMapping(Key.H, "H", KeyCode.H),
			new KeyMapping(Key.I, "I", KeyCode.I),
			new KeyMapping(Key.J, "J", KeyCode.J),
			new KeyMapping(Key.K, "K", KeyCode.K),
			new KeyMapping(Key.L, "L", KeyCode.L),
			new KeyMapping(Key.M, "M", KeyCode.M),
			new KeyMapping(Key.N, "N", KeyCode.N),
			new KeyMapping(Key.O, "O", KeyCode.O),
			new KeyMapping(Key.P, "P", KeyCode.P),
			new KeyMapping(Key.Q, "Q", KeyCode.Q),
			new KeyMapping(Key.R, "R", KeyCode.R),
			new KeyMapping(Key.S, "S", KeyCode.S),
			new KeyMapping(Key.T, "T", KeyCode.T),
			new KeyMapping(Key.U, "U", KeyCode.U),
			new KeyMapping(Key.V, "V", KeyCode.V),
			new KeyMapping(Key.W, "W", KeyCode.W),
			new KeyMapping(Key.X, "X", KeyCode.X),
			new KeyMapping(Key.Y, "Y", KeyCode.Y),
			new KeyMapping(Key.Z, "Z", KeyCode.Z),
			new KeyMapping(Key.Backquote, "Backquote", KeyCode.BackQuote),
			new KeyMapping(Key.Minus, "Minus", KeyCode.Minus),
			new KeyMapping(Key.Equals, "Equals", KeyCode.Equals),
			new KeyMapping(Key.Backspace, "Backspace", "Delete", KeyCode.Backspace),
			new KeyMapping(Key.Tab, "Tab", KeyCode.Tab),
			new KeyMapping(Key.LeftBracket, "Left Bracket", KeyCode.LeftBracket),
			new KeyMapping(Key.RightBracket, "Right Bracket", KeyCode.RightBracket),
			new KeyMapping(Key.Backslash, "Backslash", KeyCode.Backslash),
			new KeyMapping(Key.Semicolon, "Semicolon", KeyCode.Semicolon),
			new KeyMapping(Key.Quote, "Quote", KeyCode.Quote),
			new KeyMapping(Key.Return, "Return", KeyCode.Return),
			new KeyMapping(Key.Comma, "Comma", KeyCode.Comma),
			new KeyMapping(Key.Period, "Period", KeyCode.Period),
			new KeyMapping(Key.Slash, "Slash", KeyCode.Slash),
			new KeyMapping(Key.Space, "Space", KeyCode.Space),
			new KeyMapping(Key.Insert, "Insert", KeyCode.Insert),
			new KeyMapping(Key.Delete, "Delete", "Forward Delete", KeyCode.Delete),
			new KeyMapping(Key.Home, "Home", KeyCode.Home),
			new KeyMapping(Key.End, "End", KeyCode.End),
			new KeyMapping(Key.PageUp, "PageUp", KeyCode.PageUp),
			new KeyMapping(Key.PageDown, "PageDown", KeyCode.PageDown),
			new KeyMapping(Key.LeftArrow, "Left Arrow", KeyCode.LeftArrow),
			new KeyMapping(Key.RightArrow, "Right Arrow", KeyCode.RightArrow),
			new KeyMapping(Key.UpArrow, "Up Arrow", KeyCode.UpArrow),
			new KeyMapping(Key.DownArrow, "Down Arrow", KeyCode.DownArrow),
			new KeyMapping(Key.Pad0, "Pad 0", KeyCode.Keypad0),
			new KeyMapping(Key.Pad1, "Pad 1", KeyCode.Keypad1),
			new KeyMapping(Key.Pad2, "Pad 2", KeyCode.Keypad2),
			new KeyMapping(Key.Pad3, "Pad 3", KeyCode.Keypad3),
			new KeyMapping(Key.Pad4, "Pad 4", KeyCode.Keypad4),
			new KeyMapping(Key.Pad5, "Pad 5", KeyCode.Keypad5),
			new KeyMapping(Key.Pad6, "Pad 6", KeyCode.Keypad6),
			new KeyMapping(Key.Pad7, "Pad 7", KeyCode.Keypad7),
			new KeyMapping(Key.Pad8, "Pad 8", KeyCode.Keypad8),
			new KeyMapping(Key.Pad9, "Pad 9", KeyCode.Keypad9),
			new KeyMapping(Key.Numlock, "Numlock", KeyCode.Numlock),
			new KeyMapping(Key.PadDivide, "Pad Divide", KeyCode.KeypadDivide),
			new KeyMapping(Key.PadMultiply, "Pad Multiply", KeyCode.KeypadMultiply),
			new KeyMapping(Key.PadMinus, "Pad Minus", KeyCode.KeypadMinus),
			new KeyMapping(Key.PadPlus, "Pad Plus", KeyCode.KeypadPlus),
			new KeyMapping(Key.PadEnter, "Pad Enter", KeyCode.KeypadEnter),
			new KeyMapping(Key.PadPeriod, "Pad Period", KeyCode.KeypadPeriod),
			new KeyMapping(Key.Clear, "Clear", KeyCode.Clear),
			new KeyMapping(Key.PadEquals, "Pad Equals", KeyCode.KeypadEquals),
			new KeyMapping(Key.F13, "F13", KeyCode.F13),
			new KeyMapping(Key.F14, "F14", KeyCode.F14),
			new KeyMapping(Key.F15, "F15", KeyCode.F15),
			new KeyMapping(Key.AltGr, "Alt Graphic", KeyCode.AltGr),
			new KeyMapping(Key.CapsLock, "Caps Lock", KeyCode.CapsLock),
			new KeyMapping(Key.ExclamationMark, "Exclamation", KeyCode.Exclaim),
			new KeyMapping(Key.Tilde, "Tilde", KeyCode.Tilde),
			new KeyMapping(Key.At, "At", KeyCode.At),
			new KeyMapping(Key.Hash, "Hash", KeyCode.Hash),
			new KeyMapping(Key.Dollar, "Dollar", KeyCode.Dollar),
			new KeyMapping(Key.Percent, "Percent", KeyCode.Percent),
			new KeyMapping(Key.Caret, "Caret", KeyCode.Caret),
			new KeyMapping(Key.Ampersand, "Ampersand", KeyCode.Ampersand),
			new KeyMapping(Key.Asterisk, "Asterisk", KeyCode.Asterisk),
			new KeyMapping(Key.LeftParen, "Left Paren", KeyCode.LeftParen),
			new KeyMapping(Key.RightParen, "Right Paren", KeyCode.RightParen),
			new KeyMapping(Key.Underscore, "Underscore", KeyCode.Underscore),
			new KeyMapping(Key.Plus, "Plus", KeyCode.Plus),
			new KeyMapping(Key.LeftBrace, "LeftBrace", KeyCode.LeftCurlyBracket),
			new KeyMapping(Key.RightBrace, "RightBrace", KeyCode.RightCurlyBracket),
			new KeyMapping(Key.Pipe, "Pipe", KeyCode.Pipe),
			new KeyMapping(Key.Colon, "Colon", KeyCode.Colon),
			new KeyMapping(Key.DoubleQuote, "Double Quote", KeyCode.DoubleQuote),
			new KeyMapping(Key.LessThan, "Less Than", KeyCode.Less),
			new KeyMapping(Key.GreaterThan, "Greater Than", KeyCode.Greater),
			new KeyMapping(Key.QuestionMark, "Question Mark", KeyCode.Question)
		};

		public void Setup()
		{
		}

		public void Reset()
		{
		}

		public void Update()
		{
		}

		public bool AnyKeyIsPressed()
		{
			return Input.anyKey;
		}

		public bool GetKeyIsPressed(Key control)
		{
			return KeyMappings[(int)control].IsPressed;
		}

		public string GetNameForKey(Key control)
		{
			return KeyMappings[(int)control].Name;
		}
	}
}

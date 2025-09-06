using System;
using UnityEngine;

namespace InControl
{
	[Serializable]
	public struct InputControlSource
	{
		[SerializeField]
		private InputControlSourceType sourceType;

		[SerializeField]
		private int index;

		public InputControlSourceType SourceType
		{
			get
			{
				return sourceType;
			}
			set
			{
				sourceType = value;
			}
		}

		public int Index
		{
			get
			{
				return index;
			}
			set
			{
				index = value;
			}
		}

		public InputControlSource(InputControlSourceType sourceType, int index)
		{
			this.sourceType = sourceType;
			this.index = index;
		}

		public InputControlSource(KeyCode keyCode)
			: this(InputControlSourceType.KeyCode, (int)keyCode)
		{
		}

		public float GetValue(InputDevice inputDevice)
		{
			switch (SourceType)
			{
			case InputControlSourceType.None:
				return 0f;
			case InputControlSourceType.Button:
				if (!GetState(inputDevice))
				{
					return 0f;
				}
				return 1f;
			case InputControlSourceType.Analog:
				return inputDevice.ReadRawAnalogValue(Index);
			case InputControlSourceType.KeyCode:
				if (!GetState(inputDevice))
				{
					return 0f;
				}
				return 1f;
			default:
				throw new ArgumentOutOfRangeException();
			}
		}

		public bool GetState(InputDevice inputDevice)
		{
			return SourceType switch
			{
				InputControlSourceType.None => false, 
				InputControlSourceType.Button => inputDevice.ReadRawButtonState(Index), 
				InputControlSourceType.Analog => Utility.IsNotZero(GetValue(inputDevice)), 
				InputControlSourceType.KeyCode => Input.GetKey((KeyCode)Index), 
				_ => throw new ArgumentOutOfRangeException(), 
			};
		}

		public string ToCode()
		{
			return "new InputControlSource( InputControlSourceType." + SourceType.ToString() + ", " + Index + " )";
		}
	}
}

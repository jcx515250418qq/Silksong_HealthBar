using System;
using UnityEngine;

namespace InControl
{
	[Serializable]
	public class InputControlMapping
	{
		[SerializeField]
		private string name = "";

		[SerializeField]
		private bool invert;

		[SerializeField]
		private float scale = 1f;

		[SerializeField]
		private bool raw;

		[SerializeField]
		private bool passive;

		[SerializeField]
		private bool ignoreInitialZeroValue;

		[SerializeField]
		private float sensitivity = 1f;

		[SerializeField]
		private float lowerDeadZone;

		[SerializeField]
		private float upperDeadZone = 1f;

		[SerializeField]
		private InputControlSource source;

		[SerializeField]
		private InputControlType target;

		[SerializeField]
		private InputRangeType sourceRange = InputRangeType.MinusOneToOne;

		[SerializeField]
		private InputRangeType targetRange = InputRangeType.MinusOneToOne;

		public string Name
		{
			get
			{
				if (!string.IsNullOrEmpty(name))
				{
					return name;
				}
				return Target.ToString();
			}
			set
			{
				name = value;
			}
		}

		public bool Invert
		{
			get
			{
				return invert;
			}
			set
			{
				invert = value;
			}
		}

		public float Scale
		{
			get
			{
				return scale;
			}
			set
			{
				scale = value;
			}
		}

		public bool Raw
		{
			get
			{
				return raw;
			}
			set
			{
				raw = value;
			}
		}

		public bool Passive
		{
			get
			{
				return passive;
			}
			set
			{
				passive = value;
			}
		}

		public bool IgnoreInitialZeroValue
		{
			get
			{
				return ignoreInitialZeroValue;
			}
			set
			{
				ignoreInitialZeroValue = value;
			}
		}

		public float Sensitivity
		{
			get
			{
				return sensitivity;
			}
			set
			{
				sensitivity = Mathf.Clamp01(value);
			}
		}

		public float LowerDeadZone
		{
			get
			{
				return lowerDeadZone;
			}
			set
			{
				lowerDeadZone = Mathf.Clamp01(value);
			}
		}

		public float UpperDeadZone
		{
			get
			{
				return upperDeadZone;
			}
			set
			{
				upperDeadZone = Mathf.Clamp01(value);
			}
		}

		public InputControlSource Source
		{
			get
			{
				return source;
			}
			set
			{
				source = value;
			}
		}

		public InputControlType Target
		{
			get
			{
				return target;
			}
			set
			{
				target = value;
			}
		}

		public InputRangeType SourceRange
		{
			get
			{
				return sourceRange;
			}
			set
			{
				sourceRange = value;
			}
		}

		public InputRangeType TargetRange
		{
			get
			{
				return targetRange;
			}
			set
			{
				targetRange = value;
			}
		}

		public float ApplyToValue(float value)
		{
			if (Raw)
			{
				value *= Scale;
				value = (InputRange.Excludes(sourceRange, value) ? 0f : value);
			}
			else
			{
				value = Mathf.Clamp(value * Scale, -1f, 1f);
				value = InputRange.Remap(value, sourceRange, targetRange);
			}
			if (Invert)
			{
				value = 0f - value;
			}
			return value;
		}
	}
}

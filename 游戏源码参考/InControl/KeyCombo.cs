using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace InControl
{
	public struct KeyCombo
	{
		public static readonly KeyCombo Empty = default(KeyCombo);

		private int includeSize;

		private ulong includeData;

		private int excludeSize;

		private ulong excludeData;

		private static readonly Dictionary<ulong, string> cachedStrings = new Dictionary<ulong, string>();

		private static readonly StringBuilder cachedStringBuilder = new StringBuilder(256);

		[Obsolete("Use KeyCombo.IncludeCount instead.")]
		public int Count => includeSize;

		public int IncludeCount => includeSize;

		public int ExcludeCount => excludeSize;

		public bool IsPressed
		{
			get
			{
				if (includeSize == 0)
				{
					return false;
				}
				IKeyboardProvider keyboardProvider = InputManager.KeyboardProvider;
				bool flag = true;
				for (int i = 0; i < includeSize; i++)
				{
					Key include = GetInclude(i);
					flag = flag && keyboardProvider.GetKeyIsPressed(include);
				}
				for (int j = 0; j < excludeSize; j++)
				{
					Key exclude = GetExclude(j);
					if (keyboardProvider.GetKeyIsPressed(exclude))
					{
						return false;
					}
				}
				return flag;
			}
		}

		public KeyCombo(params Key[] keys)
		{
			includeData = 0uL;
			includeSize = 0;
			excludeData = 0uL;
			excludeSize = 0;
			for (int i = 0; i < keys.Length; i++)
			{
				AddInclude(keys[i]);
			}
		}

		private void AddIncludeInt(int key)
		{
			if (includeSize != 8)
			{
				includeData |= ((ulong)key & 0xFFuL) << includeSize * 8;
				includeSize++;
			}
		}

		private int GetIncludeInt(int index)
		{
			return (int)((includeData >> index * 8) & 0xFF);
		}

		[Obsolete("Use KeyCombo.AddInclude instead.")]
		public void Add(Key key)
		{
			AddInclude(key);
		}

		[Obsolete("Use KeyCombo.GetInclude instead.")]
		public Key Get(int index)
		{
			return GetInclude(index);
		}

		public void AddInclude(Key key)
		{
			AddIncludeInt((int)key);
		}

		public Key GetInclude(int index)
		{
			if (index < 0 || index >= includeSize)
			{
				throw new IndexOutOfRangeException("Index " + index + " is out of the range 0.." + includeSize);
			}
			return (Key)GetIncludeInt(index);
		}

		private void AddExcludeInt(int key)
		{
			if (excludeSize != 8)
			{
				excludeData |= ((ulong)key & 0xFFuL) << excludeSize * 8;
				excludeSize++;
			}
		}

		private int GetExcludeInt(int index)
		{
			return (int)((excludeData >> index * 8) & 0xFF);
		}

		public void AddExclude(Key key)
		{
			AddExcludeInt((int)key);
		}

		public Key GetExclude(int index)
		{
			if (index < 0 || index >= excludeSize)
			{
				throw new IndexOutOfRangeException("Index " + index + " is out of the range 0.." + excludeSize);
			}
			return (Key)GetExcludeInt(index);
		}

		public static KeyCombo With(params Key[] keys)
		{
			return new KeyCombo(keys);
		}

		public KeyCombo AndNot(params Key[] keys)
		{
			for (int i = 0; i < keys.Length; i++)
			{
				AddExclude(keys[i]);
			}
			return this;
		}

		public void Clear()
		{
			includeData = 0uL;
			includeSize = 0;
			excludeData = 0uL;
			excludeSize = 0;
		}

		public static KeyCombo Detect(bool modifiersAsKeys)
		{
			KeyCombo empty = Empty;
			IKeyboardProvider keyboardProvider = InputManager.KeyboardProvider;
			if (keyboardProvider == null)
			{
				return empty;
			}
			if (modifiersAsKeys)
			{
				for (Key key = Key.LeftShift; key <= Key.RightControl; key++)
				{
					if (keyboardProvider.GetKeyIsPressed(key))
					{
						empty.AddInclude(key);
						if (key == Key.LeftControl && keyboardProvider.GetKeyIsPressed(Key.RightAlt))
						{
							empty.AddInclude(Key.RightAlt);
						}
						return empty;
					}
				}
			}
			else
			{
				for (Key key2 = Key.Shift; key2 <= Key.Control; key2++)
				{
					if (keyboardProvider.GetKeyIsPressed(key2))
					{
						empty.AddInclude(key2);
					}
				}
			}
			for (Key key3 = Key.Escape; key3 <= Key.QuestionMark; key3++)
			{
				if (keyboardProvider.GetKeyIsPressed(key3))
				{
					empty.AddInclude(key3);
					return empty;
				}
			}
			empty.Clear();
			return empty;
		}

		public override string ToString()
		{
			if (!cachedStrings.TryGetValue(includeData, out var value))
			{
				cachedStringBuilder.Clear();
				for (int i = 0; i < includeSize; i++)
				{
					if (i != 0)
					{
						cachedStringBuilder.Append(" ");
					}
					Key include = GetInclude(i);
					cachedStringBuilder.Append(InputManager.KeyboardProvider.GetNameForKey(include));
				}
				value = cachedStringBuilder.ToString();
				cachedStrings[includeData] = value;
			}
			return value;
		}

		public static bool operator ==(KeyCombo a, KeyCombo b)
		{
			if (a.includeData == b.includeData)
			{
				return a.excludeData == b.excludeData;
			}
			return false;
		}

		public static bool operator !=(KeyCombo a, KeyCombo b)
		{
			if (a.includeData == b.includeData)
			{
				return a.excludeData != b.excludeData;
			}
			return true;
		}

		public override bool Equals(object other)
		{
			if (other is KeyCombo keyCombo)
			{
				if (includeData == keyCombo.includeData)
				{
					return excludeData == keyCombo.excludeData;
				}
				return false;
			}
			return false;
		}

		public override int GetHashCode()
		{
			return (17 * 31 + includeData.GetHashCode()) * 31 + excludeData.GetHashCode();
		}

		internal void Load(BinaryReader reader, ushort dataFormatVersion)
		{
			switch (dataFormatVersion)
			{
			case 1:
				includeSize = reader.ReadInt32();
				includeData = reader.ReadUInt64();
				break;
			case 2:
				includeSize = reader.ReadInt32();
				includeData = reader.ReadUInt64();
				excludeSize = reader.ReadInt32();
				excludeData = reader.ReadUInt64();
				break;
			default:
				throw new InControlException("Unknown data format version: " + dataFormatVersion);
			}
		}

		internal void Save(BinaryWriter writer)
		{
			writer.Write(includeSize);
			writer.Write(includeData);
			writer.Write(excludeSize);
			writer.Write(excludeData);
		}
	}
}

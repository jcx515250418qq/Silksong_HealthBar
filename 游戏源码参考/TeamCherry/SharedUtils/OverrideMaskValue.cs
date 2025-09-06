using System;

namespace TeamCherry.SharedUtils
{
	[Serializable]
	public class OverrideMaskValue<T> : OverrideMaskValueBase where T : Enum
	{
		public bool IsEnabled;

		public T Value;
	}
}

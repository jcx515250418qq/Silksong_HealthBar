using System;

namespace GlobalEnums
{
	[Flags]
	public enum AudioPitchEffects
	{
		None = 0,
		Quickening = 1,
		Unused = int.MinValue
	}
}

using System;

namespace GlobalEnums
{
	[Flags]
	public enum DamagePropertyFlags
	{
		None = 0,
		SilkAcid = 1,
		NonLethal = 2,
		Flame = 4,
		Void = 8,
		Acid = 0x10,
		Self = 0x20
	}
}

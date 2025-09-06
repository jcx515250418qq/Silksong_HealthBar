using System;

[Flags]
public enum SpecialTypes
{
	None = 0,
	Acid = 1,
	Lava = 2,
	Piercer = 4,
	Taunt = 8,
	RapidBullet = 0x10,
	RapidBomb = 0x20,
	CocoonBreak = 0x40,
	Heavy = 0x80
}

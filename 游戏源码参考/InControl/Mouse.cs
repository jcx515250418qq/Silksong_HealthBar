using System;

namespace InControl
{
	public enum Mouse
	{
		None = 0,
		LeftButton = 1,
		RightButton = 2,
		MiddleButton = 3,
		NegativeX = 4,
		PositiveX = 5,
		NegativeY = 6,
		PositiveY = 7,
		PositiveScrollWheel = 8,
		NegativeScrollWheel = 9,
		Button4 = 10,
		Button5 = 11,
		Button6 = 12,
		Button7 = 13,
		[Obsolete("Mouse.Button8 is no longer supported and will be removed in a future version.")]
		Button8 = 14,
		[Obsolete("Mouse.Button9 is no longer supported and will be removed in a future version.")]
		Button9 = 15
	}
}

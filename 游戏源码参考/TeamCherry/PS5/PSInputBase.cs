using UnityEngine;

namespace TeamCherry.PS5
{
	public abstract class PSInputBase
	{
		public static PSInputBase GetPSInput()
		{
			return new PS5InControlInput();
		}

		public abstract void Init(GamePad gamePad);

		public abstract Vector2 GetThumbStickLeft();

		public abstract Vector2 GetThumbStickRight();

		public abstract bool GetL3();

		public abstract bool GetR3();

		public abstract bool GetOptions();

		public abstract bool GetCross();

		public abstract bool GetCircle();

		public abstract bool GetSquare();

		public abstract bool GetTriangle();

		public abstract bool GetDpadRight();

		public abstract bool GetDpadLeft();

		public abstract bool GetDpadUp();

		public abstract bool GetDpadDown();

		public abstract bool GetR1();

		public abstract bool GetR2();

		public abstract bool GetL1();

		public abstract bool GetL2();

		public abstract bool TouchPadButton();
	}
}

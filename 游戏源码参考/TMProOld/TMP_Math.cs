namespace TMProOld
{
	public static class TMP_Math
	{
		public const float FLOAT_MAX = 32768f;

		public const float FLOAT_MIN = -32768f;

		public const int INT_MAX = int.MaxValue;

		public const int INT_MIN = -2147483647;

		public static bool Approximately(float a, float b)
		{
			if (b - 0.0001f < a)
			{
				return a < b + 0.0001f;
			}
			return false;
		}
	}
}

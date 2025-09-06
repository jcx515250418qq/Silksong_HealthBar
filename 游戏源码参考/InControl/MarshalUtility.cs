using System;
using System.Runtime.InteropServices;

namespace InControl
{
	public static class MarshalUtility
	{
		private static int[] buffer = new int[32];

		public static void Copy(IntPtr source, uint[] destination, int length)
		{
			Utility.ArrayExpand(ref buffer, length);
			Marshal.Copy(source, buffer, 0, length);
			Buffer.BlockCopy(buffer, 0, destination, 0, 4 * length);
		}
	}
}

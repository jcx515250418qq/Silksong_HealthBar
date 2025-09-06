using System;

namespace TeamCherry.GameCore
{
	public static class SafeExtension
	{
		public static void SafeInvoke<T>(this Action<T> callback, T value)
		{
			if (callback != null)
			{
				CoreLoop.InvokeSafe(delegate
				{
					callback(value);
				});
			}
		}
	}
}

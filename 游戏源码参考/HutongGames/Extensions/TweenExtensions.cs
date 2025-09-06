using UnityEngine;

namespace HutongGames.Extensions
{
	public static class TweenExtensions
	{
		public static Rect Lerp(this Rect rect, Rect from, Rect to, float t)
		{
			rect.Set(Mathf.Lerp(from.x, to.x, t), Mathf.Lerp(from.y, to.y, t), Mathf.Lerp(from.width, to.width, t), Mathf.Lerp(from.height, to.height, t));
			return rect;
		}
	}
}

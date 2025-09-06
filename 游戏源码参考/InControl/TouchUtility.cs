using UnityEngine;

namespace InControl
{
	public static class TouchUtility
	{
		public static Vector2 AnchorToViewPoint(TouchControlAnchor touchControlAnchor)
		{
			return touchControlAnchor switch
			{
				TouchControlAnchor.TopLeft => new Vector2(0f, 1f), 
				TouchControlAnchor.CenterLeft => new Vector2(0f, 0.5f), 
				TouchControlAnchor.BottomLeft => new Vector2(0f, 0f), 
				TouchControlAnchor.TopCenter => new Vector2(0.5f, 1f), 
				TouchControlAnchor.Center => new Vector2(0.5f, 0.5f), 
				TouchControlAnchor.BottomCenter => new Vector2(0.5f, 0f), 
				TouchControlAnchor.TopRight => new Vector2(1f, 1f), 
				TouchControlAnchor.CenterRight => new Vector2(1f, 0.5f), 
				TouchControlAnchor.BottomRight => new Vector2(1f, 0f), 
				_ => Vector2.zero, 
			};
		}

		public static Vector2 RoundVector(Vector2 vector)
		{
			return new Vector2(Mathf.Round(vector.x), Mathf.Round(vector.y));
		}
	}
}

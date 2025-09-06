using UnityEngine;

namespace HutongGames.PlayMaker
{
	public class PlayMakerCanvasRaycastFilterProxy : MonoBehaviour, ICanvasRaycastFilter
	{
		public bool RayCastingEnabled = true;

		public bool IsRaycastLocationValid(Vector2 sp, Camera eventCamera)
		{
			return RayCastingEnabled;
		}
	}
}

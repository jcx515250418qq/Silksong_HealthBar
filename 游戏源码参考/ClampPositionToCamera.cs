using UnityEngine;

public class ClampPositionToCamera : MonoBehaviour
{
	[SerializeField]
	private float screenEdgePadding;

	private void Update()
	{
		Vector2 vector = GameCameras.instance.mainCamera.transform.position;
		Vector2 vector2 = base.transform.position;
		Vector2 vector3 = new Vector2(8.3f * ForceCameraAspect.CurrentViewportAspect, 8.3f);
		Vector2 vector4 = vector - vector3;
		Vector2 vector5 = vector + vector3;
		Vector2 normalized = (vector2 - vector).normalized;
		if (!(vector2.x >= vector4.x) || !(vector2.x <= vector5.x))
		{
			Vector2 vector6 = vector + normalized * vector3.x;
			if (vector6.x < vector4.x)
			{
				vector6.x = vector4.x;
			}
			else if (vector6.x > vector5.x)
			{
				vector6.x = vector5.x;
			}
			vector6 += normalized * screenEdgePadding;
			base.transform.position = vector6;
		}
		if (!(vector2.y >= vector4.y) || !(vector2.y <= vector5.y))
		{
			Vector2 vector7 = vector + normalized * vector3.y;
			if (vector7.y < vector4.y)
			{
				vector7.y = vector4.y;
			}
			else if (vector7.y > vector5.y)
			{
				vector7.y = vector5.y;
			}
			vector7 += normalized * screenEdgePadding;
			base.transform.position = vector7;
		}
	}
}

using UnityEngine;

public class ForceCameraAspectLite : MonoBehaviour
{
	public Camera sceneCamera;

	private bool viewportChanged;

	private int lastX;

	private int lastY;

	private float scaleAdjust;

	private void Start()
	{
		AutoScaleViewport();
	}

	private void Update()
	{
		viewportChanged = false;
		if (lastX != Screen.width)
		{
			viewportChanged = true;
		}
		if (lastY != Screen.height)
		{
			viewportChanged = true;
		}
		if (viewportChanged)
		{
			AutoScaleViewport();
		}
		lastX = Screen.width;
		lastY = Screen.height;
	}

	private void AutoScaleViewport()
	{
		float num = (float)Screen.width / (float)Screen.height / 1.7777778f;
		float num2 = 1f + scaleAdjust;
		Rect rect = sceneCamera.rect;
		if (num < 1f)
		{
			rect.width = 1f * num2;
			rect.height = num * num2;
			float x = (1f - rect.width) / 2f;
			rect.x = x;
			float y = (1f - rect.height) / 2f;
			rect.y = y;
		}
		else
		{
			float num3 = 1f / num;
			rect.width = num3 * num2;
			rect.height = 1f * num2;
			float x2 = (1f - rect.width) / 2f;
			rect.x = x2;
			float y2 = (1f - rect.height) / 2f;
			rect.y = y2;
		}
		sceneCamera.rect = rect;
	}
}

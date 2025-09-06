using UnityEngine;
using UnityEngine.UI;

public class CameraRenderToUI : MonoBehaviour
{
	[SerializeField]
	private Camera camera;

	[SerializeField]
	private RawImage image;

	private RenderTexture renderTexture;

	private void Start()
	{
		Rect rect = image.rectTransform.rect;
		renderTexture = new RenderTexture((int)rect.width, (int)rect.height, 0, RenderTextureFormat.Default);
		renderTexture.name = "CameraRenderToUI" + GetInstanceID();
		camera.targetTexture = renderTexture;
		image.texture = renderTexture;
	}

	private void OnDestroy()
	{
		if (renderTexture != null)
		{
			renderTexture.Release();
			Object.Destroy(renderTexture);
			renderTexture = null;
		}
	}
}

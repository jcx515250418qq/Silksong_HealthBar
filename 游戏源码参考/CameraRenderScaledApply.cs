using UnityEngine;

public class CameraRenderScaledApply : MonoBehaviour
{
	private Camera camera;

	public RenderTexture Texture { get; set; }

	public Camera SourceCamera { get; set; }

	public bool IsActive
	{
		get
		{
			if ((bool)camera)
			{
				return camera.isActiveAndEnabled;
			}
			return false;
		}
	}

	private void Awake()
	{
		camera = GetComponent<Camera>();
	}

	private void OnPreRender()
	{
		if (Application.isPlaying && (bool)SourceCamera && SourceCamera.isActiveAndEnabled && (bool)Texture)
		{
			Graphics.Blit(Texture, camera.activeTexture);
		}
	}
}

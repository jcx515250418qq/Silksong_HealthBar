using UnityEngine;

[ExecuteInEditMode]
public class CameraCaptureRender : MonoBehaviour
{
	[SerializeField]
	private string globalTextureName = "_GameCameraCapture";

	private void OnRenderImage(RenderTexture source, RenderTexture destination)
	{
		Shader.SetGlobalTexture(globalTextureName, source);
		Graphics.Blit(source, destination);
	}
}

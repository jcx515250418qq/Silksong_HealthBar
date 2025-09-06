using UnityEngine;

[ExecuteInEditMode]
public class DarknessCameraEffect : MonoBehaviour
{
	[SerializeField]
	private Camera mainCamera;

	private RenderTexture camTex;

	private Camera camera;

	private bool hasChecked;

	private static readonly int _darknessCutoutProp = Shader.PropertyToID("_DarknessCutout");

	private static readonly int _darknessCameraVpProp = Shader.PropertyToID("_DarknessCameraVP");

	private static readonly int _previewLerpProp = Shader.PropertyToID("_PreviewLerp");

	private bool isDebugView;

	public bool IsDebugView
	{
		get
		{
			return isDebugView;
		}
		set
		{
			isDebugView = value;
			mainCamera.enabled = !isDebugView;
		}
	}

	private void Awake()
	{
		if (!mainCamera)
		{
			Debug.LogError("Can't merge null camera!", this);
			return;
		}
		camera = GetComponent<Camera>();
		if (!Application.isPlaying)
		{
			OnPreRender();
		}
	}

	private void OnDestroy()
	{
		if (camTex != null)
		{
			camera.targetTexture = null;
			Object.DestroyImmediate(camTex);
			camTex = null;
		}
	}

	private void OnPreRender()
	{
		EnsureSetup();
		Shader.SetGlobalMatrix(_darknessCameraVpProp, CalculateVp());
		Shader.SetGlobalFloat(_previewLerpProp, IsDebugView ? 0f : 1f);
	}

	private void OnPostRender()
	{
		Shader.SetGlobalFloat(_previewLerpProp, 0f);
	}

	private void EnsureSetup()
	{
		camera.transparencySortMode = mainCamera.transparencySortMode;
		camera.fieldOfView = mainCamera.fieldOfView;
		if (isDebugView)
		{
			if (camTex != null)
			{
				camera.targetTexture = null;
				Object.DestroyImmediate(camTex);
			}
			camera.rect = mainCamera.rect;
			return;
		}
		camera.rect = new Rect(0f, 0f, 1f, 1f);
		if (camTex == null)
		{
			CreateTargetTexture();
		}
		else if (camTex.width != mainCamera.pixelWidth || camTex.height != mainCamera.pixelHeight)
		{
			camera.targetTexture = null;
			Object.DestroyImmediate(camTex);
			CreateTargetTexture();
		}
	}

	private void CreateTargetTexture()
	{
		camTex = new RenderTexture(mainCamera.pixelWidth, mainCamera.pixelHeight, 0)
		{
			hideFlags = HideFlags.DontSave,
			name = "DarknessCameraEffect" + GetInstanceID()
		};
		camera.targetTexture = camTex;
		Shader.SetGlobalTexture(_darknessCutoutProp, camTex);
	}

	private Matrix4x4 CalculateVp()
	{
		Matrix4x4 worldToCameraMatrix = camera.worldToCameraMatrix;
		return GL.GetGPUProjectionMatrix(camera.projectionMatrix, renderIntoTexture: true) * worldToCameraMatrix;
	}
}

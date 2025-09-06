using UnityEngine;

[RequireComponent(typeof(GameCameras))]
public class LightBlurredBackground : MonoBehaviour
{
	[SerializeField]
	private float distantFarClipPlane;

	[SerializeField]
	private int renderTextureHeight;

	[SerializeField]
	private Material blitMaterial;

	[SerializeField]
	private float clipEpsilon;

	private GameCameras gameCameras;

	private Camera sceneCamera;

	private Camera backgroundCamera;

	private RenderTexture renderTexture;

	private Material blurMaterialInstance;

	private Material blitMaterialInstance;

	private LightBlur lightBlur;

	private int passGroupCount;

	private bool cancelEnable;

	public int RenderTextureHeight
	{
		get
		{
			return renderTextureHeight;
		}
		set
		{
			renderTextureHeight = value;
		}
	}

	public int PassGroupCount
	{
		get
		{
			return passGroupCount;
		}
		set
		{
			passGroupCount = value;
			if (lightBlur != null)
			{
				lightBlur.PassGroupCount = passGroupCount;
			}
		}
	}

	protected void Awake()
	{
		gameCameras = GetComponent<GameCameras>();
		sceneCamera = gameCameras.tk2dCam.GetComponent<Camera>();
		passGroupCount = 2;
		if (gameCameras != GameCameras.instance)
		{
			cancelEnable = true;
		}
	}

	protected void OnEnable()
	{
		if (!cancelEnable)
		{
			distantFarClipPlane = sceneCamera.farClipPlane;
			GameObject gameObject = new GameObject("BlurCamera");
			gameObject.transform.SetParent(sceneCamera.transform);
			backgroundCamera = gameObject.AddComponent<Camera>();
			backgroundCamera.CopyFrom(sceneCamera);
			backgroundCamera.farClipPlane = distantFarClipPlane;
			backgroundCamera.depth -= 5f;
			backgroundCamera.rect = new Rect(0f, 0f, 1f, 1f);
			backgroundCamera.cullingMask &= -33;
			lightBlur = gameObject.AddComponent<LightBlur>();
			lightBlur.PassGroupCount = passGroupCount;
			gameObject.AddComponent<CameraRenderHooks>();
			sceneCamera.GetComponent<CameraShakeManager>().CopyTo(gameObject);
			UpdateCameraClipPlanes();
			blitMaterialInstance = new Material(blitMaterial);
			blitMaterialInstance.EnableKeyword("BLUR_PLANE");
			ForceCameraAspect.ViewportAspectChanged += OnCameraAspectChanged;
			OnCameraAspectChanged(ForceCameraAspect.CurrentViewportAspect);
			ForceCameraAspect.MainCamFovChanged += OnCameraFovChanged;
			OnCameraFovChanged(ForceCameraAspect.CurrentMainCamFov);
			OnBlurPlanesChanged();
			BlurPlane.BlurPlanesChanged += OnBlurPlanesChanged;
		}
	}

	private void OnCameraAspectChanged(float aspect)
	{
		if (aspect <= Mathf.Epsilon)
		{
			return;
		}
		int num = Mathf.RoundToInt((float)renderTextureHeight * aspect);
		if (num > 0)
		{
			if (renderTexture != null)
			{
				Object.Destroy(renderTexture);
			}
			renderTexture = new RenderTexture(num, renderTextureHeight, 32, RenderTextureFormat.Default);
			renderTexture.name = "LightBlurredBackground" + GetInstanceID();
			backgroundCamera.targetTexture = renderTexture;
			blitMaterialInstance.mainTexture = renderTexture;
		}
	}

	private void OnCameraFovChanged(float newFov)
	{
		backgroundCamera.fieldOfView = newFov;
	}

	private void OnDestroy()
	{
		ForceCameraAspect.ViewportAspectChanged -= OnCameraAspectChanged;
		ForceCameraAspect.MainCamFovChanged -= OnCameraFovChanged;
		BlurPlane.BlurPlanesChanged -= OnBlurPlanesChanged;
		if (renderTexture != null)
		{
			renderTexture.Release();
			Object.Destroy(renderTexture);
			renderTexture = null;
		}
		if (blitMaterialInstance != null)
		{
			Object.Destroy(blitMaterialInstance);
			blitMaterialInstance = null;
		}
	}

	protected void OnDisable()
	{
		if (!cancelEnable)
		{
			ForceCameraAspect.ViewportAspectChanged -= OnCameraAspectChanged;
			ForceCameraAspect.MainCamFovChanged -= OnCameraFovChanged;
			BlurPlane.BlurPlanesChanged -= OnBlurPlanesChanged;
			for (int i = 0; i < BlurPlane.BlurPlaneCount; i++)
			{
				BlurPlane blurPlane = BlurPlane.GetBlurPlane(i);
				blurPlane.SetPlaneMaterial(null);
				blurPlane.SetPlaneVisibility(isVisible: true);
			}
			Object.Destroy(blitMaterialInstance);
			blitMaterialInstance = null;
			lightBlur = null;
			backgroundCamera.targetTexture = null;
			Object.Destroy(renderTexture);
			renderTexture = null;
			sceneCamera.farClipPlane = distantFarClipPlane;
			Object.Destroy(backgroundCamera.gameObject);
			backgroundCamera = null;
		}
	}

	private void OnBlurPlanesChanged()
	{
		for (int i = 0; i < BlurPlane.BlurPlaneCount; i++)
		{
			BlurPlane blurPlane = BlurPlane.GetBlurPlane(i);
			blurPlane.SetPlaneVisibility(isVisible: true);
			blurPlane.SetPlaneMaterial(blitMaterialInstance);
		}
		UpdateCameraClipPlanes();
	}

	protected void LateUpdate()
	{
		if (!cancelEnable)
		{
			UpdateCameraClipPlanes();
		}
	}

	private void UpdateCameraClipPlanes()
	{
		BlurPlane closestBlurPlane = BlurPlane.ClosestBlurPlane;
		if (closestBlurPlane != null)
		{
			sceneCamera.farClipPlane = closestBlurPlane.PlaneZ - sceneCamera.transform.GetPositionZ() + clipEpsilon;
			backgroundCamera.nearClipPlane = closestBlurPlane.PlaneZ - backgroundCamera.transform.GetPositionZ() + clipEpsilon;
		}
		else
		{
			sceneCamera.farClipPlane = distantFarClipPlane;
			backgroundCamera.nearClipPlane = distantFarClipPlane;
		}
	}
}

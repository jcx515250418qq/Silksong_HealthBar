using System.Collections.Generic;
using UnityEngine;

public class CameraRenderToMesh : MonoBehaviour
{
	public enum ActiveSources
	{
		None = 0,
		GameMap = 1
	}

	[SerializeField]
	private Camera sourceCamera;

	[Space]
	[SerializeField]
	private Camera targetCamera;

	[SerializeField]
	private RenderTextureFormat textureFormat = RenderTextureFormat.Default;

	[SerializeField]
	[ModifiableProperty]
	[Conditional("aspectMatchMainCam", false, false, false)]
	private float aspect;

	[SerializeField]
	private bool aspectMatchMainCam;

	[Space]
	[SerializeField]
	private MeshRenderer meshRenderer;

	[Space]
	[SerializeField]
	private ActiveSources activeSource;

	private int previousPixelWidth;

	private int previousPixelHeight;

	private RenderTexture texture;

	private float heightMult = 1f;

	private static readonly List<CameraRenderToMesh> _hasActiveSource = new List<CameraRenderToMesh>();

	private static readonly int _mainTexProp = Shader.PropertyToID("_MainTex");

	private void Reset()
	{
		targetCamera = GetComponent<Camera>();
	}

	private void Awake()
	{
		targetCamera.orthographic = sourceCamera.orthographic;
		targetCamera.orthographicSize = sourceCamera.orthographicSize;
		UpdateTexture();
	}

	private void OnEnable()
	{
		if (aspectMatchMainCam)
		{
			ForceCameraAspect.ViewportAspectChanged += OnAspectChanged;
			OnAspectChanged(ForceCameraAspect.CurrentViewportAspect);
		}
		if (activeSource != 0)
		{
			targetCamera.enabled = false;
			meshRenderer.gameObject.SetActive(value: false);
			_hasActiveSource.Add(this);
		}
	}

	private void OnDisable()
	{
		_hasActiveSource.Remove(this);
		if (aspectMatchMainCam)
		{
			ForceCameraAspect.ViewportAspectChanged -= OnAspectChanged;
		}
	}

	private void OnDestroy()
	{
		if (texture != null)
		{
			texture.Release();
			Object.Destroy(texture);
			texture = null;
		}
	}

	private void LateUpdate()
	{
		UpdateTexture();
	}

	private void OnAspectChanged(float newAspect)
	{
		aspect = newAspect;
		if (aspect < 1.7777778f)
		{
			aspect = 1.7777778f;
		}
		heightMult = ForceCameraAspect.CurrentMainCamHeightMult;
		UpdateTexture();
	}

	private void UpdateTexture()
	{
		float num = ((aspect > Mathf.Epsilon) ? aspect : targetCamera.aspect);
		int num2 = Mathf.RoundToInt((float)sourceCamera.pixelHeight * heightMult);
		int num3 = Mathf.RoundToInt((float)num2 * aspect);
		if (num3 > 0 && num2 > 0 && (num3 != previousPixelWidth || num2 != previousPixelHeight))
		{
			previousPixelWidth = num3;
			previousPixelHeight = num2;
			if (texture != null)
			{
				texture.Release();
				Object.Destroy(texture);
			}
			texture = new RenderTexture(num3, num2, 32, textureFormat);
			texture.name = "CameraRenderToMesh" + GetInstanceID();
			targetCamera.targetTexture = texture;
			Transform obj = meshRenderer.transform;
			float num4 = targetCamera.orthographicSize * 2f;
			obj.localScale = new Vector3(num4 * num, num4, 1f);
			meshRenderer.material.SetTexture(_mainTexProp, texture);
		}
	}

	public static void SetActive(ActiveSources activeSource, bool value)
	{
		foreach (CameraRenderToMesh item in _hasActiveSource)
		{
			if (item.activeSource == activeSource)
			{
				item.targetCamera.enabled = value;
				item.meshRenderer.gameObject.SetActive(value);
			}
		}
	}
}

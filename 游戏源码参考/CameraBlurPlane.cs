using UnityEngine;

public class CameraBlurPlane : MonoBehaviour
{
	private const int PASSES = 5;

	[SerializeField]
	private Material material;

	private static CameraBlurPlane _instance;

	private Material blurMaterial;

	private Camera camera;

	private float lastSetSpacing;

	private float lastSetVibrancy;

	private float lastSetMaskLerp;

	private float lastSetMaskScale;

	private Vector2 maskTiling;

	private Vector2 baseMaskOffset;

	private const float MIN_SNAP = 0.0001f;

	private static readonly int _maskStProp = Shader.PropertyToID("_Mask_ST");

	private static readonly int _sizeProp = Shader.PropertyToID("_Size");

	private static readonly int _vibrancyProp = Shader.PropertyToID("_Vibrancy");

	private static readonly int _maskLerpProp = Shader.PropertyToID("_MaskLerp");

	public static float Spacing
	{
		get
		{
			if (!_instance)
			{
				return 0f;
			}
			return _instance.lastSetSpacing;
		}
		set
		{
			if ((bool)_instance)
			{
				_instance.SetSpacingInternal(value);
			}
		}
	}

	public static float Vibrancy
	{
		get
		{
			if (!_instance)
			{
				return 0f;
			}
			return _instance.lastSetVibrancy;
		}
		set
		{
			if ((bool)_instance)
			{
				_instance.SetVibrancyInternal(value);
			}
		}
	}

	public static float MaskLerp
	{
		get
		{
			if (!_instance)
			{
				return 0f;
			}
			return _instance.lastSetMaskLerp;
		}
		set
		{
			if ((bool)_instance)
			{
				_instance.SetMaskLerpInternal(value);
			}
		}
	}

	public static float MaskScale
	{
		get
		{
			if (!_instance)
			{
				return 0f;
			}
			return _instance.lastSetMaskScale;
		}
		set
		{
			if ((bool)_instance)
			{
				_instance.SetMaskScale(value, ForceCameraAspect.CurrentViewportAspect);
			}
		}
	}

	private void Awake()
	{
		if (!_instance)
		{
			_instance = this;
		}
		camera = GetComponent<Camera>();
		blurMaterial = new Material(material);
		SetMaskScale(1f, ForceCameraAspect.CurrentViewportAspect);
		base.enabled = false;
	}

	private void OnDestroy()
	{
		if (_instance == this)
		{
			_instance = null;
		}
		if (blurMaterial != null)
		{
			Object.Destroy(blurMaterial);
			blurMaterial = null;
		}
	}

	private void OnEnable()
	{
		OnCameraAspectChanged(ForceCameraAspect.CurrentViewportAspect);
		ForceCameraAspect.ViewportAspectChanged += OnCameraAspectChanged;
	}

	private void OnDisable()
	{
		ForceCameraAspect.ViewportAspectChanged -= OnCameraAspectChanged;
	}

	private void Update()
	{
		if (lastSetMaskLerp >= 0.0001f)
		{
			SetMaskOffset();
		}
	}

	private void SetSpacingInternal(float value)
	{
		value *= 0.1f;
		SetMaterialFloatValue(_sizeProp, value);
		lastSetSpacing = value;
		UpdateActiveState();
	}

	private void SetVibrancyInternal(float value)
	{
		SetMaterialFloatValue(_vibrancyProp, value);
		lastSetVibrancy = value;
		UpdateActiveState();
	}

	private void SetMaskLerpInternal(float value)
	{
		SetMaterialFloatValue(_maskLerpProp, value);
		if (lastSetMaskLerp > 0.0001f)
		{
			if (value <= 0.0001f)
			{
				blurMaterial.DisableKeyword("USE_MASK");
			}
		}
		else if (value > 0.0001f)
		{
			blurMaterial.EnableKeyword("USE_MASK");
		}
		lastSetMaskLerp = value;
		UpdateActiveState();
	}

	private void SetMaterialFloatValue(int propId, float value)
	{
		if ((bool)blurMaterial)
		{
			blurMaterial.SetFloat(propId, value);
		}
	}

	private void UpdateActiveState()
	{
		base.enabled = lastSetVibrancy > 0.0001f || lastSetSpacing > 0.0001f;
	}

	private void SetMaskScale(float scale, float cameraAspect)
	{
		maskTiling = new Vector2(cameraAspect, 1f) / scale;
		baseMaskOffset = new Vector2((1f - maskTiling.x) / 2f, (1f - maskTiling.y) / 2f);
		SetMaskOffset();
		lastSetMaskScale = scale;
	}

	private void SetMaskOffset()
	{
		HeroController silentInstance = HeroController.SilentInstance;
		if ((bool)silentInstance)
		{
			Vector3 position = silentInstance.transform.position;
			Vector2 vector = (Vector2)camera.WorldToViewportPoint(position) - new Vector2(0.5f, 0.5f);
			Vector2 vector2 = baseMaskOffset - vector / 2f;
			blurMaterial.SetVector(_maskStProp, new Vector4(maskTiling.x, maskTiling.y, vector2.x, vector2.y));
		}
	}

	private void OnCameraAspectChanged(float aspect)
	{
		SetMaskScale(lastSetMaskScale, aspect);
	}

	public void OnRenderImage(RenderTexture source, RenderTexture destination)
	{
		RenderTexture temporary = RenderTexture.GetTemporary(source.width, source.height);
		Graphics.Blit(source, temporary, blurMaterial, 0);
		bool flag = false;
		for (int i = 1; i < 4; i++)
		{
			if (flag)
			{
				Graphics.Blit(destination, temporary, blurMaterial, i);
			}
			else
			{
				Graphics.Blit(temporary, destination, blurMaterial, i);
			}
			flag = !flag;
		}
		Graphics.Blit(flag ? destination : temporary, flag ? temporary : destination, blurMaterial, 4);
		if (flag)
		{
			Graphics.Blit(temporary, destination);
		}
		RenderTexture.ReleaseTemporary(temporary);
	}
}

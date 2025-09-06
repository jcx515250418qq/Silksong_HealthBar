using UnityEngine;
using UnityStandardAssets.ImageEffects;

public class LightBlur : PostEffectsBase
{
	private const string BlurShaderName = "Hollow Knight/Light Blur";

	private const int BlurMaterialPassCount = 2;

	private int passGroupCount;

	private const int BlurPassCountMax = 4;

	private const string BlurInfoPropertyName = "_BlurInfo";

	private int blurInfoId;

	private Shader blurShader;

	private Material blurMaterial;

	private bool effectIsSupported;

	private RenderTextureCache rt1Cache = new RenderTextureCache();

	private RenderTextureCache rt2Cache = new RenderTextureCache();

	public int PassGroupCount
	{
		get
		{
			return passGroupCount;
		}
		set
		{
			passGroupCount = value;
		}
	}

	public int BlurPassCount => passGroupCount * 2;

	protected void Awake()
	{
		passGroupCount = 2;
	}

	protected override void OnEnable()
	{
		base.OnEnable();
		effectIsSupported = CheckResources();
	}

	private void OnDisable()
	{
		rt1Cache.CleanUpRenderTexture();
		rt2Cache.CleanUpRenderTexture();
	}

	protected void OnDestroy()
	{
		if (blurMaterial != null)
		{
			Object.Destroy(blurMaterial);
		}
		blurMaterial = null;
	}

	public override bool CheckResources()
	{
		bool flag = true;
		if (blurInfoId == 0)
		{
			blurInfoId = Shader.PropertyToID("_BlurInfo");
		}
		if (blurShader == null)
		{
			blurShader = Shader.Find("Hollow Knight/Light Blur");
			if (blurShader == null)
			{
				Debug.LogErrorFormat(this, "Failed to find shader {0}", "Hollow Knight/Light Blur");
				flag = false;
			}
		}
		if (blurMaterial == null)
		{
			blurMaterial = CheckShaderAndCreateMaterial(blurShader, blurMaterial);
		}
		return CheckSupport() && flag;
	}

	private void OnRenderImage(RenderTexture source, RenderTexture destination)
	{
		if (!effectIsSupported)
		{
			base.enabled = false;
			Graphics.Blit(source, destination);
			return;
		}
		RenderTexture renderTexture = source;
		Vector4 value = new Vector4(1f / (float)source.width, 1f / (float)source.height, 0f, 0f);
		RenderTexture renderTexture2 = rt1Cache.GetRenderTexture(source.width, source.height, 32, source.format);
		RenderTexture renderTexture3 = rt2Cache.GetRenderTexture(source.width, source.height, 32, source.format);
		bool flag = true;
		for (int i = 0; i < BlurPassCount; i++)
		{
			RenderTexture renderTexture4;
			if (i == BlurPassCount - 1)
			{
				renderTexture4 = destination;
			}
			else if (flag)
			{
				renderTexture4 = renderTexture2;
				flag = false;
			}
			else
			{
				renderTexture4 = renderTexture3;
				flag = true;
			}
			blurMaterial.SetVector(blurInfoId, value);
			renderTexture.filterMode = FilterMode.Bilinear;
			Graphics.Blit(renderTexture, renderTexture4, blurMaterial, i % 2);
			renderTexture = renderTexture4;
		}
		if (!RenderTextureCache.ReuseRenderTexture)
		{
			RenderTexture.ReleaseTemporary(renderTexture2);
			RenderTexture.ReleaseTemporary(renderTexture3);
		}
	}
}

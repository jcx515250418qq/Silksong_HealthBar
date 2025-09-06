using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityStandardAssets.ImageEffects;

[ExecuteInEditMode]
[RequireComponent(typeof(Camera))]
[AddComponentMenu("Image Effects/Bloom and Glow/Bloom (Optimized)")]
public class BloomOptimized : PostEffectsBase
{
	private enum Resolution
	{
		Low = 0,
		High = 1
	}

	public enum BlurTypes
	{
		Standard = 0,
		Sgx = 1
	}

	private class BloomResources
	{
		public RenderTexture BufferA { get; private set; }

		public RenderTexture BufferB { get; private set; }

		public bool IsCreated
		{
			get
			{
				if (BufferA != null)
				{
					return BufferB != null;
				}
				return false;
			}
		}

		private BloomResources()
		{
		}

		public static BloomResources Create(int width, int height, RenderTextureFormat format, FilterMode filterMode)
		{
			BloomResources bloomResources = new BloomResources();
			bloomResources.CreateBuffers(width, height, format, filterMode);
			return bloomResources;
		}

		public void Release()
		{
			if (BufferA != null)
			{
				BufferA.Release();
				BufferA = null;
			}
			if (BufferB != null)
			{
				BufferB.Release();
				BufferB = null;
			}
		}

		public void EnsureFormat(int width, int height, RenderTextureFormat format, FilterMode filterMode)
		{
			if (BufferA == null)
			{
				throw new InvalidOperationException("Can't use textures in non-created bloom resources");
			}
			if (BufferA.width != width || BufferA.height != height || BufferA.format != format || BufferA.filterMode != filterMode)
			{
				Release();
				CreateBuffers(width, height, format, filterMode);
			}
		}

		private void CreateBuffers(int width, int height, RenderTextureFormat format, FilterMode filterMode)
		{
			BufferA = new RenderTexture(width, height, 0, format);
			BufferA.filterMode = filterMode;
			BufferB = new RenderTexture(width, height, 0, format);
			BufferB.filterMode = filterMode;
		}
	}

	[SerializeField]
	[Range(0f, 1.5f)]
	private float threshold = 0.25f;

	private float previousThreshold;

	[SerializeField]
	[Range(0f, 2.5f)]
	private float intensity = 0.75f;

	private float previousIntensity;

	[SerializeField]
	[Range(0.25f, 5.5f)]
	private float blurSize = 1f;

	private float previousBlurSize;

	[SerializeField]
	[Range(0.25f, 4f)]
	private float blurShape = 0.5f;

	[SerializeField]
	[Range(1f, 4f)]
	private int blurIterations = 1;

	[SerializeField]
	private BlurTypes blurType;

	[SerializeField]
	private Shader fastBloomShader;

	[Space]
	[SerializeField]
	private AnimationCurve fadeCurve = AnimationCurve.EaseInOut(0f, 0f, 1f, 1f);

	private float currentIntensity;

	private float currentThreshold;

	private float currentBlurSize;

	private float fadeDuration;

	private Material fastBloomMaterial;

	private Resolution resolution;

	private readonly List<SceneAppearanceRegion> insideRegions = new List<SceneAppearanceRegion>();

	private Coroutine fadeRoutine;

	private static readonly int _parameterPropId = Shader.PropertyToID("_Parameter");

	private static readonly int _bloomPropId = Shader.PropertyToID("_Bloom");

	private static readonly int _targetTextelSizePropId = Shader.PropertyToID("_TargetTexelSize");

	private bool effectIsSupported;

	private BloomResources resources;

	public float Threshold
	{
		get
		{
			return threshold;
		}
		set
		{
			threshold = value;
			previousThreshold = value;
			currentThreshold = value;
		}
	}

	public float Intensity
	{
		get
		{
			return intensity;
		}
		set
		{
			intensity = value;
			previousIntensity = value;
			currentIntensity = value;
		}
	}

	public float BlurSize
	{
		get
		{
			return blurSize;
		}
		set
		{
			blurSize = value;
			previousBlurSize = value;
			currentBlurSize = value;
		}
	}

	public float BlurShape
	{
		get
		{
			return blurShape;
		}
		set
		{
			blurShape = value;
		}
	}

	public int BlurIterations
	{
		get
		{
			return blurIterations;
		}
		set
		{
			blurIterations = value;
		}
	}

	public BlurTypes BlurType
	{
		get
		{
			return blurType;
		}
		set
		{
			blurType = value;
		}
	}

	public float InitialIntensity { get; private set; }

	public float InitialThreshold { get; private set; }

	public float InitialBlurSize { get; private set; }

	public float InitialBlurShape { get; private set; }

	public int InitialIterations { get; private set; }

	public BlurTypes InitialBlurType { get; private set; }

	private void OnValidate()
	{
		if (Math.Abs(previousThreshold - threshold) > 0.0001f)
		{
			previousThreshold = threshold;
			currentThreshold = threshold;
		}
		if (Math.Abs(previousIntensity - intensity) > 0.0001f)
		{
			previousIntensity = intensity;
			currentIntensity = intensity;
		}
		if (Math.Abs(previousBlurSize - blurSize) > 0.0001f)
		{
			previousBlurSize = blurSize;
			currentBlurSize = blurSize;
		}
	}

	public override bool CheckResources()
	{
		CheckSupport(needDepth: false);
		fastBloomMaterial = CheckShaderAndCreateMaterial(fastBloomShader, fastBloomMaterial);
		if (!isSupported)
		{
			ReportAutoDisable();
		}
		return isSupported;
	}

	private void Awake()
	{
		if (blurShape < 0.2f)
		{
			blurShape = 0.5f;
		}
		InitialThreshold = threshold;
		InitialIntensity = intensity;
		InitialBlurSize = blurSize;
		InitialBlurShape = blurShape;
		InitialIterations = blurIterations;
		InitialBlurType = blurType;
	}

	protected override void OnEnable()
	{
		currentIntensity = intensity;
		currentThreshold = threshold;
		currentBlurSize = blurSize;
		base.OnEnable();
		effectIsSupported = CheckResources();
		resources = BloomResources.Create(32, 32, RenderTextureFormat.ARGB32, FilterMode.Bilinear);
	}

	private void OnDisable()
	{
		if ((bool)fastBloomMaterial)
		{
			UnityEngine.Object.DestroyImmediate(fastBloomMaterial);
		}
		resources.Release();
		resources = null;
	}

	private void OnRenderImage(RenderTexture source, RenderTexture destination)
	{
		if (!effectIsSupported)
		{
			Graphics.Blit(source, destination);
			return;
		}
		int num = ((resolution == Resolution.Low) ? 4 : 2);
		float num2 = ((resolution == Resolution.Low) ? 0.5f : 1f);
		fastBloomMaterial.SetVector(_parameterPropId, new Vector4(currentBlurSize * num2, 0f, currentThreshold, currentIntensity));
		source.filterMode = FilterMode.Bilinear;
		int width = source.width / num;
		int height = source.height / num;
		resources.EnsureFormat(width, height, source.format, FilterMode.Bilinear);
		fastBloomMaterial.SetVector(_targetTextelSizePropId, new Vector4(1f / (float)resources.BufferA.width, 1f / (float)resources.BufferA.height, resources.BufferA.width, resources.BufferA.height));
		Graphics.Blit(source, resources.BufferA, fastBloomMaterial, 1);
		int num3 = ((blurType != 0) ? 2 : 0);
		for (int i = 0; i < blurIterations; i++)
		{
			float num4 = currentBlurSize * num2 + (float)i * 1f;
			fastBloomMaterial.SetVector(_parameterPropId, new Vector4(num4 * blurShape, num4, currentThreshold, currentIntensity));
			Graphics.Blit(resources.BufferA, resources.BufferB, fastBloomMaterial, 2 + num3);
			Graphics.Blit(resources.BufferB, resources.BufferA, fastBloomMaterial, 3 + num3);
		}
		fastBloomMaterial.SetTexture(_bloomPropId, resources.BufferA);
		Graphics.Blit(source, destination, fastBloomMaterial, 0);
		fastBloomMaterial.SetTexture(_bloomPropId, null);
	}

	public void AddInside(SceneAppearanceRegion region, bool forceImmediate)
	{
		insideRegions.AddIfNotPresent(region);
		UpdateValues(forceImmediate);
	}

	public void RemoveInside(SceneAppearanceRegion region, bool forceImmediate)
	{
		insideRegions.Remove(region);
		UpdateValues(forceImmediate);
	}

	private void UpdateValues(bool forceImmediate)
	{
		if (fadeRoutine != null)
		{
			StopCoroutine(fadeRoutine);
		}
		float bloomIntensity;
		float bloomThreshold;
		float bloomBlurSize;
		if (insideRegions.Count == 0)
		{
			bloomIntensity = intensity;
			bloomThreshold = threshold;
			bloomBlurSize = blurSize;
		}
		else
		{
			SceneAppearanceRegion sceneAppearanceRegion = insideRegions.Last();
			bloomIntensity = sceneAppearanceRegion.BloomIntensity;
			bloomThreshold = sceneAppearanceRegion.BloomThreshold;
			bloomBlurSize = sceneAppearanceRegion.BloomBlurSize;
			fadeDuration = sceneAppearanceRegion.FadeDuration;
		}
		if (fadeDuration > 0f && !forceImmediate)
		{
			fadeRoutine = StartCoroutine(FadeValues(bloomIntensity, bloomThreshold, bloomBlurSize));
			return;
		}
		currentIntensity = bloomIntensity;
		currentThreshold = bloomThreshold;
		currentBlurSize = bloomBlurSize;
	}

	private IEnumerator FadeValues(float targetIntensity, float targetThreshold, float targetBlurSize)
	{
		float startIntensity = currentIntensity;
		float startThreshold = currentThreshold;
		float startBlurSize = currentBlurSize;
		for (float elapsed = 0f; elapsed < fadeDuration; elapsed += Time.deltaTime)
		{
			float t = fadeCurve.Evaluate(elapsed / fadeDuration);
			currentIntensity = Mathf.Lerp(startIntensity, targetIntensity, t);
			currentThreshold = Mathf.Lerp(startThreshold, targetThreshold, t);
			currentBlurSize = Mathf.Lerp(startBlurSize, targetBlurSize, t);
			yield return null;
		}
		currentIntensity = targetIntensity;
		currentThreshold = targetThreshold;
		currentBlurSize = targetBlurSize;
		fadeRoutine = null;
	}
}

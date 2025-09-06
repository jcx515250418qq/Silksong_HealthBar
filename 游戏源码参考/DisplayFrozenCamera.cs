using System;
using TeamCherry.SharedUtils;
using UnityEngine;
using UnityStandardAssets.ImageEffects;

public class DisplayFrozenCamera : MonoBehaviour
{
	private const int BLUR_MAT_PASSES = 5;

	[SerializeField]
	private Camera displayCamera;

	[SerializeField]
	private FastNoise fixNoise;

	[Space]
	[SerializeField]
	private Material blurMaterial;

	[SerializeField]
	private float blurSpacingMax;

	[SerializeField]
	private AnimationCurve blurSpacingCurve = AnimationCurve.Linear(0f, 0f, 1f, 1f);

	private RenderTexture baseTexture;

	private RenderTexture blurredTexture;

	private Camera frozenCamera;

	private MeshRenderer renderer;

	private Material activeBlurMat;

	private float lastBlurT;

	private int unfreezeQueueFrames;

	private static readonly int _mainTexId = Shader.PropertyToID("_MainTex");

	private static readonly int _sizeProp = Shader.PropertyToID("_Size");

	public static bool IsRendering { get; private set; }

	public float BlurT
	{
		get
		{
			return lastBlurT;
		}
		set
		{
			if (Application.isPlaying && !(Math.Abs(value - lastBlurT) <= Mathf.Epsilon))
			{
				lastBlurT = value;
				UpdateBlur();
			}
		}
	}

	private void Awake()
	{
		renderer = GetComponent<MeshRenderer>();
		activeBlurMat = new Material(blurMaterial);
		if ((bool)activeBlurMat)
		{
			UpdateBlur();
		}
	}

	private void OnDestroy()
	{
		if ((bool)activeBlurMat)
		{
			UnityEngine.Object.Destroy(activeBlurMat);
			activeBlurMat = null;
		}
	}

	private void Update()
	{
		if (unfreezeQueueFrames < 0)
		{
			return;
		}
		unfreezeQueueFrames--;
		if (unfreezeQueueFrames <= 0)
		{
			renderer.enabled = false;
			if (baseTexture != null)
			{
				UnityEngine.Object.Destroy(baseTexture);
				baseTexture = null;
			}
			if (blurredTexture != null)
			{
				UnityEngine.Object.Destroy(blurredTexture);
				blurredTexture = null;
			}
		}
	}

	public void Freeze()
	{
		if (renderer == null)
		{
			return;
		}
		Camera main = Camera.main;
		int width;
		int height;
		if (!(main == null))
		{
			float num = displayCamera.pixelWidth;
			float num2 = displayCamera.pixelHeight;
			float num3 = displayCamera.orthographicSize * 2f;
			float x = num3 * (num / num2);
			Transform obj = base.transform;
			Vector3 localScale = obj.localScale;
			localScale.x = x;
			localScale.y = num3;
			obj.localScale = localScale;
			width = main.pixelWidth;
			height = main.pixelHeight;
			ScreenRes resolution = CameraRenderScaled.Resolution;
			if (resolution.Width != 0)
			{
				width = resolution.Width;
			}
			if (resolution.Height != 0)
			{
				height = resolution.Height;
			}
			FixTextureSize(ref baseTexture);
			if ((bool)blurMaterial)
			{
				FixTextureSize(ref blurredTexture);
			}
			RenderTexture targetTexture = main.targetTexture;
			main.enabled = true;
			main.targetTexture = baseTexture;
			IsRendering = true;
			main.Render();
			IsRendering = false;
			main.targetTexture = targetTexture;
			main.enabled = false;
			frozenCamera = main;
			renderer.enabled = true;
			renderer.material.SetTexture(_mainTexId, baseTexture);
		}
		void FixTextureSize(ref RenderTexture texture)
		{
			bool flag = false;
			if (texture != null && (texture.width != width || texture.height != height))
			{
				UnityEngine.Object.Destroy(texture);
				flag = true;
			}
			if (flag || texture == null)
			{
				texture = new RenderTexture(width, height, 24)
				{
					name = "DisplayFrozenCamera" + GetInstanceID()
				};
			}
		}
	}

	public void Unfreeze()
	{
		if (!(frozenCamera == null))
		{
			if ((bool)fixNoise)
			{
				fixNoise.ForceRender = true;
			}
			frozenCamera.enabled = true;
			frozenCamera = null;
			unfreezeQueueFrames = 2;
		}
	}

	private void UpdateBlur()
	{
		if (!renderer.enabled)
		{
			return;
		}
		float num = blurSpacingCurve.Evaluate(lastBlurT);
		if (num <= Mathf.Epsilon)
		{
			renderer.material.SetTexture(_mainTexId, baseTexture);
			return;
		}
		activeBlurMat.SetFloat(_sizeProp, blurSpacingMax * num);
		RenderTexture temporary = RenderTexture.GetTemporary(blurredTexture.width, blurredTexture.height);
		RenderTexture temporary2 = RenderTexture.GetTemporary(blurredTexture.width, blurredTexture.height);
		Graphics.Blit(baseTexture, temporary, activeBlurMat, 0);
		bool flag = false;
		for (int i = 1; i < 4; i++)
		{
			if (flag)
			{
				Graphics.Blit(temporary2, temporary, activeBlurMat, i);
			}
			else
			{
				Graphics.Blit(temporary, temporary2, activeBlurMat, i);
			}
			flag = !flag;
		}
		Graphics.Blit(flag ? temporary2 : temporary, blurredTexture, activeBlurMat, 4);
		RenderTexture.ReleaseTemporary(temporary);
		RenderTexture.ReleaseTemporary(temporary2);
		renderer.material.SetTexture(_mainTexId, blurredTexture);
	}
}

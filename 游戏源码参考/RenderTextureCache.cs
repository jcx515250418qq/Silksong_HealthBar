using UnityEngine;

public sealed class RenderTextureCache
{
	private RenderTexture renderTexture;

	private bool createdRenderTexture;

	private int cachedW;

	private int cachedH;

	private int cachedDepth;

	private RenderTextureFormat cachedFormat;

	public static bool ReuseRenderTexture { get; set; } = true;

	~RenderTextureCache()
	{
		CleanUpRenderTexture();
	}

	public RenderTexture GetRenderTexture(int width, int height, int depth, RenderTextureFormat format)
	{
		if (!ReuseRenderTexture)
		{
			return RenderTexture.GetTemporary(width, height, depth, format);
		}
		if (createdRenderTexture && width == cachedW && height == cachedH && format == cachedFormat)
		{
			return renderTexture;
		}
		cachedW = width;
		cachedH = height;
		cachedFormat = format;
		if (createdRenderTexture)
		{
			renderTexture.Release();
			Object.Destroy(renderTexture);
			renderTexture = null;
		}
		createdRenderTexture = true;
		renderTexture = new RenderTexture(width, height, depth, format)
		{
			name = string.Format("{0}_{1}x{2}_frame{3}", "BloomOptimized", width, height, Time.frameCount)
		};
		return renderTexture;
	}

	public void CleanUpRenderTexture()
	{
		if (createdRenderTexture)
		{
			createdRenderTexture = false;
			if ((bool)renderTexture)
			{
				renderTexture.Release();
				Object.Destroy(renderTexture);
				renderTexture = null;
			}
		}
	}
}

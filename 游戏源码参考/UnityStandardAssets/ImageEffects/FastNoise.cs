using System.Collections.Generic;
using UnityEngine;

namespace UnityStandardAssets.ImageEffects
{
	[RequireComponent(typeof(Camera))]
	[AddComponentMenu("Image Effects/Noise/Fast Noise")]
	public class FastNoise : PostEffectsBase
	{
		public enum FrameMultiple
		{
			Always = 1,
			Half = 2,
			Third = 3,
			Quarter = 4,
			Fifth = 5,
			Sixth = 6,
			Eighth = 8,
			Tenth = 10
		}

		public static readonly Vector2 ReferenceRes = new Vector2(1920f, 1080f);

		public static readonly Vector2 InverseRef = new Vector2(1f / ReferenceRes.x, 1f / ReferenceRes.y);

		private bool monochrome = true;

		[Header("Update Rate")]
		public FrameMultiple frameRateMultiplier = FrameMultiple.Always;

		[Header("Intensity")]
		public float intensityMultiplier = 0.25f;

		public float generalIntensity = 0.5f;

		public float blackIntensity = 1f;

		public float whiteIntensity = 1f;

		[Range(0f, 1f)]
		public float midGrey = 0.2f;

		[Header("Noise Shape")]
		public Texture2D noiseTexture;

		public FilterMode filterMode = FilterMode.Bilinear;

		[Range(0f, 0.99f)]
		private Vector3 intensities = new Vector3(1f, 1f, 1f);

		[Range(0f, 0.99f)]
		public float softness = 0.052f;

		[Header("Advanced")]
		public float monochromeTiling = 64f;

		public Shader noiseShader;

		private Material noiseMaterial;

		private static float TILE_AMOUNT = 64f;

		private byte frameCount;

		private int previousWidth;

		private int previousHeight;

		private readonly List<float> tcStartRandoms = new List<float>();

		private bool effectIsSupported;

		private bool hasMaterial;

		private static readonly int _noiseTex = Shader.PropertyToID("_NoiseTex");

		private static readonly int _noisePerChannel = Shader.PropertyToID("_NoisePerChannel");

		private static readonly int _noiseTilingPerChannel = Shader.PropertyToID("_NoiseTilingPerChannel");

		private static readonly int _midGrey = Shader.PropertyToID("_MidGrey");

		private static readonly int _noiseAmount = Shader.PropertyToID("_NoiseAmount");

		private static readonly int _mainTex = Shader.PropertyToID("_MainTex");

		public bool ForceRender { get; set; }

		public static FastNoise Instance { get; private set; }

		public void Init()
		{
			Instance = this;
		}

		protected override void OnEnable()
		{
			base.OnEnable();
			effectIsSupported = CheckResources() && hasMaterial;
		}

		public override bool CheckResources()
		{
			CheckSupport(needDepth: false);
			noiseMaterial = CheckShaderAndCreateMaterial(noiseShader, noiseMaterial);
			hasMaterial = noiseMaterial;
			if (!isSupported)
			{
				ReportAutoDisable();
			}
			return isSupported;
		}

		private void OnRenderImage(RenderTexture source, RenderTexture destination)
		{
			if (!effectIsSupported)
			{
				Graphics.Blit(source, destination);
				if (!hasMaterial)
				{
					Debug.LogWarning("FastNoise effect failing as noise texture is not assigned. please assign.", base.transform);
				}
				return;
			}
			softness = Mathf.Clamp(softness, 0f, 0.99f);
			if ((bool)noiseTexture)
			{
				noiseTexture.wrapMode = TextureWrapMode.Repeat;
				noiseTexture.filterMode = filterMode;
			}
			noiseMaterial.SetTexture(_noiseTex, noiseTexture);
			noiseMaterial.SetVector(_noisePerChannel, monochrome ? Vector3.one : intensities);
			Vector2 one = Vector2.one;
			noiseMaterial.SetVector(_noiseTilingPerChannel, one * monochromeTiling);
			noiseMaterial.SetVector(_midGrey, new Vector3(midGrey, 1f / (1f - midGrey), -1f / midGrey));
			noiseMaterial.SetVector(_noiseAmount, new Vector3(generalIntensity, blackIntensity, whiteIntensity) * intensityMultiplier);
			int frameMultiple = (int)(ForceRender ? FrameMultiple.Always : frameRateMultiplier);
			if (softness > Mathf.Epsilon)
			{
				RenderTexture temporary = RenderTexture.GetTemporary((int)((float)source.width * (1f - softness)), (int)((float)source.height * (1f - softness)));
				DrawNoiseQuadGrid(source, temporary, noiseMaterial, noiseTexture, 2, frameMultiple);
				noiseMaterial.SetTexture(_noiseTex, temporary);
				Graphics.Blit(source, destination, noiseMaterial, 1);
				noiseMaterial.SetTexture(_noiseTex, null);
				RenderTexture.ReleaseTemporary(temporary);
			}
			else
			{
				DrawNoiseQuadGrid(source, destination, noiseMaterial, noiseTexture, 0, frameMultiple);
			}
			frameCount++;
		}

		private void DrawNoiseQuadGrid(RenderTexture source, RenderTexture dest, Material fxMaterial, Texture2D noise, int passNr, int frameMultiple)
		{
			float num = (float)noise.width * 1f;
			float x = ReferenceRes.x;
			float num2 = 1f * x / TILE_AMOUNT;
			float num3 = 1f * (float)source.width / (1f * (float)source.height);
			float num4 = 1f / num2;
			float num5 = num4 * num3;
			float num6 = num / ((float)noise.width * 1f);
			if (Time.frameCount % frameMultiple == 0 || tcStartRandoms.Count == 0 || source.width != previousWidth || source.height != previousHeight)
			{
				tcStartRandoms.Clear();
				for (float num7 = 0f; num7 < 1f; num7 += num4)
				{
					for (float num8 = 0f; num8 < 1f; num8 += num5)
					{
						float item = Random.Range(0f, 1f);
						tcStartRandoms.Add(item);
						float item2 = Random.Range(0f, 1f);
						tcStartRandoms.Add(item2);
					}
				}
			}
			previousWidth = source.width;
			previousHeight = source.height;
			RenderTexture active = RenderTexture.active;
			RenderTexture.active = dest;
			fxMaterial.SetTexture(_mainTex, source);
			GL.PushMatrix();
			GL.LoadOrtho();
			fxMaterial.SetPass(passNr);
			GL.Begin(7);
			int tcStartIndex = 0;
			for (float num9 = 0f; num9 < 1f; num9 += num4)
			{
				for (float num10 = 0f; num10 < 1f; num10 += num5)
				{
					float num11 = GetNextTcStart();
					float num12 = GetNextTcStart();
					num11 = Mathf.Floor(num11 * num) / num;
					num12 = Mathf.Floor(num12 * num) / num;
					float num13 = 1f / num;
					GL.MultiTexCoord2(0, num11, num12);
					GL.MultiTexCoord2(1, 0f, 0f);
					GL.Vertex3(num9, num10, 0.1f);
					GL.MultiTexCoord2(0, num11 + num6 * num13, num12);
					GL.MultiTexCoord2(1, 1f, 0f);
					GL.Vertex3(num9 + num4, num10, 0.1f);
					GL.MultiTexCoord2(0, num11 + num6 * num13, num12 + num6 * num13);
					GL.MultiTexCoord2(1, 1f, 1f);
					GL.Vertex3(num9 + num4, num10 + num5, 0.1f);
					GL.MultiTexCoord2(0, num11, num12 + num6 * num13);
					GL.MultiTexCoord2(1, 0f, 1f);
					GL.Vertex3(num9, num10 + num5, 0.1f);
				}
			}
			GL.End();
			GL.PopMatrix();
			fxMaterial.SetTexture(_mainTex, null);
			RenderTexture.active = active;
			float GetNextTcStart()
			{
				float result = tcStartRandoms[tcStartIndex];
				tcStartIndex++;
				return result;
			}
		}
	}
}

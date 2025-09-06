using UnityEngine;
using UnityStandardAssets.ImageEffects;

public sealed class NewCameraNoise : MonoBehaviour, IPostprocessModule
{
	[SerializeField]
	private Material material;

	[Space]
	[SerializeField]
	private float density = 6.4f;

	[SerializeField]
	private Vector2 densityRatio = new Vector2(1f, 1f);

	[SerializeField]
	private float noiseStrength = 0.25f;

	[SerializeField]
	private float timeSnap = 1f / 60f;

	[SerializeField]
	private Vector4 colorScale = new Vector4(1f, 1f, 1f, 1f);

	[SerializeField]
	private Color colorPreview;

	[Header("Noise Texture")]
	[SerializeField]
	private int noiseWidth = 1024;

	[SerializeField]
	private int noiseHeight = 1024;

	[Header("Debug")]
	[SerializeField]
	private Texture noiseTexture;

	private static NewCameraNoise instance;

	private static readonly int NOISE_STRENGTH = Shader.PropertyToID("_NoiseStrength");

	private static readonly int TIME_SNAP = Shader.PropertyToID("_TimeSnap");

	private static readonly int NOISE_TEXTURE = Shader.PropertyToID("_NoiseTex");

	private static readonly int NOISE_COLOR = Shader.PropertyToID("_NoiseColor");

	private bool isStarting;

	public float Density
	{
		get
		{
			return density;
		}
		set
		{
			density = value;
			if (AutoUpdateTexture)
			{
				UpdateDensity();
			}
		}
	}

	public Vector2 DensityRatio
	{
		get
		{
			return densityRatio;
		}
		set
		{
			densityRatio = value;
			if (AutoUpdateTexture)
			{
				UpdateDensity();
			}
		}
	}

	public float NoiseStrength
	{
		get
		{
			return noiseStrength;
		}
		set
		{
			noiseStrength = value;
		}
	}

	public float TimeSnap
	{
		get
		{
			return timeSnap;
		}
		set
		{
			timeSnap = value;
			if (timeSnap <= 0f)
			{
				timeSnap = 0.0033333334f;
			}
		}
	}

	public float R
	{
		get
		{
			return colorScale.x;
		}
		set
		{
			colorScale.x = Mathf.Max(0f, value);
		}
	}

	public float G
	{
		get
		{
			return colorScale.y;
		}
		set
		{
			colorScale.y = Mathf.Max(0f, value);
		}
	}

	public float B
	{
		get
		{
			return colorScale.z;
		}
		set
		{
			colorScale.z = Mathf.Max(0f, value);
		}
	}

	public bool AutoUpdateTexture { get; set; } = true;

	public static NewCameraNoise Instance
	{
		get
		{
			if (!HasInstance)
			{
				instance = Object.FindObjectOfType<NewCameraNoise>();
				HasInstance = instance;
			}
			return instance;
		}
		private set
		{
			instance = value;
			HasInstance = instance;
		}
	}

	public static bool HasInstance { get; private set; }

	public string EffectKeyword => "NOISE_ENABLED";

	private void Awake()
	{
		Instance = this;
	}

	private void Start()
	{
		isStarting = true;
		Density = density;
		DensityRatio = densityRatio;
		NoiseStrength = noiseStrength;
		TimeSnap = timeSnap;
		isStarting = false;
		UpdateDensity();
		if (!IsEnabledOnPlatform() || ((bool)FastNoise.Instance && FastNoise.Instance.enabled))
		{
			base.enabled = false;
		}
	}

	private void OnDestroy()
	{
		if (Instance == this)
		{
			Instance = null;
		}
		if (noiseTexture != null)
		{
			Object.Destroy(noiseTexture);
		}
	}

	private static bool IsEnabledOnPlatform()
	{
		return true;
	}

	private Color GenerateColorPreview(Vector4 scale)
	{
		float num = Mathf.Max(scale.x, scale.y, scale.z);
		float value = scale.x / num;
		float value2 = scale.y / num;
		float value3 = scale.z / num;
		float w = scale.w;
		float r = Mathf.Clamp(value, 0f, 1f);
		value2 = Mathf.Clamp(value2, 0f, 1f);
		value3 = Mathf.Clamp(value3, 0f, 1f);
		w = Mathf.Clamp(w, 0f, 1f);
		return new Color(r, value2, value3, w);
	}

	public Color GetNoiseColor()
	{
		return GenerateColorPreview(colorScale);
	}

	private Texture2D GenerateNoiseTexture(int width, int height)
	{
		Texture2D texture2D = new Texture2D(width, height);
		texture2D.name = $"{this} NoiseTexture";
		Vector2 vector = densityRatio * density;
		if (vector.x > 0f)
		{
			vector.x = 1920f / vector.x;
		}
		else
		{
			vector.x = 0f;
		}
		if (vector.y > 0f)
		{
			vector.y = 1080f / vector.y;
		}
		else
		{
			vector.y = 0f;
		}
		for (int i = 0; i < width; i++)
		{
			for (int j = 0; j < height; j++)
			{
				float x = (float)i / (float)width * vector.x;
				float y = (float)j / (float)height * vector.y;
				float num = Mathf.PerlinNoise(x, y);
				texture2D.SetPixel(i, j, new Color(num, num, num, 1f));
			}
		}
		texture2D.wrapMode = TextureWrapMode.Repeat;
		texture2D.filterMode = FilterMode.Bilinear;
		texture2D.Apply();
		return texture2D;
	}

	private void UpdateDensity()
	{
		if (!isStarting)
		{
			UpdateNoiseTexture();
		}
	}

	public void UpdateNoiseTexture()
	{
		if (noiseTexture != null)
		{
			Object.Destroy(noiseTexture);
		}
		noiseTexture = GenerateNoiseTexture(noiseWidth, noiseHeight);
	}

	public void UpdateProperties(Material material)
	{
		material.SetFloat(NOISE_STRENGTH, noiseStrength);
		material.SetFloat(TIME_SNAP, timeSnap);
		material.SetTexture(NOISE_TEXTURE, noiseTexture);
		material.SetVector(NOISE_COLOR, colorScale);
	}
}

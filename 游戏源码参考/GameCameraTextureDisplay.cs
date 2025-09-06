using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class GameCameraTextureDisplay : MonoBehaviour
{
	public static GameCameraTextureDisplay Instance;

	private RenderTexture texture;

	public RawImage image;

	public Image altImage;

	private static readonly int _brightnessProp = Shader.PropertyToID("_Brightness");

	private void Awake()
	{
		if (!Instance)
		{
			Instance = this;
		}
		SceneManager.activeSceneChanged += OnActiveSceneChanged;
	}

	private void Start()
	{
		UpdateActiveImage();
	}

	private void OnDestroy()
	{
		SceneManager.activeSceneChanged -= OnActiveSceneChanged;
		if (Instance == this)
		{
			Instance = null;
		}
		if (texture != null)
		{
			texture.Release();
			Object.Destroy(texture);
			texture = null;
		}
	}

	private void OnActiveSceneChanged(Scene arg0, Scene scene)
	{
		UpdateActiveImage();
	}

	private void UpdateActiveImage()
	{
		if ((bool)GameManager.instance && GameManager.instance.sceneName != "Menu_Title")
		{
			image.enabled = true;
			altImage.enabled = false;
			if (texture == null)
			{
				image.color = Color.black;
			}
		}
		else
		{
			image.enabled = false;
			altImage.enabled = true;
		}
	}

	public void UpdateDisplay(RenderTexture source, Material material)
	{
		if (base.gameObject.activeInHierarchy)
		{
			if (texture != null && (texture.width != source.width || texture.height != source.height))
			{
				image.texture = null;
				texture.Release();
				texture = null;
			}
			if (texture == null)
			{
				texture = new RenderTexture(source.width, source.height, source.depth);
				texture.name = "GameCameraTextureDisplay" + GetInstanceID();
				image.texture = texture;
				image.color = Color.white;
				float num = (float)source.width / (float)source.height;
				RectTransform rectTransform = image.rectTransform;
				Vector2 sizeDelta = rectTransform.sizeDelta;
				sizeDelta.x = sizeDelta.y * num;
				rectTransform.sizeDelta = sizeDelta;
			}
			Graphics.Blit(source, texture, material);
		}
	}

	public void UpdateBrightness(float brightness)
	{
		altImage.material.SetFloat(_brightnessProp, brightness);
	}
}

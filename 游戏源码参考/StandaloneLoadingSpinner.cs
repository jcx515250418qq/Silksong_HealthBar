using System;
using UnityEngine;
using UnityEngine.UI;

public class StandaloneLoadingSpinner : MonoBehaviour
{
	[SerializeField]
	private Camera renderingCamera;

	[SerializeField]
	private Image backgroundImage;

	[SerializeField]
	private Image image;

	[SerializeField]
	private float displayDelay;

	[SerializeField]
	private float fadeDuration;

	[SerializeField]
	private float fadeAmount;

	[SerializeField]
	private float fadeVariance;

	[SerializeField]
	private float fadePulseDuration;

	[SerializeField]
	private Sprite[] sprites;

	[SerializeField]
	private float frameRate;

	private float fadeFactor;

	private float frameTimer;

	private int frameIndex;

	private float timeRunning;

	private bool isComplete;

	private GameManager lastGameManager;

	private void OnValidate()
	{
		if (frameRate <= 0f)
		{
			frameRate = 12f;
		}
	}

	public void Setup(GameManager lastGameManager)
	{
		this.lastGameManager = lastGameManager;
	}

	protected void OnEnable()
	{
		fadeFactor = 0f;
		if (frameRate <= 0f)
		{
			frameRate = 12f;
		}
	}

	protected void Start()
	{
		image.color = new Color(1f, 1f, 1f, 0f);
		image.enabled = false;
		fadeFactor = 0f;
		UnityEngine.Object.DontDestroyOnLoad(base.gameObject);
	}

	protected void LateUpdate()
	{
		GameManager unsafeInstance = GameManager.UnsafeInstance;
		if (lastGameManager == null && unsafeInstance != null && (lastGameManager != unsafeInstance || lastGameManager == null) && !isComplete)
		{
			renderingCamera.enabled = false;
			isComplete = true;
		}
		timeRunning += Time.unscaledDeltaTime;
		float unscaledDeltaTime = Time.unscaledDeltaTime;
		float target = ((timeRunning > displayDelay && !isComplete) ? 1f : 0f);
		fadeFactor = Mathf.MoveTowards(fadeFactor, target, unscaledDeltaTime / fadeDuration);
		if (fadeFactor < Mathf.Epsilon)
		{
			if (image.enabled)
			{
				image.enabled = false;
			}
			if (isComplete)
			{
				UnityEngine.Object.Destroy(base.gameObject);
			}
		}
		else
		{
			if (!image.enabled)
			{
				image.enabled = true;
			}
			image.color = new Color(1f, 1f, 1f, fadeFactor * (fadeAmount + fadeVariance * Mathf.Sin(timeRunning * MathF.PI * 2f / fadePulseDuration)));
		}
		if (sprites.Length != 0)
		{
			frameTimer += unscaledDeltaTime * frameRate;
			if (frameTimer >= 1f)
			{
				int num = Mathf.FloorToInt(frameTimer);
				frameTimer -= num;
				frameIndex = (frameIndex + num) % sprites.Length;
				SetImage(sprites[frameIndex]);
			}
		}
	}

	private void SetImage(Sprite sprite)
	{
		if (image.sprite != sprite)
		{
			image.sprite = sprite;
		}
	}
}

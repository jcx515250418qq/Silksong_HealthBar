using System;
using UnityEngine;
using UnityEngine.UI;

public class LoadingSpinner : MonoBehaviour
{
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

	private bool targetActive;

	public float DisplayDelayAdjustment { get; set; }

	public float DisplayDelay => displayDelay + DisplayDelayAdjustment;

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
	}

	protected void Update()
	{
		float unscaledDeltaTime = Time.unscaledDeltaTime;
		GameManager silentInstance = GameManager.SilentInstance;
		if (silentInstance != null)
		{
			fadeFactor = Mathf.MoveTowards(target: (!targetActive) ? 0f : ((!(silentInstance.CurrentLoadDuration > DisplayDelay)) ? 0f : 1f), current: fadeFactor, maxDelta: unscaledDeltaTime / fadeDuration);
			if (fadeFactor < Mathf.Epsilon)
			{
				if (image.enabled)
				{
					image.enabled = false;
				}
				if (!targetActive)
				{
					base.gameObject.SetActive(value: false);
				}
			}
			else
			{
				if (!image.enabled)
				{
					image.enabled = true;
				}
				image.color = new Color(1f, 1f, 1f, fadeFactor * (fadeAmount + fadeVariance * Mathf.Sin(silentInstance.CurrentLoadDuration * MathF.PI * 2f / fadePulseDuration)));
			}
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

	public void SetActive(bool value, bool isInstant)
	{
		targetActive = value;
		if (value)
		{
			if (!base.gameObject.activeSelf)
			{
				base.gameObject.SetActive(value: true);
			}
		}
		else if (isInstant)
		{
			base.gameObject.SetActive(value: false);
		}
	}
}

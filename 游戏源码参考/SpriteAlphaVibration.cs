using UnityEngine;

public sealed class SpriteAlphaVibration : ScaledVibration
{
	[Space]
	[SerializeField]
	private SpriteRenderer spriteRenderer;

	[SerializeField]
	private float alphaToStrengthRate = 1f;

	private bool hasSprite;

	private bool isPlaying;

	private void Awake()
	{
		hasSprite = spriteRenderer;
		if (!hasSprite)
		{
			spriteRenderer = GetComponent<SpriteRenderer>();
			hasSprite = spriteRenderer;
			_ = hasSprite;
		}
	}

	private void Start()
	{
		if (!isPlaying)
		{
			base.enabled = false;
		}
	}

	private void OnValidate()
	{
		hasSprite = spriteRenderer;
		if (!hasSprite)
		{
			spriteRenderer = GetComponent<SpriteRenderer>();
			hasSprite = spriteRenderer;
		}
	}

	private void OnDestroy()
	{
		StopVibration();
	}

	private void OnDisable()
	{
		StopVibration();
	}

	private void LateUpdate()
	{
		emission.SetStrength(GetStrength());
	}

	public override void PlayVibration(float fade = 0f)
	{
		if (emission == null || !emission.IsPlaying)
		{
			VibrationData vibrationData = vibrationDataAsset.VibrationData;
			bool isLooping = loop;
			bool isRealtime = isRealTime;
			string text = tag;
			emission = VibrationManager.PlayVibrationClipOneShot(vibrationData, null, isLooping, text, isRealtime);
			base.enabled = (isPlaying = emission != null);
			if (isPlaying)
			{
				FadeInEmission(fade);
				emission.SetStrength(GetStrength());
			}
		}
	}

	public override void StopVibration()
	{
		emission?.Stop();
		emission = null;
		base.enabled = false;
		isPlaying = false;
	}

	private float GetStrength()
	{
		if (!hasSprite)
		{
			return 0f;
		}
		return spriteRenderer.color.a * alphaToStrengthRate * internalStrength;
	}
}

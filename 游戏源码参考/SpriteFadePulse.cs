using UnityEngine;

public class SpriteFadePulse : MonoBehaviour
{
	public float lowAlpha;

	public float highAlpha;

	public float fadeDuration;

	public bool startPaused;

	private bool paused;

	private SpriteRenderer spriteRenderer;

	private int state;

	private float currentLerpTime;

	private float currentAlpha;

	private float lowAlphaOriginal;

	private void Awake()
	{
		spriteRenderer = GetComponent<SpriteRenderer>();
		lowAlphaOriginal = lowAlpha;
		paused = startPaused;
	}

	private void Start()
	{
		ResetFade();
	}

	private void OnEnable()
	{
		ResetFade();
	}

	private void ResetFade()
	{
		lowAlpha = lowAlphaOriginal;
		paused = startPaused;
		currentLerpTime = 0f;
		Color color = spriteRenderer.color;
		color.a = lowAlpha;
		spriteRenderer.color = color;
		FadeIn();
	}

	private void Update()
	{
		if (paused)
		{
			return;
		}
		float t = currentLerpTime / fadeDuration;
		currentAlpha = Mathf.Lerp(lowAlpha, highAlpha, t);
		Color color = spriteRenderer.color;
		color.a = currentAlpha;
		spriteRenderer.color = color;
		if (state == 0)
		{
			currentLerpTime += Time.deltaTime;
			if (currentLerpTime > fadeDuration)
			{
				FadeOut();
			}
		}
		else if (state == 1)
		{
			currentLerpTime -= Time.deltaTime;
			if (currentLerpTime < 0f)
			{
				FadeIn();
			}
		}
		else
		{
			currentLerpTime -= Time.deltaTime;
		}
	}

	public void FadeIn()
	{
		state = 0;
		currentLerpTime = 0f;
	}

	public void FadeOut()
	{
		state = 1;
		currentLerpTime = fadeDuration;
	}

	public void EndFade()
	{
		lowAlpha = 0f;
		state = 2;
	}

	public void PauseFade()
	{
		paused = true;
	}

	public void UnpauseFade()
	{
		paused = false;
	}
}

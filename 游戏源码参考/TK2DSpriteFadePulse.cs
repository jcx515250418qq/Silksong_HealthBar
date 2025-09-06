using UnityEngine;

public class TK2DSpriteFadePulse : MonoBehaviour
{
	public float lowAlpha;

	public float highAlpha;

	public float fadeDuration;

	public bool startPaused;

	private bool paused;

	private tk2dSprite tk2d_sprite;

	private int state;

	private float currentLerpTime;

	private float currentAlpha;

	private float lowAlphaOriginal;

	private void Awake()
	{
		tk2d_sprite = GetComponent<tk2dSprite>();
		lowAlphaOriginal = lowAlpha;
		paused = startPaused;
	}

	private void OnEnable()
	{
		lowAlpha = lowAlphaOriginal;
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
		Color color = tk2d_sprite.color;
		color.a = currentAlpha;
		tk2d_sprite.color = color;
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

using TeamCherry.NestedFadeGroup;
using UnityEngine;

public class SimpleSpriteFade : MonoBehaviour
{
	[SerializeField]
	private SpriteRenderer spriteRenderer;

	[Space]
	public Color fadeInColor;

	private NestedFadeGroupBase fadeGroup;

	private Color normalColor;

	public float fadeDuration;

	private bool fadingIn;

	private bool fadingOut;

	private float currentLerpTime;

	public bool fadeInOnStart;

	public bool deactivateOnFadeIn;

	public bool recycleOnFadeIn;

	public bool isRealtime;

	private void Awake()
	{
		if (!spriteRenderer)
		{
			spriteRenderer = GetComponent<SpriteRenderer>();
		}
		fadeGroup = GetComponent<NestedFadeGroupBase>();
		normalColor = spriteRenderer.color;
		if ((bool)fadeGroup)
		{
			normalColor.a = fadeGroup.AlphaSelf;
		}
	}

	private void OnEnable()
	{
		spriteRenderer.color = normalColor;
		if (fadeInOnStart)
		{
			FadeIn();
		}
	}

	private void Update()
	{
		float num = (isRealtime ? Time.unscaledDeltaTime : Time.deltaTime);
		if (!fadingIn && !fadingOut)
		{
			return;
		}
		if (fadingIn)
		{
			currentLerpTime += num;
			if (currentLerpTime > fadeDuration)
			{
				currentLerpTime = fadeDuration;
				fadingIn = false;
				if (recycleOnFadeIn)
				{
					base.gameObject.Recycle();
				}
				if (deactivateOnFadeIn)
				{
					base.gameObject.SetActive(value: false);
				}
			}
		}
		else if (fadingOut)
		{
			currentLerpTime -= num;
			if (currentLerpTime < 0f)
			{
				currentLerpTime = 0f;
				fadingOut = false;
			}
		}
		float t = currentLerpTime / fadeDuration;
		Color color = Color.Lerp(normalColor, fadeInColor, t);
		spriteRenderer.color = color;
		if ((bool)fadeGroup)
		{
			fadeGroup.AlphaSelf = color.a;
		}
	}

	public void FadeIn()
	{
		fadingIn = true;
		currentLerpTime = 0f;
	}

	public void FadeOut()
	{
		fadingOut = true;
		currentLerpTime = fadeDuration;
	}

	public void SetDuration(float newDuration)
	{
		fadeDuration = newDuration;
	}
}

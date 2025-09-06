using UnityEngine;

public class SimpleFadeOut : MonoBehaviour
{
	[SerializeField]
	[Tooltip("The time to complete one half cycle of a pulse.")]
	private float fadeDuration = 1f;

	[SerializeField]
	private bool waitForCall;

	[SerializeField]
	private bool resetOnEnable;

	[SerializeField]
	private bool isRealtime;

	private Color startColor;

	private Color fadeColor;

	private float currentLerpTime;

	private Color originalColour;

	private SpriteRenderer spriteRenderer;

	private void Awake()
	{
		spriteRenderer = GetComponent<SpriteRenderer>();
		originalColour = spriteRenderer.color;
	}

	private void OnEnable()
	{
		ResetFade();
	}

	private void ResetFade()
	{
		currentLerpTime = 0f;
		if (resetOnEnable)
		{
			spriteRenderer.color = originalColour;
		}
		startColor = spriteRenderer.color;
		Color original = startColor;
		float? a = 0f;
		fadeColor = original.Where(null, null, null, a);
	}

	private void Update()
	{
		if (!waitForCall)
		{
			currentLerpTime += (isRealtime ? Time.unscaledDeltaTime : Time.deltaTime);
			if (currentLerpTime > fadeDuration)
			{
				currentLerpTime = fadeDuration;
				base.gameObject.SetActive(value: false);
			}
			float t = currentLerpTime / fadeDuration;
			spriteRenderer.color = Color.Lerp(startColor, fadeColor, t);
		}
	}

	public void FadeOut()
	{
		waitForCall = false;
	}

	public void SetColor(Color color)
	{
		spriteRenderer.color = color;
		ResetFade();
	}
}

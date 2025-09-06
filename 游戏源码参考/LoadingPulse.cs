using UnityEngine;
using UnityEngine.UI;

public class LoadingPulse : MonoBehaviour
{
	public Color pulseColor;

	public float pulseDuration;

	private Image sprite;

	private Color normalColor;

	private bool pulsing;

	private bool reverse;

	private float currentLerpTime;

	private void Start()
	{
		sprite = GetComponent<Image>();
		normalColor = sprite.color;
		pulsing = true;
		currentLerpTime = 0f;
	}

	private void Update()
	{
		if (!pulsing)
		{
			return;
		}
		if (!reverse)
		{
			currentLerpTime += Time.deltaTime;
			if (currentLerpTime > pulseDuration)
			{
				currentLerpTime = pulseDuration;
				reverse = true;
			}
		}
		else
		{
			currentLerpTime -= Time.deltaTime;
			if (currentLerpTime < 0f)
			{
				currentLerpTime = 0f;
				reverse = false;
			}
		}
		float t = currentLerpTime / pulseDuration;
		sprite.color = Color.Lerp(normalColor, pulseColor, t);
	}
}

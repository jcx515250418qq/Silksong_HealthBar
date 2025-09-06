using TeamCherry.NestedFadeGroup;
using UnityEngine;
using UnityEngine.UI;

public class ImageSlider : MonoBehaviour
{
	[SerializeField]
	private Image image;

	[SerializeField]
	private float minDisplayValue;

	[SerializeField]
	private NestedFadeGroupSpriteRenderer glower;

	private float value;

	public float Value
	{
		get
		{
			return value;
		}
		set
		{
			DisplayValue(value);
		}
	}

	public Color Color
	{
		get
		{
			return image.color;
		}
		set
		{
			image.color = value;
			if ((bool)glower)
			{
				glower.Color = value;
			}
		}
	}

	private void OnEnable()
	{
		DisplayValue(value);
	}

	private void DisplayValue(float newValue)
	{
		value = newValue;
		if (value > 0f && value < minDisplayValue)
		{
			value = minDisplayValue;
		}
		if ((bool)image)
		{
			image.fillAmount = value;
		}
		if ((bool)glower)
		{
			glower.gameObject.SetActive(value >= 1f);
		}
	}
}

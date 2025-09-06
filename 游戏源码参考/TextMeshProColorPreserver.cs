using TMProOld;
using UnityEngine;

public sealed class TextMeshProColorPreserver : MonoBehaviour
{
	[SerializeField]
	private TextMeshPro tmpText;

	[SerializeField]
	private Color targetColor = Color.white;

	private void Awake()
	{
		if (tmpText == null)
		{
			tmpText = GetComponent<TextMeshPro>();
		}
		if (tmpText != null)
		{
			tmpText.color = targetColor;
		}
	}

	private void Reset()
	{
		if (tmpText == null)
		{
			tmpText = GetComponent<TextMeshPro>();
		}
		if (tmpText != null)
		{
			targetColor = tmpText.color;
		}
	}

	private void OnValidate()
	{
		if (tmpText == null)
		{
			tmpText = GetComponent<TextMeshPro>();
		}
		if (tmpText != null)
		{
			tmpText.color = targetColor;
		}
	}

	private void OnEnable()
	{
		if (tmpText != null)
		{
			tmpText.color = targetColor;
		}
	}
}

using TMProOld;
using UnityEngine;

public class TextBridge : MonoBehaviour
{
	[SerializeField]
	private TextMesh textMesh;

	[SerializeField]
	private TMP_Text tmpText;

	public string Text
	{
		get
		{
			if ((bool)textMesh)
			{
				return textMesh.text;
			}
			if ((bool)tmpText)
			{
				return tmpText.text;
			}
			return string.Empty;
		}
		set
		{
			if ((bool)textMesh && textMesh.text != value)
			{
				textMesh.text = value;
			}
			if ((bool)tmpText)
			{
				tmpText.text = value;
			}
		}
	}

	public Color Color
	{
		get
		{
			if ((bool)textMesh)
			{
				return textMesh.color;
			}
			if ((bool)tmpText)
			{
				return tmpText.color;
			}
			return Color.white;
		}
		set
		{
			if ((bool)textMesh && textMesh.color != value)
			{
				textMesh.color = value;
			}
			if ((bool)tmpText)
			{
				tmpText.color = value;
			}
		}
	}

	public void FindComponent()
	{
		if (textMesh == null)
		{
			textMesh = GetComponent<TextMesh>();
		}
		if (tmpText == null)
		{
			tmpText = GetComponent<TMP_Text>();
		}
	}
}

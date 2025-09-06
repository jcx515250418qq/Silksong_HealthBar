using UnityEngine;
using UnityEngine.UI;

[ExecuteInEditMode]
[RequireComponent(typeof(Text))]
public class FixVerticalAlign : MonoBehaviour
{
	public enum LabelFixType
	{
		Normal = 0,
		KeyMap = 1
	}

	private Text text;

	private bool hasText;

	public LabelFixType labelFixType;

	private void OnEnable()
	{
		AlignAuto();
	}

	private void Start()
	{
		AlignAuto();
	}

	private void FindText()
	{
		if (!hasText)
		{
			text = GetComponent<Text>();
			hasText = text;
		}
	}

	public void AlignAuto()
	{
		if (labelFixType == LabelFixType.Normal)
		{
			AlignText();
		}
		else if (labelFixType == LabelFixType.KeyMap)
		{
			AlignTextKeymap();
		}
	}

	public void AlignText()
	{
		FindText();
		if (!string.IsNullOrEmpty(text.text))
		{
			if (text.text[text.text.Length - 1] != '\n')
			{
				text.text += "\n";
			}
			text.lineSpacing = -0.33f;
		}
	}

	public void AlignTextKeymap()
	{
		FindText();
		if (!string.IsNullOrEmpty(text.text))
		{
			if (text.text[text.text.Length - 1] != '\n')
			{
				text.text += "\n";
			}
			text.lineSpacing = -0.05f;
		}
	}
}

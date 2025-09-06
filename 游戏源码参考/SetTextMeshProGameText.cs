using System.Collections.Generic;
using System.Text;
using TMProOld;
using TeamCherry.Localization;
using UnityEngine;

public class SetTextMeshProGameText : MonoBehaviour
{
	[SerializeField]
	private LocalisedString text;

	[SerializeField]
	private bool toSingleLine;

	[SerializeField]
	private TMP_Text setTextOn;

	[SerializeField]
	private List<TMP_Text> splitLinesAcross;

	[HideInInspector]
	public string sheetName;

	[HideInInspector]
	public string convName;

	private bool started;

	private bool init;

	public LocalisedString Text
	{
		get
		{
			return text;
		}
		set
		{
			text = value;
			UpdateText();
		}
	}

	private void OnValidate()
	{
		if (text.IsEmpty && (!string.IsNullOrEmpty(sheetName) || !string.IsNullOrEmpty(convName)))
		{
			text.Sheet = sheetName;
			text.Key = convName;
			sheetName = string.Empty;
			convName = string.Empty;
		}
	}

	private void Awake()
	{
		Init();
	}

	private void Start()
	{
		started = true;
		UpdateText();
	}

	private void Init()
	{
		if (!init)
		{
			init = true;
			OnValidate();
			CleanSplitLines();
			if (splitLinesAcross.Count == 0 && !setTextOn)
			{
				setTextOn = GetComponent<TextMeshPro>();
			}
		}
	}

	private void OnEnable()
	{
		if (CheatManager.ForceLanguageComponentUpdates && started)
		{
			UpdateText();
		}
	}

	private void CleanSplitLines()
	{
		splitLinesAcross.RemoveAll((TMP_Text o) => o == null);
	}

	[ContextMenu("Update Text")]
	public void UpdateText()
	{
		Init();
		string text = (toSingleLine ? this.text.ToString().ToSingleLine() : ((string)this.text));
		if ((bool)setTextOn)
		{
			setTextOn.text = text;
		}
		if (splitLinesAcross.Count <= 0)
		{
			return;
		}
		string[] array = text.Split('\n');
		CleanSplitLines();
		if (splitLinesAcross.Count == 0)
		{
			return;
		}
		StringBuilder[] array2 = new StringBuilder[splitLinesAcross.Count];
		int num = 0;
		string[] array3 = array;
		foreach (string value in array3)
		{
			StringBuilder stringBuilder = array2[num];
			if (stringBuilder == null)
			{
				stringBuilder = (array2[num] = new StringBuilder());
			}
			stringBuilder.AppendLine(value);
			num++;
			if (num >= splitLinesAcross.Count)
			{
				num = 0;
			}
		}
		for (int j = 0; j < array2.Length; j++)
		{
			StringBuilder stringBuilder2 = array2[j];
			if (stringBuilder2 != null)
			{
				splitLinesAcross[j].text = stringBuilder2.ToString();
			}
		}
	}
}

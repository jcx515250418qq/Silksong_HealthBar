using TeamCherry.Localization;
using UnityEngine;
using UnityEngine.UI;

public class AutoLocalizeTextUI : MonoBehaviour
{
	[SerializeField]
	[Tooltip("UI Text component to place text.")]
	private Text textField;

	[SerializeField]
	private LocalisedString text;

	[SerializeField]
	[HideInInspector]
	private string sheetTitle;

	[SerializeField]
	[HideInInspector]
	private string textKey;

	private GameManager gm;

	private FixVerticalAlign textAligner;

	private bool hasTextAligner;

	public LocalisedString Text
	{
		get
		{
			return text;
		}
		set
		{
			text = value;
			RefreshTextFromLocalization();
		}
	}

	public string TextSheet
	{
		get
		{
			return Text.Sheet;
		}
		set
		{
			LocalisedString localisedString = Text;
			localisedString.Sheet = value;
			Text = localisedString;
		}
	}

	public string TextKey
	{
		get
		{
			return Text.Key;
		}
		set
		{
			LocalisedString localisedString = Text;
			localisedString.Key = value;
			Text = localisedString;
		}
	}

	private void OnValidate()
	{
		if (!string.IsNullOrEmpty(sheetTitle) || !string.IsNullOrEmpty(textKey))
		{
			text = new LocalisedString(sheetTitle, textKey);
			sheetTitle = string.Empty;
			textKey = string.Empty;
		}
	}

	private void Awake()
	{
		OnValidate();
		textAligner = GetComponent<FixVerticalAlign>();
		if ((bool)textAligner)
		{
			hasTextAligner = true;
		}
	}

	private void OnEnable()
	{
		gm = GameManager.instance;
		if ((bool)gm)
		{
			gm.RefreshLanguageText += RefreshTextFromLocalization;
		}
		RefreshTextFromLocalization();
	}

	private void OnDisable()
	{
		if (gm != null)
		{
			gm.RefreshLanguageText -= RefreshTextFromLocalization;
		}
	}

	public void RefreshTextFromLocalization()
	{
		textField.text = text;
		if (hasTextAligner)
		{
			textAligner.AlignText();
		}
	}
}

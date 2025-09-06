using TeamCherry.Localization;
using UnityEngine;
using UnityEngine.UI;

public class VibrationLocalisation : MonoBehaviour
{
	[SerializeField]
	[Tooltip("UI Text component to place text.")]
	private Text textField;

	[SerializeField]
	private LocalisedString rumbleLocalisation = new LocalisedString
	{
		Sheet = "MainMenu",
		Key = "GAME_CONTROLLER_RUMBLE"
	};

	[SerializeField]
	private LocalisedString vibrationLocalisation = new LocalisedString
	{
		Sheet = "MainMenu",
		Key = "GAME_CONTROLLER_VIBRATION"
	};

	private GameManager gm;

	private FixVerticalAlign textAligner;

	private bool hasTextAligner;

	private bool hasTextField;

	private void Awake()
	{
		hasTextField = textField != null;
		if (!hasTextField)
		{
			textField = base.gameObject.GetComponent<Text>();
			hasTextField = textField != null;
			if (!hasTextField)
			{
				base.enabled = false;
				return;
			}
		}
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
			gm.RefreshLanguageText += UpdateText;
		}
		UpdateText();
	}

	private void OnDisable()
	{
		if (gm != null)
		{
			gm.RefreshLanguageText -= UpdateText;
		}
	}

	private void UpdateText()
	{
		if (hasTextField)
		{
			LocalisedString localisedString = vibrationLocalisation;
			textField.text = localisedString;
			if (hasTextAligner)
			{
				textAligner.AlignText();
			}
		}
	}
}

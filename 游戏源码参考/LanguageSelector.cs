using System.Collections;
using TeamCherry.Localization;
using UnityEngine;
using UnityEngine.UI;

public sealed class LanguageSelector : MonoBehaviour
{
	[SerializeField]
	private Canvas canvas;

	[SerializeField]
	private GameObject eventSystemObject;

	[SerializeField]
	private CanvasGroup canvasGroup;

	[SerializeField]
	private PreselectOption preselectOption;

	[SerializeField]
	private CanvasGroup languageConfirm;

	[SerializeField]
	private MenuButton submitButton;

	[SerializeField]
	private MenuButton cancelButton;

	private GameObject lastSelection;

	private const float FADE_SPEED = 1.6f;

	private string selectedLanguage;

	private string oldLanguage;

	private bool confirmedLanguage;

	private Coroutine fadeRoutine;

	private bool hasPreselection;

	public CanvasGroup CanvasGroup => canvasGroup;

	public PreselectOption PreselectionOption => preselectOption;

	public CanvasGroup LanguageConfirm => languageConfirm;

	public MenuButton SubmitButton => submitButton;

	public MenuButton CancelButton => cancelButton;

	private void Awake()
	{
		hasPreselection = preselectOption;
	}

	public void SetCamera(Camera camera)
	{
		if ((bool)canvas)
		{
			canvas.worldCamera = camera;
		}
	}

	public IEnumerator DoLanguageSelect()
	{
		yield return ShowLanguageSelect();
		while (!confirmedLanguage)
		{
			yield return null;
		}
		yield return LanguageSettingDone();
	}

	public void SetLastSelection(Selectable selectable)
	{
		if (hasPreselection && (bool)selectable)
		{
			preselectOption.itemToHighlight = selectable;
		}
	}

	public void SetLanguage(LanguageSelectionButton languageButton)
	{
		if ((bool)languageButton)
		{
			SetLastSelection(languageButton);
			SetLanguage(languageButton.Language);
		}
	}

	public void SetLanguage(string newLanguage)
	{
		oldLanguage = Language.CurrentLanguage().ToString();
		selectedLanguage = newLanguage;
		Language.SwitchLanguage(selectedLanguage);
		languageConfirm.gameObject.SetActive(value: true);
		CancelFade();
		fadeRoutine = StartCoroutine(FadeIn(languageConfirm, 0.25f));
		AutoLocalizeTextUI[] componentsInChildren = languageConfirm.GetComponentsInChildren<AutoLocalizeTextUI>();
		for (int i = 0; i < componentsInChildren.Length; i++)
		{
			componentsInChildren[i].RefreshTextFromLocalization();
		}
	}

	private void CancelFade()
	{
		if (fadeRoutine != null)
		{
			StopCoroutine(fadeRoutine);
			fadeRoutine = null;
		}
	}

	private IEnumerator FadeIn(CanvasGroup group, float duration)
	{
		group.alpha = 0f;
		for (float elapsed = 0f; elapsed < duration; elapsed += Time.deltaTime)
		{
			group.alpha = elapsed / duration;
			yield return new WaitForEndOfFrame();
		}
		group.alpha = 1f;
		PreselectOption component = group.GetComponent<PreselectOption>();
		if ((bool)component)
		{
			component.HighlightDefault(deselect: true);
		}
		fadeRoutine = null;
	}

	private IEnumerator FadeOut(CanvasGroup group, float duration)
	{
		group.alpha = 1f;
		for (float elapsed = 0f; elapsed < duration; elapsed += Time.deltaTime)
		{
			group.alpha = 1f - elapsed / duration;
			yield return new WaitForEndOfFrame();
		}
		group.alpha = 0f;
		group.gameObject.SetActive(value: false);
		fadeRoutine = null;
	}

	public void ConfirmLanguage()
	{
		Platform.Current.LocalSharedData.SetInt("GameLangSet", 1);
		Platform.Current.LocalSharedData.Save();
		CancelFade();
		fadeRoutine = StartCoroutine(FadeOut(languageConfirm, 0.25f));
		confirmedLanguage = true;
	}

	public void CancelLanguage()
	{
		Language.SwitchLanguage(oldLanguage);
		CancelFade();
		fadeRoutine = StartCoroutine(FadeOut(languageConfirm, 0.25f));
		if (hasPreselection)
		{
			PreselectionOption.HighlightDefault(deselect: true);
		}
	}

	private IEnumerator ShowLanguageSelect()
	{
		canvasGroup.alpha = 0f;
		canvasGroup.gameObject.SetActive(value: true);
		while ((double)canvasGroup.alpha < 0.99)
		{
			canvasGroup.alpha += Time.smoothDeltaTime * 1.6f;
			if ((double)canvasGroup.alpha > 0.99)
			{
				canvasGroup.alpha = 1f;
			}
			yield return null;
		}
		Cursor.lockState = CursorLockMode.None;
		PreselectionOption.HighlightDefault();
		yield return null;
	}

	private IEnumerator LanguageSettingDone()
	{
		Cursor.lockState = CursorLockMode.Locked;
		while ((double)canvasGroup.alpha > 0.01)
		{
			canvasGroup.alpha -= Time.smoothDeltaTime * 1.6f;
			if ((double)canvasGroup.alpha < 0.01)
			{
				canvasGroup.alpha = 0f;
			}
			yield return null;
		}
		canvasGroup.gameObject.SetActive(value: false);
		ConfigManager.SaveConfig();
	}
}

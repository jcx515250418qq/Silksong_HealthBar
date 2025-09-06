using System;
using TeamCherry.Localization;
using UnityEngine.EventSystems;

namespace UnityEngine.UI
{
	public class MenuOptionHorizontal : MenuSelectable, IMoveHandler, IEventSystemHandler, IPointerClickHandler, ISubmitHandler
	{
		public enum ApplyOnType
		{
			Scroll = 0,
			Submit = 1
		}

		[Header("Option List Settings")]
		public Text optionText;

		public string[] optionList;

		public int selectedOptionIndex;

		public MenuSetting menuSetting;

		[Header("Interaction")]
		public ApplyOnType applySettingOn;

		public CanvasGroup applyButton;

		[Header("Localization")]
		public bool localizeText;

		public string sheetTitle;

		protected GameManager gm;

		private bool hasApplyButton;

		private int currentActiveIndex;

		private new void Awake()
		{
			gm = GameManager.instance;
			hasApplyButton = applyButton != null;
		}

		private new void OnEnable()
		{
			gm.RefreshLanguageText += UpdateText;
			RefreshMenuControls();
			UpdateApplyButton();
		}

		private new void OnDisable()
		{
			gm.RefreshLanguageText -= UpdateText;
		}

		public new void OnMove(AxisEventData move)
		{
			if (base.interactable && !MoveOption(move.moveDir))
			{
				base.OnMove(move);
			}
		}

		public void OnPointerClick(PointerEventData eventData)
		{
			if (base.interactable)
			{
				PointerClickCheckArrows(eventData);
			}
		}

		public void OnSubmit(BaseEventData eventData)
		{
			if (base.interactable)
			{
				if (applySettingOn == ApplyOnType.Submit)
				{
					ApplySettings();
				}
				else
				{
					MoveOption(MoveDirection.Right);
				}
			}
		}

		protected bool MoveOption(MoveDirection dir)
		{
			switch (dir)
			{
			case MoveDirection.Right:
				IncrementOption();
				break;
			case MoveDirection.Left:
				DecrementOption();
				break;
			default:
				return false;
			}
			if ((bool)uiAudioPlayer)
			{
				uiAudioPlayer.PlaySlider();
			}
			return true;
		}

		protected void PointerClickCheckArrows(PointerEventData eventData)
		{
			if ((bool)leftCursor && IsInside(leftCursor.gameObject, eventData))
			{
				MoveOption(MoveDirection.Left);
			}
			else if ((bool)rightCursor && IsInside(rightCursor.gameObject, eventData))
			{
				MoveOption(MoveDirection.Right);
			}
			else
			{
				MoveOption(MoveDirection.Right);
			}
		}

		private bool IsInside(GameObject obj, PointerEventData eventData)
		{
			RectTransform component = obj.GetComponent<RectTransform>();
			if ((bool)component && RectTransformUtility.RectangleContainsScreenPoint(component, eventData.position, Camera.main))
			{
				return true;
			}
			return false;
		}

		public void SetOptionList(string[] optionList)
		{
			this.optionList = optionList;
		}

		public string GetSelectedOptionText()
		{
			if (localizeText)
			{
				return Language.Get(optionList[selectedOptionIndex].ToString(), sheetTitle);
			}
			return optionList[selectedOptionIndex].ToString();
		}

		public string GetSelectedOptionTextRaw()
		{
			return optionList[selectedOptionIndex].ToString();
		}

		public virtual void SetOptionTo(int optionNumber)
		{
			if (optionNumber >= 0 && optionNumber < optionList.Length)
			{
				selectedOptionIndex = optionNumber;
				UpdateText();
				return;
			}
			Debug.LogErrorFormat("{0} - Trying to select an option outside the list size (index: {1} listsize: {2})", base.name, optionNumber, optionList.Length);
		}

		protected virtual void UpdateText()
		{
			if (optionList == null || !(optionText != null))
			{
				return;
			}
			try
			{
				if (localizeText)
				{
					optionText.text = Language.Get(optionList[selectedOptionIndex].ToString(), sheetTitle);
				}
				else
				{
					optionText.text = optionList[selectedOptionIndex].ToString();
				}
			}
			catch (Exception ex)
			{
				Debug.LogError(optionText.text + " : " + optionList?.ToString() + " : " + selectedOptionIndex + " " + ex);
			}
			FixVerticalAlign component = optionText.GetComponent<FixVerticalAlign>();
			if ((bool)component)
			{
				component.AlignText();
			}
		}

		protected void UpdateSetting()
		{
			if ((bool)menuSetting)
			{
				menuSetting.ChangeSetting(selectedOptionIndex);
			}
		}

		protected void DecrementOption()
		{
			if (selectedOptionIndex > 0)
			{
				selectedOptionIndex--;
				if (applySettingOn == ApplyOnType.Scroll)
				{
					UpdateSetting();
				}
				UpdateText();
			}
			else if (selectedOptionIndex == 0)
			{
				selectedOptionIndex = optionList.Length - 1;
				if (applySettingOn == ApplyOnType.Scroll)
				{
					UpdateSetting();
				}
				UpdateText();
			}
			UpdateApplyButton();
		}

		protected void IncrementOption()
		{
			if (selectedOptionIndex >= 0 && selectedOptionIndex < optionList.Length - 1)
			{
				selectedOptionIndex++;
				if (applySettingOn == ApplyOnType.Scroll)
				{
					UpdateSetting();
				}
				UpdateText();
			}
			else if (selectedOptionIndex == optionList.Length - 1)
			{
				selectedOptionIndex = 0;
				if (applySettingOn == ApplyOnType.Scroll)
				{
					UpdateSetting();
				}
				UpdateText();
			}
			UpdateApplyButton();
		}

		public void RefreshMenuControls()
		{
			RefreshCurrentIndex();
			UpdateText();
		}

		public virtual void ApplySettings()
		{
			if (selectedOptionIndex >= 0)
			{
				UpdateSetting();
				RefreshCurrentIndex();
				HideApplyButton();
			}
		}

		public virtual void UpdateApplyButton()
		{
			if (currentActiveIndex == selectedOptionIndex)
			{
				HideApplyButton();
			}
			else
			{
				ShowApplyButton();
			}
		}

		public virtual void RefreshCurrentIndex()
		{
			if (menuSetting != null)
			{
				menuSetting.RefreshValueFromGameSettings();
			}
			currentActiveIndex = selectedOptionIndex;
		}

		protected void HideApplyButton()
		{
			if (hasApplyButton)
			{
				applyButton.alpha = 0f;
				applyButton.interactable = false;
				applyButton.blocksRaycasts = false;
			}
		}

		protected void ShowApplyButton()
		{
			if (applySettingOn != 0 && hasApplyButton)
			{
				applyButton.alpha = 1f;
				applyButton.interactable = true;
				applyButton.blocksRaycasts = true;
			}
		}
	}
}

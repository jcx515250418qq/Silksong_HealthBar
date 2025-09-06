using System.Collections;
using System.Collections.Generic;
using UnityEngine.EventSystems;

namespace UnityEngine.UI
{
	public sealed class MenuButtonSaveImport : MenuButton, ISubmitHandler, IEventSystemHandler, IPointerClickHandler
	{
		[Header("Save Import Button")]
		[SerializeField]
		private Text textField;

		[SerializeField]
		private CanvasGroup parentGroup;

		[SerializeField]
		private CanvasGroup importPromptGroup;

		[SerializeField]
		private PreselectOption importPreselectOption;

		[SerializeField]
		private Transform menuParent;

		private bool detachedImportPrompt;

		private GameManager gm;

		private UIManager uiManager;

		private FixVerticalAlign textAligner;

		private bool hasTextAligner;

		private bool showingConfirm;

		private bool isCancelling;

		private Coroutine parentGroupCoroutine;

		private Coroutine importGroupCoroutine;

		private Coroutine currentRoutine;

		private string ButtonLabel
		{
			get
			{
				if ((bool)Platform.Current)
				{
					return Platform.Current.SaveImportLabel;
				}
				return string.Empty;
			}
		}

		protected override void Awake()
		{
			base.Awake();
			textAligner = GetComponent<FixVerticalAlign>();
			if ((bool)textAligner)
			{
				hasTextAligner = true;
			}
		}

		protected override void OnEnable()
		{
			base.OnEnable();
			gm = GameManager.instance;
			if ((bool)gm)
			{
				gm.RefreshLanguageText += RefreshTextFromLocalization;
			}
			uiManager = UIManager.instance;
			RefreshTextFromLocalization();
		}

		protected override void OnDisable()
		{
			base.OnDisable();
			if (gm != null)
			{
				gm.RefreshLanguageText -= RefreshTextFromLocalization;
			}
			showingConfirm = false;
			isCancelling = false;
		}

		protected override void OnDestroy()
		{
			base.OnDestroy();
			if (detachedImportPrompt && (bool)importPromptGroup)
			{
				Object.Destroy(importPromptGroup.gameObject);
			}
		}

		public void RefreshTextFromLocalization()
		{
			textField.text = ButtonLabel;
			if (hasTextAligner)
			{
				textAligner.AlignText();
			}
		}

		public new void OnSubmit(BaseEventData eventData)
		{
			if (base.interactable)
			{
				base.OnSubmit(eventData);
				ShowConfirmPrompt();
			}
		}

		public new void OnPointerClick(PointerEventData eventData)
		{
			if (base.interactable)
			{
				base.OnPointerClick(eventData);
				ShowConfirmPrompt();
			}
		}

		public void ShowConfirmPrompt()
		{
			if (!showingConfirm)
			{
				showingConfirm = true;
				if (currentRoutine != null)
				{
					uiManager.StopCoroutine(currentRoutine);
				}
				currentRoutine = uiManager.StartCoroutine(ShowConfirmRoutine());
			}
		}

		private IEnumerator ShowConfirmRoutine()
		{
			if (importPromptGroup == null || importPreselectOption == null || menuParent == null)
			{
				ConfirmPrompt();
				yield break;
			}
			if (parentGroup != null)
			{
				parentGroup.interactable = false;
				if (parentGroupCoroutine != null)
				{
					uiManager.StopCoroutine(parentGroupCoroutine);
				}
				parentGroupCoroutine = uiManager.StartCoroutine(uiManager.FadeOutCanvasGroup(parentGroup));
			}
			if (!detachedImportPrompt)
			{
				importPromptGroup.transform.SetParent(menuParent.parent);
			}
			if (importGroupCoroutine != null)
			{
				uiManager.StopCoroutine(importGroupCoroutine);
			}
			importGroupCoroutine = null;
			yield return uiManager.FadeInCanvasGroup(importPromptGroup);
			importPromptGroup.interactable = true;
			importPreselectOption.HighlightDefault();
		}

		public void ConfirmPrompt()
		{
			if (currentRoutine != null)
			{
				uiManager.StopCoroutine(currentRoutine);
			}
			currentRoutine = uiManager.StartCoroutine(ConfirmPromptRoutine());
		}

		private IEnumerator ConfirmPromptRoutine()
		{
			DoImport();
			if ((bool)importPromptGroup)
			{
				importPromptGroup.interactable = false;
				if (importGroupCoroutine != null)
				{
					uiManager.StopCoroutine(importGroupCoroutine);
				}
				importGroupCoroutine = uiManager.StartCoroutine(uiManager.FadeOutCanvasGroup(importPromptGroup));
			}
			showingConfirm = false;
			if (parentGroup != null)
			{
				if (parentGroupCoroutine != null)
				{
					uiManager.StopCoroutine(parentGroupCoroutine);
				}
				parentGroupCoroutine = null;
				yield return uiManager.FadeInCanvasGroup(parentGroup);
				parentGroup.interactable = true;
			}
			SelectGameObject();
		}

		public void CancelConfirmPrompt()
		{
			if (!isCancelling)
			{
				isCancelling = true;
				if (currentRoutine != null)
				{
					uiManager.StopCoroutine(currentRoutine);
				}
				currentRoutine = uiManager.StartCoroutine(CancelConfirmRoutine());
			}
		}

		private IEnumerator CancelConfirmRoutine()
		{
			if ((bool)importPromptGroup)
			{
				importPromptGroup.interactable = false;
				if (importGroupCoroutine != null)
				{
					uiManager.StopCoroutine(importGroupCoroutine);
				}
				importGroupCoroutine = uiManager.StartCoroutine(uiManager.FadeOutCanvasGroup(importPromptGroup));
			}
			if (parentGroup != null)
			{
				if (parentGroupCoroutine != null)
				{
					uiManager.StopCoroutine(parentGroupCoroutine);
				}
				parentGroupCoroutine = null;
				yield return uiManager.FadeInCanvasGroup(parentGroup);
				parentGroup.interactable = true;
			}
			showingConfirm = false;
			isCancelling = false;
			SelectGameObject();
		}

		private void SelectGameObject()
		{
			EventSystem.current.SetSelectedGameObject(base.gameObject);
		}

		private void DoImport()
		{
			if (!Platform.Current.ShowSaveDataImport)
			{
				Debug.LogError($"{Platform.Current} does not support save data import.");
				showingConfirm = false;
				return;
			}
			DisplayImportInProgress();
			Platform.Current.FetchImportData(delegate(List<Platform.ImportDataInfo> importDataInfos)
			{
				if (importDataInfos == null)
				{
					HideImportInProgress();
					DisplayNoImportDataPrompt();
				}
				else
				{
					GameManager.instance.StartCoroutine(ProcessImportData(importDataInfos));
				}
			});
		}

		private IEnumerator ProcessImportData(List<Platform.ImportDataInfo> importDataInfos)
		{
			int currentSlot = 1;
			int importedCount = 0;
			int importTotal = importDataInfos.Count;
			for (int i = 0; i < importDataInfos.Count; i++)
			{
				bool wait = true;
				bool isUsed = true;
				Platform.ImportDataInfo importDataInfo = importDataInfos[i];
				if (!importDataInfo.isSaveSlot)
				{
					importDataInfo.Save(0, delegate
					{
						wait = false;
					});
					yield return new WaitWhile(() => wait);
					importTotal--;
					continue;
				}
				for (; currentSlot <= 4; currentSlot++)
				{
					wait = true;
					Platform.Current.IsSaveSlotInUse(currentSlot, delegate(bool inUse)
					{
						isUsed = inUse;
						wait = false;
					});
					yield return new WaitWhile(() => wait);
					if (!isUsed)
					{
						break;
					}
				}
				if (currentSlot <= 4)
				{
					wait = true;
					importDataInfo.Save(currentSlot, delegate
					{
						wait = false;
					});
					yield return new WaitWhile(() => wait);
					currentSlot++;
					importedCount++;
				}
			}
			bool wait2 = true;
			bool success2 = importedCount == importTotal;
			Platform.Current.DisplayImportDataResultMessage(new Platform.ImportDataResult
			{
				importedCount = importedCount,
				importTotal = importTotal,
				success = success2
			}, delegate
			{
				wait2 = false;
			});
			yield return new WaitWhile(() => wait2);
			uiManager.ReloadSaves();
			HideImportInProgress();
		}

		private void DisplayImportInProgress()
		{
		}

		private void HideImportInProgress()
		{
			Platform.Current.CloseSystemDialogs();
		}

		private void DisplayNoImportDataPrompt()
		{
		}
	}
}

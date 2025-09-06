using System.Collections;
using System.Collections.Generic;
using UnityEngine.EventSystems;
using UnityEngine.Serialization;

namespace UnityEngine.UI
{
	public sealed class RestoreSaveButton : MenuButton, ISubmitHandler, IEventSystemHandler, IPointerClickHandler, ISelectHandler
	{
		private enum RestoreState
		{
			None = 0,
			Button = 1,
			Selection = 2,
			Prompt = 3,
			Confirm = 4
		}

		[Header("Restore Save Button")]
		[SerializeField]
		private SaveSlotButton saveSlotButton;

		[SerializeField]
		private Animator selectIcon;

		[Space]
		[SerializeField]
		private CanvasGroup restoreSaveButton;

		[FormerlySerializedAs("selectionDisplay")]
		[SerializeField]
		private RestoreSavePointDisplay restoreSaveSelection;

		[SerializeField]
		private CanvasGroup restoreSavePrompt;

		[SerializeField]
		private CanvasGroup restoreSaveConfirmPrompt;

		[Header("Debug")]
		[SerializeField]
		private bool fakeLoadingState;

		[SerializeField]
		private bool fakeIncompatibleFiles;

		[SerializeField]
		private bool fakeEmptyList;

		private static readonly int _isSelectedProp = Animator.StringToHash("Is Selected");

		private UIManager ui;

		private InputHandler ih;

		private FetchDataRequest<RestorePointData> fetchRequest;

		private FetchDataRequest<SaveGameData> versionDataRequest;

		private bool isVisible;

		private RestoreState state;

		private PreselectOption restoreSavePromptHighlight;

		private PreselectOption restoreSaveConfirmPromptHighlight;

		private RestorePointData selectedRestorePoint;

		private const float SHOW_LOADING_DELAY = 0f;

		private const float LOADING_TIMEOUT = 10f;

		private Selectable previousBackSelectable;

		private bool isLoading;

		private Coroutine currentTransition;

		private static RestoreSaveButton activeRestoreSaveButton;

		public CanvasGroup CanvasGroup => restoreSaveButton;

		public void RestoreSavePromptYes()
		{
			InternalEnterState(RestoreState.Confirm);
		}

		public void CancelSelection()
		{
			saveSlotButton.ShowRelevantModeForSaveFileState();
		}

		public void RestoreSavePromptNo()
		{
			saveSlotButton.ShowRelevantModeForSaveFileState();
		}

		public void RestoreSaveConfirmYes()
		{
			saveSlotButton.StartCoroutine(RestoreConfirmed());
		}

		public void RestoreSaveConfirmNo()
		{
			saveSlotButton.ShowRelevantModeForSaveFileState();
		}

		protected override void Start()
		{
			base.Start();
			HookUpAudioPlayer();
			HookUpEventTrigger();
			ui = UIManager.instance;
			ih = GameManager.instance.inputHandler;
			restoreSavePromptHighlight = restoreSavePrompt.GetComponent<PreselectOption>();
			restoreSaveConfirmPromptHighlight = restoreSaveConfirmPrompt.GetComponent<PreselectOption>();
			restoreSaveSelection.Init();
			Navigation navigation = base.navigation;
			if (navigation.selectOnDown == null)
			{
				navigation.selectOnDown = saveSlotButton.backButton;
			}
			base.navigation = navigation;
			if (!isVisible)
			{
				SetHidden();
			}
		}

		protected override void OnDisable()
		{
			base.OnDisable();
			if (isVisible)
			{
				saveSlotButton.interactable = true;
				saveSlotButton.myCanvasGroup.interactable = true;
				saveSlotButton.myCanvasGroup.blocksRaycasts = true;
				saveSlotButton.controlsGroupBlocker.ignoreParentGroups = false;
			}
			isVisible = false;
		}

		public new void OnSubmit(BaseEventData eventData)
		{
			if (base.interactable)
			{
				base.OnSubmit(eventData);
				ForceDeselect();
				saveSlotButton.RestoreSaveMenu();
			}
		}

		public new void OnPointerClick(PointerEventData eventData)
		{
			OnSubmit(eventData);
		}

		public new void OnSelect(BaseEventData eventData)
		{
			base.OnSelect(eventData);
			if ((bool)restoreSaveButton && restoreSaveButton.interactable)
			{
				if ((bool)selectIcon)
				{
					selectIcon.SetBool(_isSelectedProp, value: true);
				}
			}
			else
			{
				StartCoroutine(SelectAfterFrame(base.navigation.selectOnUp.gameObject));
			}
		}

		protected override void OnDeselected(BaseEventData eventData)
		{
			if ((bool)selectIcon)
			{
				selectIcon.SetBool(_isSelectedProp, value: false);
			}
		}

		private IEnumerator SelectAfterFrame(GameObject obj)
		{
			yield return new WaitForEndOfFrame();
			EventSystem.current.SetSelectedGameObject(obj);
		}

		public Selectable GetBackButton()
		{
			return saveSlotButton.backButton;
		}

		private void SetHidden()
		{
			SetGroupHidden(restoreSaveButton);
			SetGroupHidden(restoreSavePrompt);
			SetGroupHidden(restoreSaveConfirmPrompt);
			restoreSaveSelection.SetHidden();
		}

		private static void SetGroupHidden(CanvasGroup canvasGroup)
		{
			canvasGroup.alpha = 0f;
			canvasGroup.interactable = false;
			canvasGroup.blocksRaycasts = false;
		}

		public IEnumerator ShowRestoreSaveButton()
		{
			base.gameObject.SetActive(value: true);
			yield return EnterState(RestoreState.Button);
		}

		public IEnumerator ShowSaveSelection()
		{
			base.gameObject.SetActive(value: true);
			yield return EnterState(RestoreState.Selection);
		}

		private IEnumerator EnterState(RestoreState restoreState)
		{
			yield return ExitState();
			if (currentTransition != null)
			{
				UIManager.instance.StopCoroutine(currentTransition);
			}
			currentTransition = UIManager.instance.StartCoroutine(TransitionToState(restoreState));
			yield return currentTransition;
			currentTransition = null;
		}

		public IEnumerator Hide()
		{
			if (isVisible)
			{
				if (currentTransition != null)
				{
					UIManager.instance.StopCoroutine(currentTransition);
					currentTransition = null;
				}
				if (isLoading)
				{
					isLoading = false;
					UIManager.instance.StartCoroutine(saveSlotButton.HideLoadingPrompt());
				}
				isVisible = false;
				yield return ExitState();
			}
		}

		private IEnumerator TransitionToState(RestoreState newState)
		{
			if (isVisible && newState == state)
			{
				yield break;
			}
			isVisible = true;
			state = newState;
			switch (newState)
			{
			case RestoreState.Button:
				saveSlotButton.StartCoroutine(ui.FadeInCanvasGroup(restoreSaveButton));
				restoreSaveButton.blocksRaycasts = true;
				break;
			case RestoreState.Selection:
			{
				if (fetchRequest == null)
				{
					LoadRestorePoints(saveSlotButton.SaveSlotIndex, isIncompatible: false);
					yield return null;
					if (fetchRequest == null)
					{
						HandleNoRequestError();
					}
				}
				bool isSaveIncompatible = saveSlotButton.IsSaveIncompatible();
				if (isSaveIncompatible && versionDataRequest == null)
				{
					LoadVersionRestoresPoints(saveSlotButton.SaveSlotIndex);
				}
				if (fetchRequest != null)
				{
					switch (fetchRequest.State)
					{
					case FetchDataRequest.Status.InProgress:
					{
						restoreSaveSelection.ToggleLoadIcon(show: true);
						isLoading = true;
						float timer = 0f;
						while (timer < 10f)
						{
							yield return null;
							timer += Time.deltaTime;
							if (fetchRequest.State == FetchDataRequest.Status.Completed)
							{
								timer = 0f;
								break;
							}
						}
						isLoading = false;
						if (timer >= 10f)
						{
							HandleTimeoutError();
						}
						break;
					}
					default:
						HandleUnexpectedStateError();
						break;
					case FetchDataRequest.Status.Completed:
						break;
					}
					List<RestorePointData> restorePoints = new List<RestorePointData>();
					if (isSaveIncompatible && versionDataRequest != null)
					{
						Debug.Log($"Processing Version Backups. : Slot {saveSlotButton.SaveSlotIndex}", this);
						if (versionDataRequest.State != FetchDataRequest.Status.Completed)
						{
							restoreSaveSelection.ToggleLoadIcon(show: true);
							isLoading = true;
							float timer = 0f;
							while (timer < 10f)
							{
								yield return null;
								timer += Time.deltaTime;
								if (versionDataRequest.State == FetchDataRequest.Status.Completed)
								{
									timer = 0f;
									break;
								}
							}
							isLoading = false;
							if (timer >= 10f)
							{
								HandleTimeoutError();
							}
						}
						if (versionDataRequest.State == FetchDataRequest.Status.Completed && versionDataRequest.results != null)
						{
							restoreSaveSelection.ToggleLoadIcon(show: true);
							Debug.Log($"Found {versionDataRequest.results.Count} version backups. : Slot {saveSlotButton.SaveSlotIndex}", this);
							for (int i = 0; i < versionDataRequest.results.Count; i++)
							{
								FetchDataRequest<SaveGameData>.FetchResult fetchResult = versionDataRequest.results[i];
								if (fetchResult.loadedObject != null && !SaveDataUtility.IsVersionIncompatible("1.0.28324", fetchResult.loadedObject.playerData.version, fetchResult.loadedObject.playerData.RevisionBreak))
								{
									restorePoints.Add(RestorePointData.CreateVersionBackup(fetchResult.loadedObject));
								}
							}
						}
					}
					if (fetchRequest.results != null)
					{
						restoreSaveSelection.ToggleLoadIcon(show: true);
						Debug.Log($"Found {fetchRequest.results.Count} restore points. : Slot {saveSlotButton.SaveSlotIndex}", this);
						for (int j = 0; j < fetchRequest.results.Count; j++)
						{
							FetchDataRequest<RestorePointData>.FetchResult fetchResult2 = fetchRequest.results[j];
							if (fetchResult2.loadedObject.saveGameData != null && !SaveDataUtility.IsVersionIncompatible("1.0.28324", fetchResult2.loadedObject.saveGameData.playerData.version, fetchResult2.loadedObject.saveGameData.playerData.RevisionBreak))
							{
								restorePoints.Add(fetchResult2.loadedObject);
							}
						}
					}
					restoreSaveSelection.SetRestorePoints(restorePoints);
				}
				else
				{
					Debug.Log($"Failed to create fetch request for slot {saveSlotButton.SaveSlotIndex}", this);
					restoreSaveSelection.ClearRestorePoints();
				}
				saveSlotButton.parentBlocker.blocksRaycasts = false;
				yield return restoreSaveSelection.Show();
				ih.StartUIInput();
				activeRestoreSaveButton = this;
				saveSlotButton.controlsGroupBlocker.ignoreParentGroups = true;
				break;
			}
			case RestoreState.Prompt:
				yield return ui.FadeInCanvasGroup(restoreSavePrompt);
				restoreSavePrompt.blocksRaycasts = true;
				restoreSavePromptHighlight.HighlightDefault();
				ih.StartUIInput();
				saveSlotButton.controlsGroupBlocker.ignoreParentGroups = true;
				break;
			case RestoreState.Confirm:
				yield return ui.FadeInCanvasGroup(restoreSaveConfirmPrompt);
				restoreSaveConfirmPrompt.blocksRaycasts = true;
				restoreSaveConfirmPromptHighlight.HighlightDefault();
				ih.StartUIInput();
				saveSlotButton.controlsGroupBlocker.ignoreParentGroups = true;
				break;
			}
		}

		private IEnumerator ExitState()
		{
			isVisible = false;
			saveSlotButton.controlsGroupBlocker.ignoreParentGroups = false;
			switch (state)
			{
			case RestoreState.Button:
				yield return ui.FadeOutCanvasGroup(restoreSaveButton, disable: false);
				restoreSaveButton.interactable = false;
				restoreSaveButton.blocksRaycasts = false;
				break;
			case RestoreState.Selection:
				ih.StopUIInput();
				if ((bool)activeRestoreSaveButton)
				{
					activeRestoreSaveButton = null;
				}
				if (isLoading)
				{
					isLoading = false;
					UIManager.instance.StartCoroutine(saveSlotButton.HideLoadingPrompt());
				}
				yield return restoreSaveSelection.Hide();
				break;
			case RestoreState.Prompt:
				ih.StopUIInput();
				yield return ui.FadeOutCanvasGroup(restoreSavePrompt);
				restoreSavePrompt.interactable = false;
				restoreSavePrompt.blocksRaycasts = false;
				break;
			case RestoreState.Confirm:
				ih.StopUIInput();
				yield return ui.FadeOutCanvasGroup(restoreSaveConfirmPrompt);
				restoreSaveConfirmPrompt.interactable = false;
				restoreSaveConfirmPrompt.blocksRaycasts = false;
				break;
			default:
				Debug.LogError($"Exiting unsupported state {state}", this);
				break;
			case RestoreState.None:
				break;
			}
		}

		public void SaveSelected(RestorePointData restorePointData)
		{
			if (state != RestoreState.Selection)
			{
				Debug.LogWarning("Restore point was selected while in an unexpected state", this);
			}
			if (restorePointData == null)
			{
				Debug.LogError("Restore Point Data is null", this);
				saveSlotButton.ShowRelevantModeForSaveFileState();
			}
			else
			{
				selectedRestorePoint = restorePointData;
				InternalEnterState(RestoreState.Prompt);
			}
		}

		private void InternalEnterState(RestoreState restoreState)
		{
			saveSlotButton.StartCoroutine(EnterState(restoreState));
		}

		private IEnumerator RestoreConfirmed()
		{
			yield return Hide();
			saveSlotButton.OverrideSaveData(selectedRestorePoint);
			selectedRestorePoint = null;
		}

		private void HandleNoRequestError()
		{
			Debug.LogError($"Failed fetch restore points Slot {saveSlotButton.SaveSlotIndex}", this);
		}

		private void HandleTimeoutError()
		{
			Debug.LogError($"Timed out while trying to fetch restore points {saveSlotButton.SaveSlotIndex}", this);
		}

		private void HandleUnexpectedStateError()
		{
			Debug.LogError($"Encountered unexpected state while trying to restore save {saveSlotButton.SaveSlotIndex}", this);
		}

		public void ResetButton()
		{
			fetchRequest = null;
			versionDataRequest = null;
		}

		public void PreloadRestorePoints(int slot, bool isIncompatible)
		{
		}

		private void LoadRestorePoints(int slot, bool isIncompatible)
		{
			fetchRequest = new FetchDataRequest<RestorePointData>(Platform.Current.FetchRestorePoints(slot));
			if (isIncompatible)
			{
				LoadVersionRestoresPoints(slot);
			}
		}

		private void LoadVersionRestoresPoints(int slot)
		{
			versionDataRequest = new FetchDataRequest<SaveGameData>(Platform.Current.FetchVersionRestorePoints(slot));
		}

		public static bool GoBack()
		{
			if ((bool)activeRestoreSaveButton && activeRestoreSaveButton.state == RestoreState.Selection)
			{
				activeRestoreSaveButton.CancelSelection();
				return true;
			}
			return false;
		}
	}
}

using System;
using System.Collections;
using System.Globalization;
using GlobalEnums;
using TeamCherry.SharedUtils;
using UnityEngine.EventSystems;
using UnityEngine.Events;

namespace UnityEngine.UI
{
	[Serializable]
	public class SaveSlotButton : MenuButton, ISelectHandler, IEventSystemHandler, IDeselectHandler, ISubmitHandler, IPointerClickHandler
	{
		public enum SaveFileStates
		{
			NotStarted = 0,
			OperationInProgress = 1,
			Empty = 2,
			LoadedStats = 3,
			Corrupted = 4,
			Incompatible = 5
		}

		public enum SaveSlot
		{
			Slot1 = 0,
			Slot2 = 1,
			Slot3 = 2,
			Slot4 = 3
		}

		public enum SlotState
		{
			Hidden = 0,
			OperationInProgress = 1,
			EmptySlot = 2,
			SavePresent = 3,
			Defeated = 4,
			Corrupted = 5,
			Incompatible = 6,
			ClearPrompt = 7,
			ClearConfirm = 8,
			BlackThreadInfected = 9,
			RestoreSave = 10
		}

		private class PreloadOperation
		{
			public enum PreloadState
			{
				NotStarted = 0,
				Loading = 1,
				Complete = 2
			}

			public readonly int SaveSlot;

			private PreloadState state;

			private object stateLock = new object();

			private GameManager gm;

			private Action<PreloadState> callback;

			private bool killed;

			public PreloadState State
			{
				get
				{
					lock (stateLock)
					{
						return state;
					}
				}
				private set
				{
					lock (stateLock)
					{
						state = value;
					}
				}
			}

			public bool IsEmpty { get; private set; } = true;

			public SaveStats SaveStats { get; private set; }

			public string Message { get; private set; }

			public PreloadOperation(int saveSlot, GameManager gm)
			{
				SaveSlot = saveSlot;
				this.gm = gm;
				GetSaveStatsForSlot();
			}

			private void GetSaveStatsForSlot()
			{
				if (State != 0)
				{
					return;
				}
				State = PreloadState.Loading;
				gm.HasSaveFile(SaveSlot, delegate(bool inUse)
				{
					if (!killed)
					{
						if (!inUse)
						{
							IsEmpty = true;
							SetComplete();
						}
						else
						{
							IsEmpty = false;
							gm.GetSaveStatsForSlot(SaveSlot, delegate(SaveStats stats, string message)
							{
								if (!killed)
								{
									Message = message;
									if (stats != null)
									{
										SaveStats = stats;
									}
									SetComplete();
								}
							});
						}
					}
				});
			}

			public void WaitForComplete(Action<PreloadState> onComplete)
			{
				if (killed)
				{
					onComplete?.Invoke(PreloadState.Complete);
					return;
				}
				lock (stateLock)
				{
					if (state != PreloadState.Complete)
					{
						callback = (Action<PreloadState>)Delegate.Combine(callback, onComplete);
						return;
					}
				}
				onComplete?.Invoke(State);
			}

			private void SetComplete()
			{
				if (killed)
				{
					return;
				}
				lock (stateLock)
				{
					state = PreloadState.Complete;
				}
				if (callback != null)
				{
					CoreLoop.InvokeSafe(delegate
					{
						callback(PreloadState.Complete);
					});
				}
			}

			public void Kill()
			{
				killed = true;
			}
		}

		[Header("SaveSlotButton")]
		public Selectable backButton;

		public SaveSlot saveSlot;

		[SerializeField]
		private RestoreSaveButton restoreSaveButton;

		[Header("Animation")]
		public Animator[] frameFluers;

		public Animator highlight;

		[SerializeField]
		private Image highlightImage;

		[SerializeField]
		private Material highlightMatEmpty;

		[SerializeField]
		private Material highlightMatFull;

		[SerializeField]
		private BlackThreadStrandGroup blackThreadInfectedGroup;

		[SerializeField]
		private Transform blackThreadsParent;

		public UnityEvent OnBlackThreadImpact;

		public UnityEvent OnBlackThreadBurst;

		public UnityEvent BlackThreadOnDisable;

		[Header("Canvas Groups")]
		public CanvasGroup newGameText;

		public CanvasGroup saveCorruptedText;

		public CanvasGroup saveIncompatibleText;

		public CanvasGroup loadingText;

		public CanvasGroup activeSaveSlot;

		public CanvasGroup clearSaveButton;

		public CanvasGroup clearSavePrompt;

		public CanvasGroup clearSaveConfirmPrompt;

		public CanvasGroup backgroundCg;

		public CanvasGroup slotNumberText;

		public CanvasGroup myCanvasGroup;

		public CanvasGroup defeatedText;

		public CanvasGroup defeatedBackground;

		public CanvasGroup brokenSteelOrb;

		public CanvasGroup parentBlocker;

		public CanvasGroup controlsGroupBlocker;

		[Header("Text Elements")]
		public Text locationText;

		public Text playTimeText;

		public Text completionText;

		[Space]
		public GameObject rosaryGroup;

		public Text rosaryText;

		public GameObject shardGroup;

		public Text shardText;

		[Header("Visual Elements")]
		public Image background;

		public GameObject blackThreadOverlay;

		public SaveProfileHealthBar healthSlots;

		public SaveProfileSilkBar silkBar;

		public SaveSlotBackgrounds saveSlots;

		private GameManager gm;

		private UIManager ui;

		private InputHandler ih;

		public SaveFileStates saveFileState;

		private PreselectOption clearSavePromptHighlight;

		private PreselectOption clearSaveConfirmPromptHighlight;

		[SerializeField]
		private SaveStats saveStats;

		[SerializeField]
		private SaveSlotCompletionIcons saveSlotCompletionIcons;

		[Header("Debug")]
		[SerializeField]
		private bool fakeCorruptedState;

		private BlackThreadStrand[] blackThreads;

		private const int BLACK_THREAD_IMPACTS = 5;

		private int blackThreadImpactsLeft;

		private const float BLACK_THREAD_IMPACT_COOLDOWN = 0.5f;

		private int strandsRemovedPerHit;

		private double nextImpactTime;

		private Navigation noNav;

		private Navigation fullSlotNav;

		private Navigation emptySlotNav;

		private Navigation defeatedSlotNav;

		private float clearSaveButtonX;

		private IEnumerator currentLoadingTextFadeIn;

		private CoroutineQueue coroutineQueue;

		private PreloadOperation preloadOperation;

		private static readonly int _showAnimProp = Animator.StringToHash("show");

		private static readonly int _hideAnimProp = Animator.StringToHash("hide");

		private bool isRestoringSave;

		public SlotState State { get; private set; }

		public bool IsBlackThreaded
		{
			get
			{
				if (State == SlotState.BlackThreadInfected)
				{
					return blackThreadImpactsLeft > 0;
				}
				return false;
			}
		}

		public bool HasPreloaded => preloadOperation != null;

		public int SaveSlotIndex => saveSlot switch
		{
			SaveSlot.Slot1 => 1, 
			SaveSlot.Slot2 => 2, 
			SaveSlot.Slot3 => 3, 
			SaveSlot.Slot4 => 4, 
			_ => 0, 
		};

		private new void Awake()
		{
			if (Application.isPlaying)
			{
				gm = GameManager.instance;
				ui = UIManager.instance;
				ih = gm.inputHandler;
				blackThreads = blackThreadsParent.GetComponentsInChildren<BlackThreadStrand>();
				clearSavePromptHighlight = clearSavePrompt.GetComponent<PreselectOption>();
				clearSaveConfirmPromptHighlight = clearSaveConfirmPrompt.GetComponent<PreselectOption>();
				coroutineQueue = new CoroutineQueue(this);
				SetupNavs();
				strandsRemovedPerHit = blackThreadInfectedGroup.TotalStrands / 5;
				clearSaveButtonX = clearSaveButton.transform.localPosition.x;
			}
		}

		private new void OnEnable()
		{
			if (saveStats != null && saveFileState == SaveFileStates.LoadedStats)
			{
				PresentSaveSlot(saveStats);
			}
		}

		protected override void OnDisable()
		{
			base.OnDisable();
			if (coroutineQueue != null)
			{
				coroutineQueue.Clear();
				StopAllCoroutines();
			}
			if (blackThreadImpactsLeft > 0)
			{
				BlackThreadOnDisable.Invoke();
			}
		}

		private new void Start()
		{
			if (Application.isPlaying)
			{
				HookUpAudioPlayer();
			}
		}

		public void ResetButton(GameManager gameManager, bool doAnimate = true, bool fetchRestorePoints = true)
		{
			ResetFluerTriggersReversed();
			UIManager.FadeOutCanvasGroupInstant(slotNumberText);
			UIManager.FadeOutCanvasGroupInstant(backgroundCg);
			backgroundCg.gameObject.SetActive(value: true);
			UIManager.FadeOutCanvasGroupInstant(activeSaveSlot);
			ui.StartCoroutine(restoreSaveButton.Hide());
			UIManager.FadeOutCanvasGroupInstant(clearSaveButton, disable: true, stopBlocking: true);
			UIManager.FadeOutCanvasGroupInstant(saveCorruptedText);
			UIManager.FadeOutCanvasGroupInstant(saveIncompatibleText);
			UIManager.FadeOutCanvasGroupInstant(loadingText);
			base.interactable = true;
			myCanvasGroup.blocksRaycasts = true;
			UIManager.FadeOutCanvasGroupInstant(defeatedBackground);
			UIManager.FadeOutCanvasGroupInstant(defeatedText);
			UIManager.FadeOutCanvasGroupInstant(brokenSteelOrb);
			UIManager.FadeOutCanvasGroupInstant(newGameText);
			UIManager.FadeOutCanvasGroupInstant(clearSavePrompt, disable: true, stopBlocking: true);
			UIManager.FadeOutCanvasGroupInstant(clearSaveConfirmPrompt, disable: true, stopBlocking: true);
			restoreSaveButton.ResetButton();
			saveFileState = SaveFileStates.NotStarted;
			State = SlotState.Hidden;
			if (doAnimate)
			{
				preloadOperation = null;
				Prepare(gameManager, isReload: true, doAnimate: true, fetchRestorePoints);
			}
			else
			{
				preloadOperation = null;
				PreloadSave(gameManager);
			}
		}

		public void ForcePreloadSave(GameManager gameManager)
		{
			preloadOperation = new PreloadOperation(SaveSlotIndex, gameManager);
		}

		public void PreloadSave(GameManager gameManager)
		{
			if (preloadOperation == null)
			{
				preloadOperation = new PreloadOperation(SaveSlotIndex, gameManager);
			}
		}

		public void Prepare(GameManager gameManager, bool isReload = false, bool doAnimate = true, bool fetchRestorePoints = false)
		{
			if (!isReload && saveFileState != 0 && saveFileState != SaveFileStates.Corrupted)
			{
				return;
			}
			ChangeSaveFileState(SaveFileStates.OperationInProgress, doAnimate);
			if (isReload)
			{
				preloadOperation = null;
			}
			else if (preloadOperation != null)
			{
				if (!doAnimate)
				{
					return;
				}
				PreloadOperation currentOperation = preloadOperation;
				currentOperation.WaitForComplete(delegate(PreloadOperation.PreloadState state)
				{
					if (currentOperation == preloadOperation)
					{
						preloadOperation = null;
						if (state == PreloadOperation.PreloadState.Complete)
						{
							if (currentOperation.IsEmpty)
							{
								ChangeSaveFileState(SaveFileStates.Empty);
							}
							else
							{
								ProcessSaveStats(doAnimate: true, currentOperation.Message, currentOperation.SaveStats);
							}
						}
					}
				});
				return;
			}
			if (DemoHelper.IsDemoMode)
			{
				if (DemoHelper.HasSaveFile(SaveSlotIndex))
				{
					PrepareInternal(gameManager, fileExists: true, doAnimate, fetchRestorePoints: false);
				}
				else
				{
					base.gameObject.SetActive(value: false);
				}
			}
			else
			{
				Platform.Current.IsSaveSlotInUse(SaveSlotIndex, delegate(bool fileExists)
				{
					PrepareInternal(gameManager, fileExists, doAnimate, fetchRestorePoints);
				});
			}
		}

		private void PrepareInternal(GameManager gameManager, bool fileExists, bool doAnimate, bool fetchRestorePoints)
		{
			if (!fileExists)
			{
				ChangeSaveFileState(SaveFileStates.Empty, doAnimate);
				return;
			}
			bool isIncompatible = false;
			gameManager.GetSaveStatsForSlot(SaveSlotIndex, delegate(SaveStats newSaveStats, string errorInfo)
			{
				isIncompatible = ProcessSaveStats(doAnimate, errorInfo, newSaveStats);
				if (fetchRestorePoints)
				{
					FetchRestorePoints(isIncompatible);
				}
			});
		}

		private bool ProcessSaveStats(bool doAnimate, string errorInfo, SaveStats newSaveStats)
		{
			bool result = false;
			CheatManager.LastErrorText = errorInfo;
			if (newSaveStats == null)
			{
				ChangeSaveFileState(SaveFileStates.Corrupted, doAnimate);
			}
			else if (newSaveStats.IsBlank)
			{
				ChangeSaveFileState(SaveFileStates.Empty, doAnimate);
			}
			else if (IsVersionIncompatible(newSaveStats.Version, newSaveStats.RevisionBreak))
			{
				ChangeSaveFileState(SaveFileStates.Incompatible, doAnimate);
				result = true;
			}
			else
			{
				saveStats = newSaveStats;
				ChangeSaveFileState(SaveFileStates.LoadedStats, doAnimate);
			}
			return result;
		}

		public bool IsSaveIncompatible()
		{
			if (saveStats == null)
			{
				return true;
			}
			return IsVersionIncompatible(saveStats.Version, saveStats.RevisionBreak);
		}

		public void FetchRestorePoints(bool isIncompatible)
		{
			if ((bool)restoreSaveButton)
			{
				restoreSaveButton.PreloadRestorePoints(SaveSlotIndex, isIncompatible);
			}
		}

		private static bool IsVersionIncompatible(string fileVersionText, int fileRevisionBreak)
		{
			return SaveDataUtility.IsVersionIncompatible("1.0.28324", fileVersionText, fileRevisionBreak);
		}

		public void ClearCache()
		{
			saveFileState = SaveFileStates.NotStarted;
			saveStats = null;
		}

		private void ChangeSaveFileState(SaveFileStates nextSaveFileState, bool doAnimate = true)
		{
			saveFileState = nextSaveFileState;
			if (doAnimate && base.isActiveAndEnabled)
			{
				ShowRelevantModeForSaveFileState();
			}
		}

		private bool TryEnterSave()
		{
			if (!saveStats.IsBlackThreadInfected)
			{
				return true;
			}
			if (Time.timeAsDouble < nextImpactTime)
			{
				return false;
			}
			nextImpactTime = Time.timeAsDouble + 0.5;
			blackThreadImpactsLeft--;
			ui.UpdateBlackThreadAudio();
			if (blackThreadImpactsLeft > 0)
			{
				OnBlackThreadImpact.Invoke();
				blackThreadInfectedGroup.HideStrands(Random.Range(strandsRemovedPerHit - 1, strandsRemovedPerHit + 1));
				MinMaxFloat minMaxFloat = new MinMaxFloat(0.35f, 0.4f);
				BlackThreadStrand[] array = blackThreads;
				for (int i = 0; i < array.Length; i++)
				{
					array[i].RageForTime(minMaxFloat.GetRandomValue(), skipDelay: false);
				}
				return false;
			}
			if (blackThreadImpactsLeft == 0)
			{
				OnBlackThreadBurst.Invoke();
				PresentSaveSlot(saveStats);
				StartCoroutine(FadeUpSlotInfoUninfected());
				saveStats.HasClearedBlackThreads = true;
				return false;
			}
			return true;
		}

		private IEnumerator FadeUpSlotInfoUninfected()
		{
			ih.StopUIInput();
			if (leftCursor != null)
			{
				leftCursor.ResetTrigger(_showAnimProp);
				leftCursor.SetTrigger(_hideAnimProp);
			}
			if (rightCursor != null)
			{
				rightCursor.ResetTrigger(_showAnimProp);
				rightCursor.SetTrigger(_hideAnimProp);
			}
			SetAlpha(0f);
			yield return new WaitForSeconds(1.5f);
			float elapsed;
			for (elapsed = 0f; elapsed < 2f; elapsed += Time.deltaTime)
			{
				float t2 = elapsed / 2.5f;
				SetAlpha(t2);
				yield return null;
			}
			ih.StartUIInput();
			if (leftCursor != null)
			{
				leftCursor.ResetTrigger(_hideAnimProp);
				leftCursor.SetTrigger(_showAnimProp);
			}
			if (rightCursor != null)
			{
				rightCursor.ResetTrigger(_hideAnimProp);
				rightCursor.SetTrigger(_showAnimProp);
			}
			for (; elapsed < 2.5f; elapsed += Time.deltaTime)
			{
				float t3 = elapsed / 2.5f;
				SetAlpha(t3);
				yield return null;
			}
			SetAlpha(1f);
			void SetAlpha(float t)
			{
				activeSaveSlot.alpha = t;
				slotNumberText.alpha = t;
			}
		}

		public new void OnSubmit(BaseEventData eventData)
		{
			if (!base.interactable)
			{
				return;
			}
			Platform.Current.LocalSharedData.SetInt("lastProfileIndex", SaveSlotIndex);
			if (saveFileState == SaveFileStates.LoadedStats)
			{
				if (saveStats.PermadeathMode == PermadeathModes.Dead)
				{
					ForceDeselect();
					ClearSavePrompt();
				}
				else
				{
					if (!TryEnterSave())
					{
						return;
					}
					if (DemoHelper.IsDemoMode && !DemoHelper.IsExhibitionMode && DemoHelper.IsDummySaveFile(SaveSlotIndex))
					{
						gm.profileID = SaveSlotIndex;
						ui.StartNewGame();
					}
					else
					{
						ui.UIContinueGame(SaveSlotIndex, saveStats?.saveGameData);
					}
				}
				base.OnSubmit(eventData);
			}
			else if (saveFileState == SaveFileStates.Empty)
			{
				gm.profileID = SaveSlotIndex;
				if (gm.GetStatusRecordInt("RecPermadeathMode") == 1 || gm.GetStatusRecordInt("RecBossRushMode") == 1)
				{
					ui.UIGoToPlayModeMenu();
				}
				else
				{
					ui.StartNewGame();
				}
				base.OnSubmit(eventData);
			}
			else if (saveFileState == SaveFileStates.Corrupted)
			{
				GenericMessageCanvas.Show("SAVE_CORRUPTED", null);
			}
		}

		protected IEnumerator ReloadCorrupted()
		{
			ih.StopUIInput();
			Prepare(gm, isReload: true);
			while (saveFileState == SaveFileStates.OperationInProgress)
			{
				yield return null;
			}
			ih.StartUIInput();
		}

		public new void OnPointerClick(PointerEventData eventData)
		{
			OnSubmit(eventData);
		}

		public new void OnSelect(BaseEventData eventData)
		{
			highlight.ResetTrigger(_hideAnimProp);
			highlight.SetTrigger(_showAnimProp);
			if (leftCursor != null)
			{
				leftCursor.ResetTrigger(_hideAnimProp);
				leftCursor.SetTrigger(_showAnimProp);
			}
			if (rightCursor != null)
			{
				rightCursor.ResetTrigger(_hideAnimProp);
				rightCursor.SetTrigger(_showAnimProp);
			}
			base.OnSelect(eventData);
			if (!base.interactable)
			{
				try
				{
					uiAudioPlayer.PlaySelect();
				}
				catch (Exception ex)
				{
					Debug.LogError(base.name + " doesn't have a select sound specified. " + ex);
				}
			}
		}

		public new void OnDeselect(BaseEventData eventData)
		{
			StartCoroutine(ValidateDeselect());
		}

		public void UpdateSaveFileState()
		{
			if (saveFileState != SaveFileStates.Empty || preloadOperation == null)
			{
				return;
			}
			switch (preloadOperation.State)
			{
			case PreloadOperation.PreloadState.Loading:
				saveFileState = SaveFileStates.OperationInProgress;
				break;
			case PreloadOperation.PreloadState.Complete:
				if (preloadOperation.IsEmpty)
				{
					saveFileState = SaveFileStates.Empty;
				}
				else
				{
					ProcessSaveStats(doAnimate: false, preloadOperation.Message, preloadOperation.SaveStats);
				}
				break;
			}
		}

		public void ShowRelevantModeForSaveFileState()
		{
			if (base.gameObject.activeInHierarchy)
			{
				switch (saveFileState)
				{
				case SaveFileStates.Empty:
					coroutineQueue.Enqueue(AnimateToSlotState(SlotState.EmptySlot));
					break;
				case SaveFileStates.NotStarted:
				case SaveFileStates.OperationInProgress:
					coroutineQueue.Enqueue(AnimateToSlotState(SlotState.OperationInProgress));
					break;
				case SaveFileStates.LoadedStats:
				{
					SlotState nextState = (saveStats.IsBlackThreadInfected ? SlotState.BlackThreadInfected : ((saveStats.PermadeathMode != PermadeathModes.Dead) ? SlotState.SavePresent : SlotState.Defeated));
					coroutineQueue.Enqueue(AnimateToSlotState(nextState));
					break;
				}
				case SaveFileStates.Corrupted:
					coroutineQueue.Enqueue(AnimateToSlotState(SlotState.Corrupted));
					break;
				case SaveFileStates.Incompatible:
					coroutineQueue.Enqueue(AnimateToSlotState(SlotState.Incompatible));
					break;
				default:
					throw new ArgumentOutOfRangeException();
				}
			}
		}

		public void HideSaveSlot(bool updateBlackThread)
		{
			if (base.gameObject.activeInHierarchy)
			{
				coroutineQueue.Enqueue(AnimateToSlotState(SlotState.Hidden, updateBlackThread));
			}
		}

		public void ClearSavePrompt()
		{
			coroutineQueue.Enqueue(AnimateToSlotState(SlotState.ClearPrompt));
		}

		public void ClearSaveConfirmPrompt()
		{
			if (saveFileState == SaveFileStates.LoadedStats && saveStats != null && saveStats.PermadeathMode == PermadeathModes.Dead)
			{
				ClearSaveFile();
			}
			else
			{
				coroutineQueue.Enqueue(AnimateToSlotState(SlotState.ClearConfirm));
			}
		}

		public void CancelClearSave()
		{
			if (State == SlotState.ClearPrompt || State == SlotState.ClearConfirm)
			{
				parentBlocker.blocksRaycasts = true;
				ShowRelevantModeForSaveFileState();
			}
		}

		public void ClearSaveFile()
		{
			gm.ClearSaveFile(SaveSlotIndex, delegate(bool didClear)
			{
				if (didClear)
				{
					saveStats = null;
					ChangeSaveFileState(SaveFileStates.Empty);
				}
				else
				{
					coroutineQueue.Enqueue(AnimateToSlotState(SlotState.SavePresent));
				}
			});
		}

		public IEnumerator ShowLoadingPrompt(float delay)
		{
			if (State == SlotState.RestoreSave)
			{
				ResetFluerTriggers();
				yield return new WaitForSeconds(0.1f);
				StartCoroutine(currentLoadingTextFadeIn = FadeInCanvasGroupAfterDelay(delay, loadingText));
			}
		}

		public IEnumerator HideLoadingPrompt()
		{
			if (State != SlotState.RestoreSave)
			{
				Debug.LogError($"Unexpected call during {State} state detected", this);
				yield break;
			}
			if (currentLoadingTextFadeIn != null)
			{
				StopCoroutine(currentLoadingTextFadeIn);
			}
			yield return ui.FadeOutCanvasGroup(loadingText);
		}

		public void RestoreSaveMenu()
		{
			coroutineQueue.Enqueue(AnimateToSlotState(SlotState.RestoreSave));
		}

		public void OverrideSaveData(RestorePointData data)
		{
			if (State != SlotState.RestoreSave)
			{
				Debug.LogError("Unable to override data while not in restore save state", this);
				return;
			}
			isRestoringSave = true;
			coroutineQueue.Enqueue(AnimateToSlotState(SlotState.OperationInProgress));
			Platform.Current.WriteSaveSlot(SaveSlotIndex, GameManager.instance.GetBytesForSaveData(data.saveGameData), delegate(bool result)
			{
				if (result)
				{
					saveStats = GameManager.GetSaveStatsFromData(data.saveGameData);
				}
				else
				{
					Debug.LogError("Failed to override data", this);
				}
				preloadOperation = null;
				ProcessSaveStats(doAnimate: true, null, saveStats);
			});
		}

		private IEnumerator FadeInCanvasGroupAfterDelay(float delay, CanvasGroup cg)
		{
			for (float timer = 0f; timer < delay; timer += Time.unscaledDeltaTime)
			{
				yield return null;
			}
			yield return ui.FadeInCanvasGroup(cg);
		}

		private IEnumerator AnimateToSlotState(SlotState nextState, bool updateBlackThread = true)
		{
			SlotState state = State;
			if (state == nextState)
			{
				yield break;
			}
			if (currentLoadingTextFadeIn != null)
			{
				StopCoroutine(currentLoadingTextFadeIn);
				currentLoadingTextFadeIn = null;
			}
			State = nextState;
			if (updateBlackThread)
			{
				if (nextState == SlotState.BlackThreadInfected && !saveStats.HasClearedBlackThreads)
				{
					blackThreadImpactsLeft = 5;
				}
				ui.UpdateBlackThreadAudio();
			}
			if (!DemoHelper.IsDemoMode)
			{
				switch (nextState)
				{
				case SlotState.Hidden:
				case SlotState.OperationInProgress:
					base.navigation = noNav;
					break;
				case SlotState.EmptySlot:
				case SlotState.BlackThreadInfected:
					base.navigation = emptySlotNav;
					break;
				case SlotState.Defeated:
					base.navigation = defeatedSlotNav;
					break;
				case SlotState.SavePresent:
				case SlotState.Corrupted:
				case SlotState.Incompatible:
				case SlotState.RestoreSave:
					base.navigation = fullSlotNav;
					break;
				}
			}
			if ((bool)highlightImage)
			{
				highlightImage.material = ((nextState == SlotState.SavePresent || nextState == SlotState.BlackThreadInfected) ? highlightMatFull : highlightMatEmpty);
			}
			if (nextState != SlotState.ClearPrompt && nextState != SlotState.ClearConfirm)
			{
				clearSaveButton.transform.SetLocalPositionX(clearSaveButtonX);
			}
			switch (state)
			{
			case SlotState.Hidden:
				switch (nextState)
				{
				case SlotState.OperationInProgress:
					ResetFluerTriggers();
					yield return new WaitForSeconds(0.1f);
					StartCoroutine(currentLoadingTextFadeIn = FadeInCanvasGroupAfterDelay(5f, loadingText));
					break;
				case SlotState.EmptySlot:
					backgroundCg.alpha = 0f;
					backgroundCg.gameObject.SetActive(value: true);
					ResetFluerTriggers();
					yield return new WaitForSeconds(0.1f);
					StartCoroutine(ui.FadeInCanvasGroup(slotNumberText));
					StartCoroutine(ui.FadeInCanvasGroup(newGameText));
					break;
				case SlotState.SavePresent:
				case SlotState.BlackThreadInfected:
					if (nextState == SlotState.BlackThreadInfected && !saveStats.HasClearedBlackThreads)
					{
						blackThreadImpactsLeft = 5;
					}
					ResetFluerTriggers();
					yield return new WaitForSeconds(0.1f);
					PresentSaveSlot(saveStats);
					SavePresentFadeIn(saveStats);
					break;
				case SlotState.Defeated:
					ResetFluerTriggers();
					yield return new WaitForSeconds(0.1f);
					StartCoroutine(ui.FadeInCanvasGroup(defeatedBackground));
					StartCoroutine(ui.FadeInCanvasGroup(defeatedText));
					StartCoroutine(ui.FadeInCanvasGroup(brokenSteelOrb));
					clearSaveButton.transform.SetLocalPositionX(0f);
					StartCoroutine(ui.FadeInCanvasGroup(clearSaveButton));
					StartCoroutine(restoreSaveButton.Hide());
					clearSaveButton.blocksRaycasts = true;
					myCanvasGroup.blocksRaycasts = true;
					break;
				case SlotState.Corrupted:
				case SlotState.Incompatible:
					backgroundCg.alpha = 0f;
					backgroundCg.gameObject.SetActive(value: true);
					ResetFluerTriggers();
					yield return new WaitForSeconds(0.1f);
					StartCoroutine(ui.FadeInCanvasGroup(slotNumberText));
					switch (nextState)
					{
					case SlotState.Corrupted:
						StartCoroutine(ui.FadeInCanvasGroup(saveCorruptedText));
						break;
					case SlotState.Incompatible:
						StartCoroutine(ui.FadeInCanvasGroup(saveIncompatibleText));
						break;
					}
					StartCoroutine(ui.FadeInCanvasGroup(clearSaveButton));
					StartCoroutine(restoreSaveButton.ShowRestoreSaveButton());
					clearSaveButton.blocksRaycasts = true;
					myCanvasGroup.blocksRaycasts = true;
					break;
				}
				break;
			case SlotState.OperationInProgress:
				switch (nextState)
				{
				case SlotState.Hidden:
					ResetFluerTriggersReversed();
					yield return new WaitForSeconds(0.1f);
					StartCoroutine(ui.FadeOutCanvasGroup(loadingText));
					break;
				case SlotState.EmptySlot:
					yield return StartCoroutine(ui.FadeOutCanvasGroup(loadingText));
					StartCoroutine(ui.FadeInCanvasGroup(slotNumberText));
					StartCoroutine(ui.FadeInCanvasGroup(newGameText));
					break;
				case SlotState.SavePresent:
				case SlotState.BlackThreadInfected:
					if (!saveStats.HasClearedBlackThreads)
					{
						blackThreadImpactsLeft = 5;
					}
					yield return StartCoroutine(ui.FadeOutCanvasGroup(loadingText));
					PresentSaveSlot(saveStats);
					SavePresentFadeIn(saveStats);
					if (isRestoringSave)
					{
						isRestoringSave = false;
						StartCoroutine(ui.FadeInCanvasGroup(backgroundCg));
						StartCoroutine(ui.FadeInCanvasGroup(activeSaveSlot));
						if (!DemoHelper.IsDemoMode)
						{
							if (!saveStats.IsBlackThreadInfected || blackThreadImpactsLeft <= 0)
							{
								StartCoroutine(ui.FadeInCanvasGroup(slotNumberText));
							}
							if (!saveStats.IsBlackThreadInfected)
							{
								StartCoroutine(restoreSaveButton.ShowRestoreSaveButton());
								yield return ui.FadeInCanvasGroup(clearSaveButton);
								clearSaveButton.blocksRaycasts = true;
							}
						}
						ih.StartUIInput();
						Select();
					}
					else
					{
						SavePresentFadeIn(saveStats);
					}
					break;
				case SlotState.Defeated:
					yield return StartCoroutine(ui.FadeOutCanvasGroup(loadingText));
					StartCoroutine(ui.FadeInCanvasGroup(defeatedBackground));
					StartCoroutine(ui.FadeInCanvasGroup(defeatedText));
					StartCoroutine(ui.FadeInCanvasGroup(brokenSteelOrb));
					clearSaveButton.transform.SetLocalPositionX(0f);
					StartCoroutine(ui.FadeInCanvasGroup(clearSaveButton));
					StartCoroutine(restoreSaveButton.Hide());
					clearSaveButton.blocksRaycasts = true;
					myCanvasGroup.blocksRaycasts = true;
					break;
				case SlotState.Corrupted:
				case SlotState.Incompatible:
					yield return StartCoroutine(ui.FadeOutCanvasGroup(loadingText));
					StartCoroutine(ui.FadeInCanvasGroup(slotNumberText));
					switch (nextState)
					{
					case SlotState.Corrupted:
						StartCoroutine(ui.FadeInCanvasGroup(saveCorruptedText));
						break;
					case SlotState.Incompatible:
						StartCoroutine(ui.FadeInCanvasGroup(saveIncompatibleText));
						break;
					}
					StartCoroutine(ui.FadeInCanvasGroup(clearSaveButton));
					StartCoroutine(restoreSaveButton.ShowRestoreSaveButton());
					clearSaveButton.blocksRaycasts = true;
					myCanvasGroup.blocksRaycasts = true;
					break;
				}
				break;
			case SlotState.SavePresent:
			case SlotState.BlackThreadInfected:
				switch (nextState)
				{
				case SlotState.ClearPrompt:
					ih.StopUIInput();
					base.interactable = false;
					myCanvasGroup.blocksRaycasts = false;
					StartCoroutine(ui.FadeOutCanvasGroup(slotNumberText));
					StartCoroutine(ui.FadeOutCanvasGroup(activeSaveSlot));
					StartCoroutine(ui.FadeOutCanvasGroup(backgroundCg));
					StartCoroutine(restoreSaveButton.Hide());
					yield return StartCoroutine(ui.FadeOutCanvasGroup(clearSaveButton, disable: false));
					clearSaveButton.blocksRaycasts = false;
					parentBlocker.blocksRaycasts = false;
					yield return StartCoroutine(ui.FadeInCanvasGroup(clearSavePrompt));
					clearSavePrompt.interactable = true;
					clearSavePrompt.blocksRaycasts = true;
					clearSavePromptHighlight.HighlightDefault();
					ih.StartUIInput();
					break;
				case SlotState.Hidden:
					ResetFluerTriggersReversed();
					yield return new WaitForSeconds(0.1f);
					StartCoroutine(ui.FadeOutCanvasGroup(slotNumberText));
					StartCoroutine(ui.FadeOutCanvasGroup(backgroundCg));
					StartCoroutine(ui.FadeOutCanvasGroup(activeSaveSlot));
					StartCoroutine(restoreSaveButton.Hide());
					StartCoroutine(ui.FadeOutCanvasGroup(clearSaveButton, disable: false));
					clearSaveButton.blocksRaycasts = false;
					break;
				case SlotState.RestoreSave:
					ih.StopUIInput();
					base.interactable = false;
					myCanvasGroup.blocksRaycasts = false;
					StartCoroutine(ui.FadeOutCanvasGroup(slotNumberText));
					StartCoroutine(ui.FadeOutCanvasGroup(activeSaveSlot));
					StartCoroutine(ui.FadeOutCanvasGroup(backgroundCg));
					StartCoroutine(ui.FadeOutCanvasGroup(clearSaveButton, disable: false));
					clearSaveButton.blocksRaycasts = false;
					parentBlocker.blocksRaycasts = false;
					yield return restoreSaveButton.ShowSaveSelection();
					break;
				}
				break;
			case SlotState.ClearPrompt:
			case SlotState.ClearConfirm:
			{
				CanvasGroup currentFadePrompt = ((state == SlotState.ClearPrompt) ? clearSavePrompt : clearSaveConfirmPrompt);
				switch (nextState)
				{
				case SlotState.ClearConfirm:
					ih.StopUIInput();
					yield return StartCoroutine(ui.FadeOutCanvasGroup(currentFadePrompt));
					currentFadePrompt.interactable = false;
					yield return StartCoroutine(ui.FadeInCanvasGroup(clearSaveConfirmPrompt));
					clearSaveConfirmPrompt.interactable = true;
					clearSaveConfirmPrompt.blocksRaycasts = true;
					clearSaveConfirmPromptHighlight.HighlightDefault();
					ih.StartUIInput();
					break;
				case SlotState.SavePresent:
				case SlotState.BlackThreadInfected:
					ih.StopUIInput();
					yield return StartCoroutine(ui.FadeOutCanvasGroup(currentFadePrompt));
					parentBlocker.blocksRaycasts = true;
					currentFadePrompt.interactable = false;
					currentFadePrompt.blocksRaycasts = false;
					if (saveStats != null)
					{
						PresentSaveSlot(saveStats);
					}
					StartCoroutine(ui.FadeInCanvasGroup(slotNumberText));
					StartCoroutine(ui.FadeInCanvasGroup(activeSaveSlot));
					StartCoroutine(ui.FadeInCanvasGroup(backgroundCg));
					StartCoroutine(restoreSaveButton.ShowRestoreSaveButton());
					yield return StartCoroutine(ui.FadeInCanvasGroup(clearSaveButton));
					clearSaveButton.blocksRaycasts = true;
					base.interactable = true;
					myCanvasGroup.blocksRaycasts = true;
					Select();
					ih.StartUIInput();
					break;
				case SlotState.EmptySlot:
					ih.StopUIInput();
					StartCoroutine(ui.FadeOutCanvasGroup(backgroundCg));
					yield return StartCoroutine(ui.FadeOutCanvasGroup(currentFadePrompt));
					currentFadePrompt.interactable = false;
					currentFadePrompt.blocksRaycasts = false;
					parentBlocker.blocksRaycasts = true;
					StartCoroutine(ui.FadeInCanvasGroup(slotNumberText));
					yield return StartCoroutine(ui.FadeInCanvasGroup(newGameText));
					myCanvasGroup.blocksRaycasts = true;
					base.interactable = true;
					Select();
					ih.StartUIInput();
					break;
				case SlotState.Defeated:
					ih.StopUIInput();
					StartCoroutine(ui.FadeOutCanvasGroup(backgroundCg));
					yield return StartCoroutine(ui.FadeOutCanvasGroup(currentFadePrompt));
					currentFadePrompt.interactable = false;
					currentFadePrompt.blocksRaycasts = false;
					parentBlocker.blocksRaycasts = true;
					StartCoroutine(ui.FadeInCanvasGroup(defeatedBackground));
					StartCoroutine(ui.FadeInCanvasGroup(defeatedText));
					StartCoroutine(ui.FadeInCanvasGroup(brokenSteelOrb));
					StartCoroutine(restoreSaveButton.Hide());
					clearSaveButton.transform.SetLocalPositionX(0f);
					yield return StartCoroutine(ui.FadeInCanvasGroup(clearSaveButton));
					base.interactable = true;
					clearSaveButton.blocksRaycasts = true;
					myCanvasGroup.blocksRaycasts = true;
					Select();
					ih.StartUIInput();
					break;
				case SlotState.Hidden:
					yield return StartCoroutine(ui.FadeOutCanvasGroup(currentFadePrompt));
					break;
				case SlotState.Corrupted:
				case SlotState.Incompatible:
					ih.StopUIInput();
					yield return StartCoroutine(ui.FadeOutCanvasGroup(currentFadePrompt));
					currentFadePrompt.interactable = false;
					currentFadePrompt.blocksRaycasts = false;
					parentBlocker.blocksRaycasts = true;
					StartCoroutine(ui.FadeInCanvasGroup(slotNumberText));
					switch (nextState)
					{
					case SlotState.Corrupted:
						StartCoroutine(ui.FadeInCanvasGroup(saveCorruptedText));
						break;
					case SlotState.Incompatible:
						StartCoroutine(ui.FadeInCanvasGroup(saveIncompatibleText));
						break;
					}
					StartCoroutine(restoreSaveButton.ShowRestoreSaveButton());
					yield return StartCoroutine(ui.FadeInCanvasGroup(clearSaveButton));
					clearSaveButton.blocksRaycasts = true;
					myCanvasGroup.blocksRaycasts = true;
					Select();
					ih.StartUIInput();
					break;
				}
				break;
			}
			case SlotState.EmptySlot:
				if (nextState == SlotState.Hidden)
				{
					ResetFluerTriggersReversed();
					yield return new WaitForSeconds(0.1f);
					StartCoroutine(ui.FadeOutCanvasGroup(slotNumberText));
					StartCoroutine(ui.FadeOutCanvasGroup(backgroundCg));
					StartCoroutine(ui.FadeOutCanvasGroup(newGameText));
				}
				break;
			case SlotState.Defeated:
				switch (nextState)
				{
				case SlotState.ClearPrompt:
					ih.StopUIInput();
					base.interactable = false;
					myCanvasGroup.blocksRaycasts = false;
					StartCoroutine(ui.FadeOutCanvasGroup(defeatedBackground));
					StartCoroutine(ui.FadeOutCanvasGroup(defeatedText));
					StartCoroutine(ui.FadeOutCanvasGroup(brokenSteelOrb));
					StartCoroutine(ui.FadeOutCanvasGroup(restoreSaveButton.CanvasGroup, disable: false));
					yield return StartCoroutine(ui.FadeOutCanvasGroup(clearSaveButton, disable: false));
					clearSaveButton.blocksRaycasts = false;
					parentBlocker.blocksRaycasts = false;
					yield return StartCoroutine(ui.FadeInCanvasGroup(clearSavePrompt));
					clearSavePrompt.interactable = true;
					clearSavePrompt.blocksRaycasts = true;
					clearSavePromptHighlight.HighlightDefault();
					base.interactable = false;
					myCanvasGroup.blocksRaycasts = false;
					ih.StartUIInput();
					break;
				case SlotState.Hidden:
					ResetFluerTriggersReversed();
					yield return new WaitForSeconds(0.1f);
					StartCoroutine(ui.FadeOutCanvasGroup(slotNumberText));
					StartCoroutine(ui.FadeOutCanvasGroup(backgroundCg));
					StartCoroutine(ui.FadeOutCanvasGroup(activeSaveSlot));
					StartCoroutine(ui.FadeOutCanvasGroup(defeatedBackground));
					StartCoroutine(ui.FadeOutCanvasGroup(defeatedText));
					StartCoroutine(ui.FadeOutCanvasGroup(brokenSteelOrb));
					StartCoroutine(ui.FadeOutCanvasGroup(restoreSaveButton.CanvasGroup));
					StartCoroutine(ui.FadeOutCanvasGroup(clearSaveButton, disable: false));
					clearSaveButton.blocksRaycasts = false;
					break;
				case SlotState.RestoreSave:
					ih.StopUIInput();
					base.interactable = false;
					myCanvasGroup.blocksRaycasts = false;
					StartCoroutine(ui.FadeOutCanvasGroup(defeatedBackground));
					StartCoroutine(ui.FadeOutCanvasGroup(defeatedText));
					StartCoroutine(ui.FadeOutCanvasGroup(brokenSteelOrb));
					StartCoroutine(ui.FadeOutCanvasGroup(restoreSaveButton.CanvasGroup));
					StartCoroutine(ui.FadeOutCanvasGroup(slotNumberText));
					StartCoroutine(ui.FadeOutCanvasGroup(activeSaveSlot));
					StartCoroutine(ui.FadeOutCanvasGroup(backgroundCg));
					StartCoroutine(ui.FadeOutCanvasGroup(clearSaveButton, disable: false));
					clearSaveButton.blocksRaycasts = false;
					parentBlocker.blocksRaycasts = false;
					yield return restoreSaveButton.ShowSaveSelection();
					break;
				}
				break;
			case SlotState.Corrupted:
			case SlotState.Incompatible:
				switch (nextState)
				{
				case SlotState.ClearPrompt:
					ih.StopUIInput();
					base.interactable = false;
					myCanvasGroup.blocksRaycasts = false;
					StartCoroutine(ui.FadeOutCanvasGroup(slotNumberText));
					StartCoroutine(ui.FadeOutCanvasGroup(saveCorruptedText));
					StartCoroutine(ui.FadeOutCanvasGroup(saveIncompatibleText));
					StartCoroutine(restoreSaveButton.Hide());
					yield return StartCoroutine(ui.FadeOutCanvasGroup(clearSaveButton, disable: false));
					clearSaveButton.blocksRaycasts = false;
					parentBlocker.blocksRaycasts = false;
					yield return StartCoroutine(ui.FadeInCanvasGroup(clearSavePrompt));
					clearSavePrompt.interactable = true;
					clearSavePrompt.blocksRaycasts = true;
					clearSavePromptHighlight.HighlightDefault();
					base.interactable = false;
					myCanvasGroup.blocksRaycasts = false;
					ih.StartUIInput();
					break;
				case SlotState.Hidden:
					ResetFluerTriggersReversed();
					yield return new WaitForSeconds(0.1f);
					StartCoroutine(ui.FadeOutCanvasGroup(slotNumberText));
					StartCoroutine(ui.FadeOutCanvasGroup(saveCorruptedText));
					StartCoroutine(ui.FadeOutCanvasGroup(saveIncompatibleText));
					StartCoroutine(restoreSaveButton.Hide());
					StartCoroutine(ui.FadeOutCanvasGroup(clearSaveButton, disable: false));
					clearSaveButton.blocksRaycasts = false;
					break;
				case SlotState.OperationInProgress:
					StartCoroutine(ui.FadeOutCanvasGroup(slotNumberText));
					StartCoroutine(ui.FadeOutCanvasGroup(saveCorruptedText));
					StartCoroutine(ui.FadeOutCanvasGroup(saveIncompatibleText));
					StartCoroutine(restoreSaveButton.Hide());
					yield return StartCoroutine(ui.FadeOutCanvasGroup(clearSaveButton, disable: false));
					StartCoroutine(currentLoadingTextFadeIn = FadeInCanvasGroupAfterDelay(5f, loadingText));
					break;
				case SlotState.RestoreSave:
					ih.StopUIInput();
					base.interactable = false;
					myCanvasGroup.blocksRaycasts = false;
					StartCoroutine(ui.FadeOutCanvasGroup(slotNumberText));
					StartCoroutine(ui.FadeOutCanvasGroup(saveCorruptedText));
					StartCoroutine(ui.FadeOutCanvasGroup(saveIncompatibleText));
					StartCoroutine(restoreSaveButton.Hide());
					StartCoroutine(ui.FadeOutCanvasGroup(backgroundCg));
					StartCoroutine(ui.FadeOutCanvasGroup(clearSaveButton, disable: false));
					clearSaveButton.blocksRaycasts = false;
					parentBlocker.blocksRaycasts = false;
					yield return restoreSaveButton.ShowSaveSelection();
					break;
				}
				break;
			case SlotState.RestoreSave:
				yield return ProcessRestoreSaveState(nextState);
				break;
			}
		}

		private IEnumerator ProcessRestoreSaveState(SlotState nextState)
		{
			switch (nextState)
			{
			case SlotState.Hidden:
				ResetFluerTriggersReversed();
				yield return new WaitForSeconds(0.1f);
				StartCoroutine(ui.FadeOutCanvasGroup(slotNumberText));
				StartCoroutine(ui.FadeOutCanvasGroup(backgroundCg));
				StartCoroutine(ui.FadeOutCanvasGroup(activeSaveSlot));
				StartCoroutine(ui.FadeOutCanvasGroup(clearSaveButton, disable: false));
				StartCoroutine(restoreSaveButton.Hide());
				clearSaveButton.blocksRaycasts = false;
				base.interactable = true;
				myCanvasGroup.blocksRaycasts = true;
				break;
			case SlotState.SavePresent:
				yield return restoreSaveButton.Hide();
				blackThreadImpactsLeft = 5;
				parentBlocker.blocksRaycasts = true;
				PresentSaveSlot(saveStats);
				StartCoroutine(ui.FadeInCanvasGroup(backgroundCg));
				StartCoroutine(ui.FadeInCanvasGroup(activeSaveSlot));
				StartCoroutine(ui.FadeInCanvasGroup(slotNumberText));
				StartCoroutine(restoreSaveButton.ShowRestoreSaveButton());
				yield return ui.FadeInCanvasGroup(clearSaveButton);
				clearSaveButton.blocksRaycasts = true;
				myCanvasGroup.blocksRaycasts = true;
				base.interactable = true;
				Select();
				ih.StartUIInput();
				break;
			case SlotState.Corrupted:
				ih.StopUIInput();
				yield return restoreSaveButton.Hide();
				parentBlocker.blocksRaycasts = true;
				StartCoroutine(ui.FadeInCanvasGroup(slotNumberText));
				StartCoroutine(ui.FadeInCanvasGroup(saveCorruptedText));
				StartCoroutine(restoreSaveButton.ShowRestoreSaveButton());
				yield return StartCoroutine(ui.FadeInCanvasGroup(clearSaveButton));
				base.interactable = true;
				clearSaveButton.blocksRaycasts = true;
				myCanvasGroup.blocksRaycasts = true;
				Select();
				ih.StartUIInput();
				break;
			case SlotState.Incompatible:
				ih.StopUIInput();
				yield return restoreSaveButton.Hide();
				parentBlocker.blocksRaycasts = true;
				StartCoroutine(ui.FadeInCanvasGroup(slotNumberText));
				StartCoroutine(ui.FadeInCanvasGroup(saveIncompatibleText));
				StartCoroutine(restoreSaveButton.ShowRestoreSaveButton());
				yield return StartCoroutine(ui.FadeInCanvasGroup(clearSaveButton));
				clearSaveButton.blocksRaycasts = true;
				myCanvasGroup.blocksRaycasts = true;
				Select();
				ih.StartUIInput();
				break;
			case SlotState.Defeated:
				ih.StopUIInput();
				StartCoroutine(ui.FadeOutCanvasGroup(backgroundCg));
				yield return restoreSaveButton.Hide();
				parentBlocker.blocksRaycasts = true;
				StartCoroutine(ui.FadeInCanvasGroup(defeatedBackground));
				StartCoroutine(ui.FadeInCanvasGroup(defeatedText));
				StartCoroutine(ui.FadeInCanvasGroup(brokenSteelOrb));
				StartCoroutine(restoreSaveButton.Hide());
				yield return StartCoroutine(ui.FadeInCanvasGroup(clearSaveButton));
				clearSaveButton.blocksRaycasts = true;
				myCanvasGroup.blocksRaycasts = true;
				Select();
				ih.StartUIInput();
				break;
			case SlotState.OperationInProgress:
				yield return restoreSaveButton.Hide();
				ih.StopUIInput();
				base.interactable = true;
				parentBlocker.blocksRaycasts = true;
				clearSaveButton.blocksRaycasts = false;
				myCanvasGroup.blocksRaycasts = true;
				StartCoroutine(currentLoadingTextFadeIn = FadeInCanvasGroupAfterDelay(5f, loadingText));
				break;
			default:
				Debug.LogError($"Unhandled state transition from {SlotState.RestoreSave} to {nextState} detected", this);
				break;
			}
		}

		private void SavePresentFadeIn(SaveStats currentSaveStats)
		{
			StartCoroutine(ui.FadeInCanvasGroup(backgroundCg));
			StartCoroutine(ui.FadeInCanvasGroup(activeSaveSlot));
			if (!DemoHelper.IsDemoMode)
			{
				if (!currentSaveStats.IsBlackThreadInfected || blackThreadImpactsLeft <= 0)
				{
					StartCoroutine(ui.FadeInCanvasGroup(slotNumberText));
				}
				if (!currentSaveStats.IsBlackThreadInfected)
				{
					StartCoroutine(restoreSaveButton.ShowRestoreSaveButton());
					StartCoroutine(ui.FadeInCanvasGroup(clearSaveButton));
					clearSaveButton.blocksRaycasts = true;
				}
			}
		}

		private void ResetFluerTriggers()
		{
			Animator[] array = frameFluers;
			foreach (Animator obj in array)
			{
				obj.ResetTrigger(_hideAnimProp);
				obj.SetTrigger(_showAnimProp);
			}
		}

		private void ResetFluerTriggersReversed()
		{
			Animator[] array = frameFluers;
			foreach (Animator obj in array)
			{
				obj.ResetTrigger(_showAnimProp);
				obj.SetTrigger(_hideAnimProp);
			}
		}

		private void PresentSaveSlot(SaveStats currentSaveStats)
		{
			rosaryGroup.SetActive(value: true);
			shardGroup.SetActive(value: true);
			completionText.gameObject.SetActive(value: true);
			background.enabled = true;
			blackThreadInfectedGroup.gameObject.SetActive(value: false);
			if ((bool)saveSlotCompletionIcons)
			{
				saveSlotCompletionIcons.SetCompletionIconState(currentSaveStats);
			}
			SaveSlotBackgrounds.AreaBackground areaBackground = null;
			string text = null;
			if (currentSaveStats.BossRushMode)
			{
				healthSlots.ShowHealth(currentSaveStats.MaxHealth, steelsoulMode: false, currentSaveStats.CrestId);
				SetSilkDisplay(currentSaveStats);
				playTimeText.text = currentSaveStats.GetPlaytimeHHMM();
				rosaryGroup.SetActive(value: false);
				shardGroup.SetActive(value: false);
				completionText.gameObject.SetActive(value: false);
				areaBackground = saveSlots.GetBackground(currentSaveStats);
			}
			else if (currentSaveStats.IsBlackThreadInfected)
			{
				healthSlots.gameObject.SetActive(value: false);
				silkBar.gameObject.SetActive(value: false);
				rosaryGroup.gameObject.SetActive(value: false);
				shardGroup.gameObject.SetActive(value: false);
				completionText.gameObject.SetActive(value: false);
				playTimeText.gameObject.SetActive(value: false);
				background.enabled = false;
				blackThreadOverlay.SetActive(value: false);
				if ((bool)saveSlotCompletionIcons)
				{
					saveSlotCompletionIcons.gameObject.SetActive(value: false);
				}
				if (blackThreadImpactsLeft > 0)
				{
					locationText.gameObject.SetActive(value: false);
					slotNumberText.gameObject.SetActive(value: false);
					blackThreadInfectedGroup.gameObject.SetActive(value: true);
					blackThreadInfectedGroup.ResetVisibleStrands();
				}
				else
				{
					text = "???";
					locationText.gameObject.SetActive(value: true);
					slotNumberText.gameObject.SetActive(value: true);
					blackThreadInfectedGroup.gameObject.SetActive(value: false);
					background.enabled = true;
				}
				areaBackground = saveSlots.GetBackground(MapZone.CRADLE);
			}
			else if (currentSaveStats.PermadeathMode == PermadeathModes.Off)
			{
				if (!DemoHelper.IsDemoMode)
				{
					healthSlots.ShowHealth(currentSaveStats.MaxHealth, steelsoulMode: false, currentSaveStats.CrestId);
					SetSilkDisplay(currentSaveStats);
					rosaryText.text = currentSaveStats.Geo.ToString();
					shardText.text = currentSaveStats.Shards.ToString();
					if (currentSaveStats.UnlockedCompletionRate)
					{
						completionText.text = currentSaveStats.CompletionPercentage + "%";
					}
					else
					{
						completionText.gameObject.SetActive(value: false);
					}
					playTimeText.text = currentSaveStats.GetPlaytimeHHMM();
				}
				else
				{
					healthSlots.gameObject.SetActive(value: false);
					silkBar.gameObject.SetActive(value: false);
					rosaryGroup.gameObject.SetActive(value: false);
					shardGroup.gameObject.SetActive(value: false);
					completionText.gameObject.SetActive(value: false);
					playTimeText.gameObject.SetActive(value: false);
					slotNumberText.gameObject.SetActive(value: false);
				}
				areaBackground = saveSlots.GetBackground(currentSaveStats);
			}
			else if (currentSaveStats.PermadeathMode == PermadeathModes.On)
			{
				healthSlots.ShowHealth(currentSaveStats.MaxHealth, steelsoulMode: true, currentSaveStats.CrestId);
				SetSilkDisplay(currentSaveStats);
				rosaryText.text = currentSaveStats.Geo.ToString();
				shardText.text = currentSaveStats.Shards.ToString();
				if (currentSaveStats.UnlockedCompletionRate)
				{
					completionText.text = currentSaveStats.CompletionPercentage.ToString(CultureInfo.InvariantCulture) + "%";
				}
				else
				{
					completionText.gameObject.SetActive(value: false);
				}
				playTimeText.text = currentSaveStats.GetPlaytimeHHMM();
				areaBackground = saveSlots.GetBackground(currentSaveStats);
			}
			string text2;
			if (text != null)
			{
				text2 = text;
				if (areaBackground != null)
				{
					if (saveStats.IsAct3 && areaBackground.Act3BackgroundImage != null)
					{
						background.sprite = areaBackground.Act3BackgroundImage;
					}
					else
					{
						background.sprite = areaBackground.BackgroundImage;
					}
				}
			}
			else if (areaBackground != null)
			{
				if (saveStats.IsAct3 && areaBackground.Act3BackgroundImage != null)
				{
					background.sprite = areaBackground.Act3BackgroundImage;
				}
				else
				{
					background.sprite = areaBackground.BackgroundImage;
				}
				text2 = ((!areaBackground.NameOverride.IsEmpty) ? ((string)areaBackground.NameOverride) : gm.GetFormattedMapZoneString(currentSaveStats.MapZone));
			}
			else
			{
				text2 = gm.GetFormattedMapZoneString(currentSaveStats.MapZone);
			}
			locationText.text = text2.Replace("<br>", Environment.NewLine);
			blackThreadOverlay.SetActive(currentSaveStats.IsAct3IntroCompleted && areaBackground != null && !areaBackground.Act3OverlayOptOut);
		}

		private void SetSilkDisplay(SaveStats stats)
		{
			silkBar.ShowSilk(stats.IsSpoolBroken, stats.MaxSilk, stats.CrestId == "Cursed");
		}

		private void SetupNavs()
		{
			Navigation navigation = base.navigation;
			noNav = new Navigation
			{
				mode = Navigation.Mode.Explicit,
				selectOnLeft = navigation.selectOnLeft,
				selectOnRight = navigation.selectOnRight,
				selectOnUp = null,
				selectOnDown = null
			};
			emptySlotNav = new Navigation
			{
				mode = Navigation.Mode.Explicit,
				selectOnLeft = navigation.selectOnLeft,
				selectOnRight = navigation.selectOnRight,
				selectOnUp = navigation.selectOnUp,
				selectOnDown = backButton
			};
			if ((bool)restoreSaveButton)
			{
				navigation.selectOnDown = restoreSaveButton;
			}
			fullSlotNav = navigation;
			navigation.selectOnDown = clearSaveButton.GetComponent<Selectable>();
			defeatedSlotNav = navigation;
		}

		private IEnumerator ValidateDeselect()
		{
			prevSelectedObject = EventSystem.current.currentSelectedGameObject;
			yield return new WaitForEndOfFrame();
			if (EventSystem.current.currentSelectedGameObject != null)
			{
				leftCursor.ResetTrigger(_showAnimProp);
				rightCursor.ResetTrigger(_showAnimProp);
				highlight.ResetTrigger(_showAnimProp);
				leftCursor.SetTrigger(_hideAnimProp);
				rightCursor.SetTrigger(_hideAnimProp);
				highlight.SetTrigger(_hideAnimProp);
				deselectWasForced = false;
				yield break;
			}
			if (deselectWasForced)
			{
				leftCursor.ResetTrigger(_showAnimProp);
				rightCursor.ResetTrigger(_showAnimProp);
				highlight.ResetTrigger(_showAnimProp);
				leftCursor.SetTrigger(_hideAnimProp);
				rightCursor.SetTrigger(_hideAnimProp);
				highlight.SetTrigger(_hideAnimProp);
				deselectWasForced = false;
				yield break;
			}
			if (!ManagerSingleton<InputHandler>.Instance.acceptingInput)
			{
				while (!ManagerSingleton<InputHandler>.Instance.acceptingInput)
				{
					yield return null;
				}
			}
			yield return null;
			if (EventSystem.current.currentSelectedGameObject != null)
			{
				leftCursor.ResetTrigger(_showAnimProp);
				rightCursor.ResetTrigger(_showAnimProp);
				highlight.ResetTrigger(_showAnimProp);
				leftCursor.SetTrigger(_hideAnimProp);
				rightCursor.SetTrigger(_hideAnimProp);
				highlight.SetTrigger(_hideAnimProp);
				deselectWasForced = false;
			}
			else if (deselectWasForced)
			{
				leftCursor.ResetTrigger(_showAnimProp);
				rightCursor.ResetTrigger(_showAnimProp);
				highlight.ResetTrigger(_showAnimProp);
				leftCursor.SetTrigger(_hideAnimProp);
				rightCursor.SetTrigger(_hideAnimProp);
				highlight.SetTrigger(_hideAnimProp);
				deselectWasForced = false;
			}
			else if (prevSelectedObject != null && prevSelectedObject.activeInHierarchy)
			{
				deselectWasForced = false;
				dontPlaySelectSound = true;
				EventSystem.current.SetSelectedGameObject(prevSelectedObject);
			}
		}
	}
}

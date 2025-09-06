using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using GlobalSettings;
using JetBrains.Annotations;
using TMProOld;
using TeamCherry.Localization;
using TeamCherry.NestedFadeGroup;
using UnityEngine;

public class QuestItemBoard : QuestItemManager
{
	private enum States
	{
		None = 0,
		DisplayQuests = 1,
		YesNo = 2,
		Empty = 3
	}

	[Header("Quest Board")]
	[SerializeField]
	private Vector2 worldPosition;

	[Space]
	[SerializeField]
	private NestedFadeGroup selectGroup;

	[SerializeField]
	private NestedFadeGroup yesNoGroup;

	[SerializeField]
	private UISelectionList yesNoList;

	[SerializeField]
	private QuestIconDisplay yesNoQuestDisplay;

	[SerializeField]
	private NestedFadeGroup emptyGroup;

	[SerializeField]
	private float groupFadeDuration;

	[SerializeField]
	private AudioEvent selectAudio;

	[Space]
	[SerializeField]
	private NestedFadeGroupBase submitPrompt;

	[SerializeField]
	private TMP_Text submitText;

	[SerializeField]
	private NestedFadeGroupBase cancelPrompt;

	[SerializeField]
	private LocalisedString takeLabel;

	[SerializeField]
	private LocalisedString donateLabel;

	[SerializeField]
	private LocalisedString confirmLabel;

	[Space]
	[SerializeField]
	private UISelectionListItem yesButton;

	[SerializeField]
	private LocalisedString notEnoughText;

	private List<BasicQuestBase> acceptedQuests;

	private States currentState;

	private bool isPromptOverTop;

	private FullQuestBase yesNoQuest;

	private FullQuestBase fullQuest;

	private Coroutine fadeStateRoutine;

	private InventoryPaneStandalone pane;

	public Func<IReadOnlyCollection<QuestGroupBase>> GetAvailableQuestsFunc;

	public int AvailableQuestsCount => GetItems().Count;

	public List<BasicQuestBase> Quests => GetItems();

	private bool InputBlocked
	{
		get
		{
			if (currentState == States.DisplayQuests)
			{
				return fadeStateRoutine != null;
			}
			return true;
		}
	}

	public event Action QuestAccepted;

	public event Action<List<BasicQuestBase>> BoardClosed;

	public event Action<FullQuestBase> DonateQuestAccepted;

	protected override void Awake()
	{
		base.Awake();
		pane = GetComponent<InventoryPaneStandalone>();
		if ((bool)pane)
		{
			pane.PaneOpenedAnimEnd += delegate
			{
				if ((bool)cursor)
				{
					cursor.Activate();
				}
				base.IsActionsBlocked = false;
				SetSelected(SelectedActionType.Default);
			};
			pane.PaneClosedAnimEnd += delegate
			{
				if (this.BoardClosed != null)
				{
					this.BoardClosed(acceptedQuests);
				}
				GameCameras.instance.HUDIn();
			};
		}
		base.transform.SetPosition2D(worldPosition);
		if ((bool)yesButton)
		{
			yesButton.InactiveConditionText = () => (!yesNoQuest || !yesNoQuest.CanComplete) ? ((string)notEnoughText) : string.Empty;
		}
	}

	protected override List<BasicQuestBase> GetItems()
	{
		if (GetAvailableQuestsFunc == null)
		{
			return new List<BasicQuestBase>();
		}
		return GetAvailableQuestsFunc().Cast<BasicQuestBase>().ToList();
	}

	protected override List<InventoryItemGrid.GridSection> GetGridSections(List<InventoryItemQuest> questItems, List<BasicQuestBase> quests)
	{
		for (int i = 0; i < questItems.Count; i++)
		{
			questItems[i].gameObject.SetActive(value: true);
			questItems[i].SetQuest(quests[i], isInCompletedSection: false);
		}
		if (!base.IsActionsBlocked && questItems.Count > 0)
		{
			InventoryItemQuest inventoryItemQuest = base.CurrentSelected as InventoryItemQuest;
			if (!inventoryItemQuest || !questItems.Contains(inventoryItemQuest))
			{
				SetSelected(questItems[questItems.Count - 1], null);
			}
		}
		return new List<InventoryItemGrid.GridSection>
		{
			new InventoryItemGrid.GridSection
			{
				Items = questItems.Cast<InventoryItemSelectableDirectional>().ToList()
			}
		};
	}

	protected override void OnItemInstantiated(InventoryItemQuest questItem)
	{
		questItem.Submitted += SubmitQuestSelection;
		questItem.Canceled += CancelQuestSelection;
	}

	public void OpenPane()
	{
		base.IsActionsBlocked = true;
		if ((bool)cursor)
		{
			cursor.Deactivate();
		}
		if ((bool)pane)
		{
			pane.PaneStart();
		}
		SetState((AvailableQuestsCount > 0) ? States.DisplayQuests : States.Empty);
		bool flag = currentState != States.Empty;
		if ((bool)selectGroup)
		{
			selectGroup.AlphaSelf = 1f;
			selectGroup.gameObject.SetActive(flag);
		}
		if ((bool)yesNoGroup)
		{
			yesNoGroup.AlphaSelf = 1f;
			yesNoGroup.gameObject.SetActive(value: false);
		}
		if ((bool)emptyGroup)
		{
			emptyGroup.AlphaSelf = 1f;
			emptyGroup.gameObject.SetActive(!flag);
		}
		if (acceptedQuests == null)
		{
			acceptedQuests = new List<BasicQuestBase>();
		}
		else
		{
			acceptedQuests.Clear();
		}
		UpdateButtonPrompts();
		SetDisplay((GameObject)null);
		GameCameras.instance.HUDOut();
	}

	public override bool MoveSelection(SelectionDirection direction)
	{
		if (InputBlocked)
		{
			return true;
		}
		return base.MoveSelection(direction);
	}

	private void SubmitQuestSelection(BasicQuestBase quest)
	{
		if (InputBlocked)
		{
			return;
		}
		acceptedQuests.Add(quest);
		yesNoQuest = quest as FullQuestBase;
		if (yesNoQuest != null)
		{
			if (yesNoQuest.IsDonateType)
			{
				DisplayCurrencyCounters(yesNoQuest);
				FadeToState(States.YesNo);
				if ((bool)yesNoQuestDisplay)
				{
					yesNoQuestDisplay.SetQuest(yesNoQuest);
				}
				selectAudio.SpawnAndPlayOneShot(Audio.DefaultUIAudioSourcePrefab, base.transform.position);
				return;
			}
			SetDisplay((GameObject)null);
			if ((bool)cursor)
			{
				cursor.Deactivate();
			}
			Action promptBlocker = GetPromptBlocker();
			promptBlocker = (Action)Delegate.Combine(promptBlocker, (Action)delegate
			{
				if ((bool)cursor)
				{
					cursor.Activate();
				}
				QuestActioned();
			});
			yesNoQuest.BeginQuest(promptBlocker);
		}
		else
		{
			quest.Get();
			QuestActioned();
		}
		this.QuestAccepted?.Invoke();
	}

	private void QuestActioned()
	{
		if (AvailableQuestsCount > 0)
		{
			UpdateList();
			SetSelected(SelectedActionType.Previous);
		}
		else
		{
			CloseBoard();
		}
	}

	private Action GetPromptBlocker()
	{
		base.IsActionsBlocked = true;
		InteractManager.IsDisabled = true;
		HeroController.instance.AddInputBlocker(this);
		isPromptOverTop = true;
		UpdateButtonPrompts();
		return delegate
		{
			base.IsActionsBlocked = false;
			InteractManager.IsDisabled = false;
			HeroController.instance.RemoveInputBlocker(this);
			isPromptOverTop = false;
			UpdateButtonPrompts();
		};
	}

	[UsedImplicitly]
	public void AcceptDonation()
	{
		CloseBoard();
		this.DonateQuestAccepted?.Invoke(yesNoQuest);
	}

	[UsedImplicitly]
	public void DeclineDonation()
	{
		if (AvailableQuestsCount > 0)
		{
			FadeToState(States.DisplayQuests);
		}
		HideCurrencyCounters(yesNoQuest);
		yesNoQuest = null;
	}

	private void CancelQuestSelection()
	{
		if (!InputBlocked)
		{
			CloseBoard();
		}
	}

	private void CloseBoard()
	{
		Quests.ForEach(delegate(BasicQuestBase quest)
		{
			quest.HasBeenSeen = true;
		});
		if ((bool)pane)
		{
			pane.PaneEnd();
		}
		base.IsActionsBlocked = true;
	}

	public override bool SubmitButtonSelected()
	{
		if (currentState == States.Empty)
		{
			CloseBoard();
			return true;
		}
		if (InputBlocked)
		{
			return false;
		}
		return base.SubmitButtonSelected();
	}

	public override bool CancelButtonSelected()
	{
		if (currentState == States.Empty)
		{
			CloseBoard();
			return true;
		}
		if (InputBlocked)
		{
			return false;
		}
		return base.CancelButtonSelected();
	}

	public override void SetDisplay(InventoryItemSelectable selectable)
	{
		base.SetDisplay(selectable);
		InventoryItemQuest inventoryItemQuest = selectable as InventoryItemQuest;
		if (!(inventoryItemQuest == null))
		{
			fullQuest = inventoryItemQuest.Quest as FullQuestBase;
			UpdateButtonPrompts();
		}
	}

	private void UpdateButtonPrompts()
	{
		if (currentState == States.DisplayQuests)
		{
			bool flag = fullQuest != null && fullQuest.IsDonateType;
			if ((bool)submitText)
			{
				submitText.text = (flag ? donateLabel : takeLabel);
			}
			if ((bool)submitPrompt)
			{
				submitPrompt.AlphaSelf = 1f;
			}
		}
		else
		{
			if ((bool)submitText)
			{
				submitText.text = confirmLabel;
			}
			if ((bool)submitPrompt)
			{
				submitPrompt.AlphaSelf = ((currentState == States.Empty) ? 0f : 1f);
			}
		}
		if (isPromptOverTop)
		{
			if ((bool)submitPrompt)
			{
				submitPrompt.AlphaSelf = 0f;
			}
			if ((bool)cancelPrompt)
			{
				cancelPrompt.AlphaSelf = 0f;
			}
		}
		else if ((bool)cancelPrompt)
		{
			cancelPrompt.AlphaSelf = 1f;
		}
	}

	private void SetState(States newState)
	{
		NestedFadeGroupBase groupForState = GetGroupForState(currentState);
		if ((bool)groupForState)
		{
			groupForState.gameObject.SetActive(value: false);
		}
		currentState = newState;
		NestedFadeGroupBase groupForState2 = GetGroupForState(currentState);
		if ((bool)groupForState2)
		{
			groupForState2.AlphaSelf = 1f;
			groupForState2.gameObject.SetActive(value: true);
		}
		UpdateButtonPrompts();
	}

	private void FadeToState(States newState)
	{
		if (fadeStateRoutine != null)
		{
			StopCoroutine(fadeStateRoutine);
		}
		fadeStateRoutine = StartCoroutine(FadeStateRoutine(newState));
	}

	private IEnumerator FadeStateRoutine(States newState)
	{
		base.IsActionsBlocked = true;
		if (newState == States.YesNo && (bool)yesNoList)
		{
			yesNoList.SetActive(value: false);
		}
		NestedFadeGroupBase previousGroup = GetGroupForState(currentState);
		if ((bool)previousGroup)
		{
			for (float elapsed = 0f; elapsed < groupFadeDuration; elapsed += Time.deltaTime)
			{
				float num = elapsed / groupFadeDuration;
				previousGroup.AlphaSelf = 1f - num;
				yield return null;
			}
			previousGroup.gameObject.SetActive(value: false);
		}
		currentState = newState;
		UpdateButtonPrompts();
		NestedFadeGroupBase currentGroup = GetGroupForState(currentState);
		if ((bool)currentGroup)
		{
			currentGroup.gameObject.SetActive(value: true);
			for (float elapsed = 0f; elapsed < groupFadeDuration; elapsed += Time.deltaTime)
			{
				float alphaSelf = elapsed / groupFadeDuration;
				currentGroup.AlphaSelf = alphaSelf;
				yield return null;
			}
		}
		if (newState == States.YesNo && (bool)yesNoList)
		{
			yesNoList.SetActive(value: true);
		}
		base.IsActionsBlocked = false;
		fadeStateRoutine = null;
	}

	private NestedFadeGroupBase GetGroupForState(States state)
	{
		return state switch
		{
			States.None => null, 
			States.DisplayQuests => selectGroup, 
			States.YesNo => yesNoGroup, 
			States.Empty => emptyGroup, 
			_ => throw new ArgumentOutOfRangeException(), 
		};
	}

	public void DisplayCurrencyCounters(FullQuestBase quest)
	{
		foreach (FullQuestBase.QuestTarget target in quest.Targets)
		{
			if (!target.Counter)
			{
				break;
			}
			QuestTargetCurrency questTargetCurrency = target.Counter as QuestTargetCurrency;
			if (questTargetCurrency != null)
			{
				CurrencyCounter.Show(questTargetCurrency.CurrencyType);
				continue;
			}
			CollectableItem collectableItem = target.Counter as CollectableItem;
			if (collectableItem != null)
			{
				ItemCurrencyCounter.Show(collectableItem);
				continue;
			}
			Debug.LogErrorFormat(this, "Could not get counter item for quest: {0}", quest.name);
		}
	}

	public void HideCurrencyCounters(FullQuestBase quest)
	{
		foreach (FullQuestBase.QuestTarget target in quest.Targets)
		{
			if (!target.Counter)
			{
				break;
			}
			QuestTargetCurrency questTargetCurrency = target.Counter as QuestTargetCurrency;
			if (questTargetCurrency != null)
			{
				CurrencyCounter.Hide(questTargetCurrency.CurrencyType);
				continue;
			}
			CollectableItem collectableItem = target.Counter as CollectableItem;
			if (collectableItem != null)
			{
				ItemCurrencyCounter.Hide(collectableItem);
				continue;
			}
			Debug.LogErrorFormat(this, "Could not get counter item for quest: {0}", quest.name);
		}
	}
}

using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class CollectableRelicBoard : InventoryItemListManager<CollectableRelicBoardItem, CollectableRelic>
{
	[Space]
	[SerializeField]
	private GameObject playPrompt;

	[SerializeField]
	private GameObject stopPrompt;

	[Space]
	[SerializeField]
	private float newItemAppearDelay;

	[SerializeField]
	private float newItemAppearOffset;

	[Space]
	[SerializeField]
	private GameObject nextActionButton;

	[SerializeField]
	private bool stopInputOnNewItems;

	private Coroutine newItemsAppearRoutine;

	private bool displayingNewItems;

	private InventoryPaneInput input;

	private InventoryPaneStandalone pane;

	private RelicBoardOwner owner;

	private CollectableRelic playingRelic;

	private List<CollectableRelicBoardItem> cachedSelectableItems;

	private readonly List<CollectableRelicBoardItem> newItems = new List<CollectableRelicBoardItem>();

	private bool didBlockInput;

	public event Action BoardClosed;

	protected override void Awake()
	{
		base.Awake();
		input = GetComponent<InventoryPaneInput>();
		pane = GetComponent<InventoryPaneStandalone>();
		if (!pane)
		{
			return;
		}
		input.enabled = false;
		pane.SkipInputEnable = true;
		pane.OnPaneStart += delegate
		{
			input.enabled = false;
			base.IsActionsBlocked = true;
		};
		pane.PaneOpenedAnimEnd += delegate
		{
			if (newItemsAppearRoutine == null)
			{
				input.enabled = true;
				if ((bool)cursor)
				{
					cursor.Activate();
				}
				base.IsActionsBlocked = false;
			}
			SetSelected(SelectedActionType.Default);
		};
		pane.PaneClosedAnimEnd += delegate
		{
			if (this.BoardClosed != null)
			{
				this.BoardClosed();
			}
			input.enabled = false;
			if ((bool)cursor)
			{
				cursor.Deactivate();
			}
			base.IsActionsBlocked = false;
		};
	}

	private void OnDisable()
	{
		if (didBlockInput)
		{
			BlockInput(state: false);
		}
		displayingNewItems = false;
	}

	protected override List<InventoryItemGrid.GridSection> GetGridSections(List<CollectableRelicBoardItem> selectableItems, List<CollectableRelic> items)
	{
		cachedSelectableItems = selectableItems;
		if (newItemsAppearRoutine != null)
		{
			StopCoroutine(newItemsAppearRoutine);
			newItemsAppearRoutine = null;
		}
		newItems.Clear();
		for (int i = 0; i < selectableItems.Count; i++)
		{
			CollectableRelicBoardItem collectableRelicBoardItem = selectableItems[i];
			collectableRelicBoardItem.gameObject.SetActive(value: true);
			collectableRelicBoardItem.Board = this;
			collectableRelicBoardItem.SetRelic(items[i], out var isNew);
			if (isNew)
			{
				newItems.Add(collectableRelicBoardItem);
			}
		}
		if (newItems.Count > 0)
		{
			displayingNewItems = true;
			nameText.text = "";
			descriptionText.text = "";
			if ((bool)playPrompt)
			{
				playPrompt.SetActive(value: false);
			}
			if ((bool)stopPrompt)
			{
				stopPrompt.SetActive(value: false);
			}
			if ((bool)nextActionButton)
			{
				nextActionButton.SetActive(value: false);
			}
			newItemsAppearRoutine = StartCoroutine(NewItemsAppear());
		}
		return new List<InventoryItemGrid.GridSection>
		{
			new InventoryItemGrid.GridSection
			{
				Items = selectableItems.Cast<InventoryItemSelectableDirectional>().ToList()
			}
		};
	}

	protected override List<CollectableRelic> GetItems()
	{
		if (owner == null)
		{
			return new List<CollectableRelic>();
		}
		if (PlayerData.instance.ConstructedFarsight)
		{
			return owner.GetRelics().ToList();
		}
		return owner.GetRelics().Where(delegate(CollectableRelic relic)
		{
			CollectableRelicsData.Data savedData = relic.SavedData;
			return savedData.IsCollected || savedData.IsDeposited;
		}).ToList();
	}

	public void OpenBoard(RelicBoardOwner setOwner)
	{
		owner = setOwner;
		playingRelic = owner.GetPlayingRelic();
		if ((bool)cursor)
		{
			cursor.Deactivate();
		}
		if ((bool)pane)
		{
			pane.PaneStart();
		}
		SetSelected(SelectedActionType.Default, justDisplay: true);
		GameCameras.instance.HUDOut();
	}

	public void CloseBoard()
	{
		if ((bool)pane)
		{
			pane.PaneEnd();
		}
	}

	protected override void OnItemInstantiated(CollectableRelicBoardItem item)
	{
		item.Canceled += CloseBoard;
	}

	public override void SetDisplay(GameObject selectedGameObject)
	{
		base.SetDisplay(selectedGameObject);
		if ((bool)playPrompt)
		{
			playPrompt.SetActive(value: false);
		}
		if ((bool)stopPrompt)
		{
			stopPrompt.SetActive(value: false);
		}
		if ((bool)nextActionButton)
		{
			nextActionButton.SetActive(value: true);
		}
	}

	public override void SetDisplay(InventoryItemSelectable selectable)
	{
		if (displayingNewItems && newItems.Contains(selectable))
		{
			return;
		}
		base.SetDisplay(selectable);
		CollectableRelicBoardItem collectableRelicBoardItem = selectable as CollectableRelicBoardItem;
		if (collectableRelicBoardItem == null)
		{
			return;
		}
		CollectableRelic relic = collectableRelicBoardItem.Relic;
		if (!relic)
		{
			return;
		}
		if ((bool)nextActionButton)
		{
			nextActionButton.SetActive(cachedSelectableItems.Count > 1);
		}
		if (!relic.SavedData.IsDeposited || !relic.IsPlayable)
		{
			return;
		}
		if (IsPlaying(collectableRelicBoardItem))
		{
			if ((bool)stopPrompt)
			{
				stopPrompt.SetActive(value: true);
			}
		}
		else if ((bool)playPrompt)
		{
			playPrompt.SetActive(value: true);
		}
	}

	public bool TryPlayRelic(CollectableRelicBoardItem relic)
	{
		if (!owner || !owner.HasGramaphone)
		{
			return false;
		}
		if ((bool)playingRelic)
		{
			owner.StopPlayingRelic();
			if (playingRelic == relic.Relic)
			{
				playingRelic = null;
				SetDisplay(relic);
				return true;
			}
		}
		playingRelic = relic.Relic;
		if (!relic)
		{
			return false;
		}
		bool flag = false;
		if ((bool)relic.Relic.GramaphoneClip)
		{
			owner.PlayOnGramaphone(relic.Relic);
			flag = true;
		}
		if (flag)
		{
			SetDisplay(relic);
		}
		return flag;
	}

	public bool IsPlaying(CollectableRelicBoardItem relic)
	{
		if (!playingRelic)
		{
			return false;
		}
		return relic.Relic == playingRelic;
	}

	public void UpdateRelicItemsIsPlaying()
	{
		foreach (CollectableRelicBoardItem cachedSelectableItem in cachedSelectableItems)
		{
			cachedSelectableItem.UpdatedIsPlaying();
		}
	}

	private IEnumerator NewItemsAppear()
	{
		base.IsActionsBlocked = true;
		if (stopInputOnNewItems)
		{
			BlockInput(state: true);
		}
		yield return new WaitForSeconds(newItemAppearDelay);
		foreach (CollectableRelicBoardItem newItem in newItems)
		{
			newItem.DoNewAppear();
			CollectableRelic relic = newItem.Relic;
			if ((bool)relic)
			{
				CollectableItemRelicType relicType = relic.RelicType;
				if ((bool)relicType && relicType.RewardAmount > 0)
				{
					CurrencyManager.AddCurrency(relicType.RewardAmount, CurrencyType.Money);
				}
			}
			yield return new WaitForSeconds(newItemAppearOffset);
		}
		displayingNewItems = false;
		base.IsActionsBlocked = false;
		if ((bool)cursor)
		{
			cursor.Activate();
		}
		input.enabled = true;
		if ((bool)base.CurrentSelected)
		{
			SetSelected(base.CurrentSelected, null);
		}
		if (stopInputOnNewItems)
		{
			BlockInput(state: false);
		}
		newItemsAppearRoutine = null;
	}

	private void BlockInput(bool state)
	{
		InventoryPaneInput.IsInputBlocked = (didBlockInput = state);
	}
}

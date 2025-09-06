using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMProOld;
using UnityEngine;

public class CollectionViewBoard : InventoryItemListManager<CollectionViewBoardItem, ICollectionViewerItem>
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
	private TMP_Text headingTemplate;

	[Space]
	[SerializeField]
	private GameObject nextActionButton;

	private bool isPaneOpenComplete;

	private Coroutine newItemsAppearRoutine;

	private readonly List<CollectionViewBoardItem> newItems = new List<CollectionViewBoardItem>();

	private bool displayingNewItems;

	private InventoryPaneInput input;

	private InventoryPaneStandalone pane;

	private CollectionViewerDesk owner;

	private CollectionViewBoardItem playingRelicItem;

	private List<CollectionViewBoardItem> cachedSelectableItems;

	private readonly List<ICollectionViewerItem> currentItemList = new List<ICollectionViewerItem>();

	private readonly Dictionary<ICollectionViewerItem, CollectionViewerDesk.Section> reverseSectionMap = new Dictionary<ICollectionViewerItem, CollectionViewerDesk.Section>();

	private readonly Dictionary<CollectionViewerDesk.Section, Transform> headingMap = new Dictionary<CollectionViewerDesk.Section, Transform>();

	private bool didBlockInput;

	protected override IEnumerable<InventoryItemSelectable> DefaultSelectables
	{
		get
		{
			if (newItems.Count <= 0)
			{
				return base.DefaultSelectables;
			}
			return newItems;
		}
	}

	public event Action BoardClosed;

	protected override void Awake()
	{
		base.Awake();
		headingTemplate.gameObject.SetActive(value: false);
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
			if (newItems.Count > 0)
			{
				float num = float.MaxValue;
				CollectionViewBoardItem item = null;
				foreach (CollectionViewBoardItem newItem in newItems)
				{
					float y = newItem.transform.localPosition.y;
					if (!(y > num))
					{
						num = y;
						item = newItem;
					}
				}
				base.ItemList.ScrollTo(item, isInstant: true);
			}
		};
		pane.PaneOpenedAnimEnd += delegate
		{
			isPaneOpenComplete = true;
			if (newItemsAppearRoutine == null)
			{
				base.IsActionsBlocked = false;
				input.enabled = true;
				if ((bool)cursor)
				{
					cursor.Activate();
				}
				SetSelected(SelectedActionType.Default);
			}
		};
		pane.PaneClosedAnimEnd += delegate
		{
			this.BoardClosed?.Invoke();
			input.enabled = false;
			GameCameras.instance.HUDIn();
			isPaneOpenComplete = false;
		};
	}

	private void OnDisable()
	{
		displayingNewItems = false;
	}

	protected override List<ICollectionViewerItem> GetItems()
	{
		currentItemList.Clear();
		if (!owner)
		{
			return currentItemList;
		}
		bool constructedFarsight = PlayerData.instance.ConstructedFarsight;
		foreach (CollectionViewerDesk.Section section in owner.Sections)
		{
			if (section.IsActive)
			{
				section.CheckIsListActive(!constructedFarsight, delegate(ICollectionViewerItem item)
				{
					currentItemList.Add(item);
					reverseSectionMap[item] = section;
				});
			}
		}
		return currentItemList;
	}

	protected override List<InventoryItemGrid.GridSection> GetGridSections(List<CollectionViewBoardItem> selectableItems, List<ICollectionViewerItem> items)
	{
		cachedSelectableItems = selectableItems;
		if (!owner)
		{
			return new List<InventoryItemGrid.GridSection>();
		}
		newItems.Clear();
		CollectableRelic playingRelic = owner.GetPlayingRelic();
		PlayerData instance = PlayerData.instance;
		for (int i = 0; i < selectableItems.Count; i++)
		{
			CollectionViewBoardItem collectionViewBoardItem = selectableItems[i];
			ICollectionViewerItem collectionViewerItem = items[i];
			if ((bool)playingRelic && collectionViewerItem.name == playingRelic.name)
			{
				playingRelicItem = collectionViewBoardItem;
			}
			collectionViewBoardItem.gameObject.SetActive(value: true);
			collectionViewBoardItem.Board = this;
			collectionViewBoardItem.SetItem(collectionViewerItem, out var isNew);
			if (isNew)
			{
				CollectableItem itemByName = CollectableItemManager.GetItemByName(collectionViewBoardItem.name);
				if (((object)itemByName == null || ((ICollectionViewerItem)itemByName).CanDeposit) && (bool)(collectionViewBoardItem.Item as CollectableItemMemento) && instance.Collectables.GetData(collectionViewBoardItem.name).Amount > 0)
				{
					newItems.Add(collectionViewBoardItem);
				}
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
		return (from selectable in selectableItems
			group selectable by reverseSectionMap[selectable.Item] into @group
			select new InventoryItemGrid.GridSection
			{
				Items = @group.Cast<InventoryItemSelectableDirectional>().ToList(),
				Header = GetHeadingForSection(@group.Key)
			}).ToList();
	}

	private Transform GetHeadingForSection(CollectionViewerDesk.Section section)
	{
		if (!headingMap.TryGetValue(section, out var value))
		{
			TMP_Text tMP_Text = UnityEngine.Object.Instantiate(headingTemplate, headingTemplate.transform.parent);
			tMP_Text.text = section.Heading;
			tMP_Text.gameObject.SetActive(value: true);
			value = tMP_Text.transform;
			headingMap[section] = value;
		}
		return value;
	}

	public void OpenBoard(CollectionViewerDesk setOwner)
	{
		base.IsActionsBlocked = true;
		owner = setOwner;
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

	private void CloseBoard()
	{
		base.IsActionsBlocked = true;
		if ((bool)cursor)
		{
			cursor.Deactivate();
		}
		if ((bool)pane)
		{
			pane.PaneEnd();
		}
	}

	protected override void OnItemInstantiated(CollectionViewBoardItem item)
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
		CollectionViewBoardItem collectionViewBoardItem = selectable as CollectionViewBoardItem;
		if (collectionViewBoardItem == null || collectionViewBoardItem.Item == null || !collectionViewBoardItem.Item.IsVisibleInCollection())
		{
			SetDisplay(selectable.gameObject);
			return;
		}
		base.SetDisplay(selectable);
		if ((bool)nextActionButton)
		{
			nextActionButton.SetActive(cachedSelectableItems.Count > 1 && (((IMoveNextButton)collectionViewBoardItem)?.WillSubmitMoveNext ?? false));
		}
		if (!CanPlayRelic(collectionViewBoardItem))
		{
			return;
		}
		if (IsPlaying(collectionViewBoardItem))
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

	public bool CanPlayRelic(CollectionViewBoardItem item)
	{
		CollectableRelic collectableRelic = item.Item as CollectableRelic;
		if (collectableRelic == null || !collectableRelic.IsPlayable)
		{
			return false;
		}
		if ((bool)owner)
		{
			return owner.HasGramaphone;
		}
		return false;
	}

	public bool TryPlayRelic(CollectionViewBoardItem item)
	{
		if (!owner || !owner.HasGramaphone)
		{
			return false;
		}
		if ((bool)playingRelicItem)
		{
			owner.StopPlayingRelic();
			if (playingRelicItem == item)
			{
				playingRelicItem = null;
				SetDisplay(item);
				return true;
			}
		}
		playingRelicItem = item;
		if (item == null)
		{
			return false;
		}
		CollectableRelic collectableRelic = item.Item as CollectableRelic;
		if (collectableRelic == null)
		{
			return false;
		}
		bool flag = false;
		if ((bool)collectableRelic.GramaphoneClip)
		{
			owner.PlayOnGramaphone(collectableRelic);
			flag = true;
		}
		if (flag)
		{
			SetDisplay(item);
		}
		return flag;
	}

	public bool IsPlaying(CollectionViewBoardItem relic)
	{
		if (!playingRelicItem)
		{
			return false;
		}
		return relic == playingRelicItem;
	}

	public void UpdateRelicItemsIsPlaying()
	{
		foreach (CollectionViewBoardItem cachedSelectableItem in cachedSelectableItems)
		{
			cachedSelectableItem.UpdatedIsPlaying();
		}
	}

	private IEnumerator NewItemsAppear()
	{
		base.IsActionsBlocked = true;
		BlockInput(state: true);
		yield return new WaitForSeconds(newItemAppearDelay);
		while (!isPaneOpenComplete)
		{
			yield return null;
		}
		PlayerData pd = PlayerData.instance;
		foreach (CollectionViewBoardItem newItem in newItems)
		{
			pd.MementosDeposited.SetData(newItem.name, new CollectableMementosData.Data
			{
				IsDeposited = true
			});
			pd.Collectables.SetData(newItem.name, default(CollectableItemsData.Data));
			CollectableItemManager.IncrementVersion();
			owner.DoMementoDeposit(newItem.name);
			newItem.DoNewAppear();
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
		newItems.Clear();
		BlockInput(state: false);
		newItemsAppearRoutine = null;
	}

	private void BlockInput(bool state)
	{
		InventoryPaneInput.IsInputBlocked = (didBlockInput = state);
	}
}

using System.Collections.Generic;
using System.Linq;
using GlobalSettings;
using TMProOld;
using TeamCherry.Localization;
using UnityEngine;
using UnityEngine.UI;

public class InventoryItemQuestManager : QuestItemManager, IInventoryPaneAvailabilityProvider
{
	[Header("Quest Inventory")]
	[SerializeField]
	private Transform currentHeading;

	[SerializeField]
	private Transform completedHeading;

	[Space]
	[SerializeField]
	private LayoutGroup itemListLayout;

	[SerializeField]
	private ScrollView itemListScrollView;

	[SerializeField]
	private InventoryItemMainQuest mainQuestTemplateItem;

	[Space]
	[SerializeField]
	private GameObject toggleCompletedParent;

	[SerializeField]
	private TMP_Text toggleCompletedText;

	[SerializeField]
	private TextMeshProContainerFitter toggleCompletedTextLayout;

	[SerializeField]
	private LocalisedString toggleCompletedOnText;

	[SerializeField]
	private LocalisedString toggleCompletedOffText;

	[SerializeField]
	private AudioEvent toggleAudio;

	private readonly List<BasicQuestBase> currentQuests = new List<BasicQuestBase>();

	private readonly List<BasicQuestBase> completedQuests = new List<BasicQuestBase>();

	private readonly List<BasicQuestBase> rumours = new List<BasicQuestBase>();

	private bool isPaneVisible;

	private bool isCompletedQuestsVisible;

	private List<InventoryItemMainQuest> spawnedMainQuestItems;

	private InputHandler inputHandler;

	private void Start()
	{
		inputHandler = ManagerSingleton<InputHandler>.Instance;
		InventoryPane component = GetComponent<InventoryPane>();
		component.OnPaneStart += delegate
		{
			isPaneVisible = true;
		};
		component.OnPaneEnd += delegate
		{
			isPaneVisible = false;
		};
		isPaneVisible = component.IsPaneActive;
		SetToggleCompletedText(toggleCompletedOnText);
	}

	private void Update()
	{
		if (isPaneVisible && !base.IsActionsBlocked && Platform.Current.GetMenuAction(inputHandler.inputActions) == Platform.MenuActions.Super && completedQuests.Count > 0)
		{
			isCompletedQuestsVisible = !isCompletedQuestsVisible;
			UpdateList();
			SetToggleCompletedText(isCompletedQuestsVisible ? toggleCompletedOffText : toggleCompletedOnText);
			toggleAudio.SpawnAndPlayOneShot(Audio.DefaultUIAudioSourcePrefab, base.transform.position);
		}
	}

	private void SetToggleCompletedText(string text)
	{
		if ((bool)toggleCompletedText)
		{
			toggleCompletedText.text = text;
		}
		if ((bool)toggleCompletedTextLayout)
		{
			toggleCompletedTextLayout.SetLayoutAll();
		}
	}

	public bool IsAvailable()
	{
		return QuestManager.IsAnyQuestVisible();
	}

	private static IEnumerable<BasicQuestBase> EnumerateSubQuests(BasicQuestBase quest)
	{
		yield return quest;
		MainQuest mainQuest = quest as MainQuest;
		if (mainQuest == null)
		{
			yield break;
		}
		bool mainQuestComplete = mainQuest.IsCompleted;
		foreach (SubQuest subQuest in mainQuest.SubQuests)
		{
			SubQuest current = subQuest.GetCurrent();
			if (!mainQuestComplete || current.IsLinkedBoolComplete)
			{
				yield return current;
			}
		}
	}

	protected override List<BasicQuestBase> GetItems()
	{
		return (from quest in (from quest in QuestManager.GetAcceptedQuests()
				where !IsInMainQuestSection(quest)
				select quest).SelectMany(EnumerateSubQuests)
			where !quest.IsHidden
			select quest).ToList();
	}

	protected override List<InventoryItemGrid.GridSection> GetGridSections(List<InventoryItemQuest> questItems, List<BasicQuestBase> quests)
	{
		currentQuests.Clear();
		completedQuests.Clear();
		rumours.Clear();
		foreach (BasicQuestBase quest in quests)
		{
			if (quest is IQuestWithCompletion questWithCompletion)
			{
				if (questWithCompletion.IsCompleted)
				{
					completedQuests.Add(quest);
				}
				else
				{
					currentQuests.Add(quest);
				}
			}
			else
			{
				rumours.Add(quest);
			}
		}
		currentQuests.AddRange(rumours);
		List<InventoryItemQuest> range = questItems.GetRange(0, currentQuests.Count);
		for (int i = 0; i < range.Count; i++)
		{
			BasicQuestBase basicQuestBase = currentQuests[i];
			InventoryItemQuest inventoryItemQuest = range[i];
			inventoryItemQuest.gameObject.SetActive(value: true);
			inventoryItemQuest.SetQuest(basicQuestBase, isInCompletedSection: false);
			inventoryItemQuest.gameObject.name = "Quest (" + basicQuestBase.name + ")";
		}
		List<InventoryItemQuest> range2 = questItems.GetRange(currentQuests.Count, completedQuests.Count);
		if (isCompletedQuestsVisible)
		{
			for (int j = 0; j < range2.Count; j++)
			{
				BasicQuestBase basicQuestBase2 = completedQuests[j];
				InventoryItemQuest inventoryItemQuest2 = range2[j];
				inventoryItemQuest2.gameObject.SetActive(value: true);
				inventoryItemQuest2.SetQuest(basicQuestBase2, isInCompletedSection: true);
				inventoryItemQuest2.gameObject.name = "Completed Quest (" + basicQuestBase2.name + ")";
			}
		}
		else
		{
			bool flag = false;
			foreach (InventoryItemQuest item in range2)
			{
				item.gameObject.SetActive(value: false);
				if (!flag && base.CurrentSelected == item)
				{
					flag = true;
				}
			}
			if (flag)
			{
				bool flag2 = false;
				if (range.Count > 0)
				{
					for (int k = 1; k <= range.Count; k++)
					{
						int num = k;
						InventoryItemQuest inventoryItemQuest3 = range[range.Count - num];
						if (inventoryItemQuest3.gameObject.activeSelf)
						{
							SetSelected(inventoryItemQuest3, null);
							flag2 = true;
							break;
						}
					}
				}
				if (!flag2)
				{
					SetSelected(SelectedActionType.Default);
				}
			}
		}
		return new List<InventoryItemGrid.GridSection>
		{
			new InventoryItemGrid.GridSection
			{
				Header = currentHeading,
				Items = range.Cast<InventoryItemSelectableDirectional>().ToList()
			},
			new InventoryItemGrid.GridSection
			{
				Header = completedHeading,
				Items = range2.Cast<InventoryItemSelectableDirectional>().ToList()
			}
		};
	}

	protected override InventoryItemSelectable GetStartSelectable()
	{
		if (!QuestManager.UpdatedQuest)
		{
			return base.GetStartSelectable();
		}
		InventoryItemQuest inventoryItemQuest = GetSelectables(null).FirstOrDefault((InventoryItemQuest quest) => QuestManager.UpdatedQuest == quest.Quest);
		QuestManager.UpdatedQuest = null;
		if ((bool)inventoryItemQuest && inventoryItemQuest.gameObject.activeSelf)
		{
			return inventoryItemQuest;
		}
		return base.GetStartSelectable();
	}

	private bool IsInMainQuestSection(BasicQuestBase quest)
	{
		MainQuest mainQuest = quest as MainQuest;
		if (mainQuest == null)
		{
			return false;
		}
		return !mainQuest.IsCompleted;
	}

	protected override void OnItemListSetup()
	{
		List<BasicQuestBase> list = (from quest in QuestManager.GetAcceptedQuests().Where(IsInMainQuestSection)
			where !quest.IsHidden
			select quest).ToList();
		mainQuestTemplateItem.gameObject.SetActive(value: false);
		if (spawnedMainQuestItems == null)
		{
			spawnedMainQuestItems = new List<InventoryItemMainQuest>(list.Count);
		}
		for (int num = list.Count - spawnedMainQuestItems.Count; num > 0; num--)
		{
			InventoryItemMainQuest item = Object.Instantiate(mainQuestTemplateItem, mainQuestTemplateItem.transform.parent);
			spawnedMainQuestItems.Add(item);
			item.OnSelected += delegate(InventoryItemSelectable selectable)
			{
				base.ItemList.ScrollTo(selectable);
			};
			item.OnSubSelected += delegate
			{
				base.ItemList.ScrollTo(item);
			};
		}
		for (int i = 0; i < list.Count; i++)
		{
			BasicQuestBase basicQuestBase = list[i];
			InventoryItemMainQuest inventoryItemMainQuest = spawnedMainQuestItems[i];
			inventoryItemMainQuest.gameObject.SetActive(value: true);
			inventoryItemMainQuest.SetQuest(basicQuestBase, isInCompletedSection: false);
			inventoryItemMainQuest.gameObject.name = "Main Quest (" + basicQuestBase.name + ")";
			if (i < list.Count - 1)
			{
				InventoryItemMainQuest inventoryItemMainQuest2 = spawnedMainQuestItems[i + 1];
				inventoryItemMainQuest.Selectables[1] = inventoryItemMainQuest2;
				inventoryItemMainQuest2.Selectables[0] = inventoryItemMainQuest;
			}
			else
			{
				InventoryItemGrid.LinkVertical(inventoryItemMainQuest, base.ItemList);
				inventoryItemMainQuest.Selectables[1] = base.ItemList.GetFirst();
			}
		}
		for (int j = list.Count; j < spawnedMainQuestItems.Count; j++)
		{
			spawnedMainQuestItems[j].gameObject.SetActive(value: false);
		}
		if (list.Count <= 0)
		{
			currentHeading.gameObject.SetActive(value: false);
		}
		itemListLayout.ForceUpdateLayoutNoCanvas();
		itemListScrollView.FullUpdate();
		toggleCompletedParent.SetActive(completedQuests.Count > 0);
	}

	protected override string FormatDisplayName(string displayName)
	{
		return displayName.Replace("\n", "");
	}
}

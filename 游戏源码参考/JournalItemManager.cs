using System;
using System.Collections.Generic;
using System.Linq;
using GlobalSettings;
using TMProOld;
using TeamCherry.Localization;
using TeamCherry.NestedFadeGroup;
using UnityEngine;

public class JournalItemManager : InventoryItemListManager<JournalEntryItem, EnemyJournalRecord>, IInventoryPaneAvailabilityProvider
{
	[Serializable]
	private class CompletionText
	{
		[SerializeField]
		private TextMeshPro countText;

		private string initialCountText;

		[SerializeField]
		private TextMeshPro totalText;

		private string initialTotalText;

		public void SetValues(int count, int total)
		{
			SetFormattedText(countText, ref initialCountText, count);
			SetFormattedText(totalText, ref initialTotalText, total);
		}

		private void SetFormattedText(TextMeshPro currentText, ref string initialText, int num)
		{
			if ((bool)currentText)
			{
				if (string.IsNullOrEmpty(initialText))
				{
					initialText = currentText.text;
				}
				currentText.text = string.Format(initialText, num);
			}
		}
	}

	[SerializeField]
	private NestedFadeGroupBase notesGroup;

	[SerializeField]
	private TextMeshPro notesText;

	[SerializeField]
	private LocalisedString notesLockedText;

	[SerializeField]
	private SpriteRenderer enemySprite;

	[SerializeField]
	private GameObject completionParent;

	[SerializeField]
	private CompletionText encounteredText;

	[SerializeField]
	private CompletionText completedText;

	public bool IsAvailable()
	{
		if (CollectableItemManager.IsInHiddenMode())
		{
			return false;
		}
		return EnemyJournalManager.GetKilledEnemies().Count > 0;
	}

	protected override List<EnemyJournalRecord> GetItems()
	{
		if (!PlayerData.instance.ConstructedFarsight)
		{
			return EnemyJournalManager.GetKilledEnemies();
		}
		return EnemyJournalManager.GetRequiredEnemies();
	}

	protected override List<InventoryItemGrid.GridSection> GetGridSections(List<JournalEntryItem> selectableItems, List<EnemyJournalRecord> items)
	{
		for (int i = 0; i < items.Count; i++)
		{
			selectableItems[i].gameObject.SetActive(value: true);
			selectableItems[i].Setup(items[i]);
		}
		if (Application.isPlaying)
		{
			if (PlayerData.instance.ConstructedFarsight)
			{
				completionParent.SetActive(value: true);
				int count = items.Count;
				int count2 = items.Count((EnemyJournalRecord record) => record.IsVisible);
				int count3 = items.Count((EnemyJournalRecord record) => record.KillCount >= record.KillsRequired);
				encounteredText.SetValues(count2, count);
				completedText.SetValues(count3, count);
			}
			else
			{
				completionParent.SetActive(value: false);
			}
		}
		return new List<InventoryItemGrid.GridSection>
		{
			new InventoryItemGrid.GridSection
			{
				Items = selectableItems.Cast<InventoryItemSelectableDirectional>().ToList()
			}
		};
	}

	public override void SetDisplay(GameObject selectedGameObject)
	{
		base.SetDisplay(selectedGameObject);
		if ((bool)notesGroup)
		{
			notesGroup.AlphaSelf = 0f;
		}
		if ((bool)notesText)
		{
			notesText.text = string.Empty;
		}
		if ((bool)enemySprite)
		{
			enemySprite.sprite = null;
		}
	}

	public override void SetDisplay(InventoryItemSelectable selectable)
	{
		base.SetDisplay(selectable);
		JournalEntryItem journalEntryItem = selectable as JournalEntryItem;
		if (!(journalEntryItem == null) && journalEntryItem.Record.IsVisible)
		{
			int killCount = journalEntryItem.Record.KillCount;
			int killsRequired = journalEntryItem.Record.KillsRequired;
			bool flag = killCount >= killsRequired;
			if ((bool)notesText)
			{
				notesText.text = (flag ? ((string)journalEntryItem.Record.Notes) : string.Format(notesLockedText, killsRequired - killCount));
			}
			if ((bool)notesGroup)
			{
				notesGroup.AlphaSelf = (flag ? 1f : UI.DisabledUiTextColor.r);
			}
			if ((bool)enemySprite)
			{
				enemySprite.sprite = journalEntryItem.Record.EnemySprite;
			}
		}
	}

	public override void InstantScroll()
	{
		EnemyJournalRecord updatedRecord = EnemyJournalManager.UpdatedRecord;
		if (updatedRecord == null)
		{
			base.InstantScroll();
			return;
		}
		InventoryItemSelectable startSelectable = GetStartSelectable();
		EnemyJournalManager.UpdatedRecord = updatedRecord;
		base.ItemList.ScrollTo(startSelectable, isInstant: true);
	}

	protected override InventoryItemSelectable GetStartSelectable()
	{
		if (!EnemyJournalManager.UpdatedRecord)
		{
			return base.GetStartSelectable();
		}
		JournalEntryItem journalEntryItem = GetSelectables(null).FirstOrDefault((JournalEntryItem entry) => entry.Record == EnemyJournalManager.UpdatedRecord);
		EnemyJournalManager.UpdatedRecord = null;
		if (!journalEntryItem)
		{
			return base.GetStartSelectable();
		}
		return journalEntryItem;
	}

	public override bool MoveSelectionPage(SelectionDirection direction)
	{
		return false;
	}
}

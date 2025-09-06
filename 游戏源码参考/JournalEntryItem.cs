using System;
using UnityEngine;

public class JournalEntryItem : InventoryItemUpdateable, InventoryItemListManager<JournalEntryItem, EnemyJournalRecord>.IMoveNextButton
{
	[Space]
	[SerializeField]
	private GameObject emptyIcon;

	[SerializeField]
	private GameObject standardFrame;

	[SerializeField]
	private GameObject completeFrame;

	[SerializeField]
	private SpriteRenderer iconSprite;

	private Sprite initialSprite;

	public EnemyJournalRecord Record { get; private set; }

	public override string DisplayName
	{
		get
		{
			if (!Record.IsVisible)
			{
				return null;
			}
			return Record.DisplayName;
		}
	}

	public override string Description
	{
		get
		{
			if (!Record.IsVisible)
			{
				return null;
			}
			return Record.Description;
		}
	}

	protected override bool IsSeen
	{
		get
		{
			if (Record.IsVisible)
			{
				return Record.SeenInJournal;
			}
			return true;
		}
		set
		{
			if (Record.IsVisible)
			{
				if (!value)
				{
					throw new NotImplementedException();
				}
				EnemyJournalManager.SetJournalSeen(Record);
			}
		}
	}

	public bool WillSubmitMoveNext => true;

	public void Setup(EnemyJournalRecord record)
	{
		Record = record;
		base.gameObject.name = "Journal Entry (" + record.name + ")";
		bool flag = !record.IsVisible;
		if ((bool)emptyIcon)
		{
			emptyIcon.SetActive(flag);
		}
		if ((bool)iconSprite)
		{
			if (initialSprite == null)
			{
				initialSprite = iconSprite.sprite;
			}
			if (flag)
			{
				iconSprite.gameObject.SetActive(value: false);
			}
			else
			{
				iconSprite.gameObject.SetActive(value: true);
				iconSprite.sprite = (record.IconSprite ? record.IconSprite : initialSprite);
			}
		}
		bool flag2 = !flag && record.KillCount >= record.KillsRequired;
		if ((bool)completeFrame)
		{
			completeFrame.SetActive(!flag && flag2);
		}
		if ((bool)standardFrame)
		{
			standardFrame.SetActive(!flag && !flag2);
		}
		UpdateDisplay();
	}
}

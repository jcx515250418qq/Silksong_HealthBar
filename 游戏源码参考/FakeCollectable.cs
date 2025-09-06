using System;
using TeamCherry.Localization;
using UnityEngine;
using UnityEngine.Serialization;

public class FakeCollectable : QuestTargetCounter
{
	private enum UpgradeIconTypes
	{
		None = 0,
		Always = 1,
		AfterFirst = 2
	}

	[SerializeField]
	[LocalisedString.NotRequired]
	private LocalisedString uiMsgName;

	[SerializeField]
	[LocalisedString.NotRequired]
	private LocalisedString uiMsgNameAlt;

	[FormerlySerializedAs("uiMsgSprite")]
	[SerializeField]
	private Sprite icon;

	[SerializeField]
	private SavedItem setItemUpdated;

	[SerializeField]
	private bool showItemGetEffects = true;

	[Space]
	[SerializeField]
	private UpgradeIconTypes showUpgradeIcon;

	public override bool CanGetMore()
	{
		return true;
	}

	public override void Get(bool showPopup = true)
	{
		if (!uiMsgName.IsEmpty || !uiMsgNameAlt.IsEmpty)
		{
			if (showPopup || showUpgradeIcon != 0)
			{
				CollectableUIMsg.Spawn(this);
			}
			if (showItemGetEffects)
			{
				CollectableItemHeroReaction.DoReaction();
			}
		}
		SetItemUpdated(showPopup);
	}

	protected void SetItemUpdated(bool showPopup = true)
	{
		if ((bool)setItemUpdated)
		{
			setItemUpdated.SetHasNew(showPopup);
		}
	}

	public override Sprite GetPopupIcon()
	{
		return icon;
	}

	public override string GetPopupName()
	{
		if (!uiMsgNameAlt.IsEmpty && HasUpgradeIcon())
		{
			return uiMsgNameAlt;
		}
		return uiMsgName;
	}

	public override bool HasUpgradeIcon()
	{
		return showUpgradeIcon switch
		{
			UpgradeIconTypes.None => false, 
			UpgradeIconTypes.Always => true, 
			UpgradeIconTypes.AfterFirst => GetSavedAmount() > 0, 
			_ => throw new ArgumentOutOfRangeException(), 
		};
	}
}

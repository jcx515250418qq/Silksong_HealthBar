using System;
using System.Collections;
using GlobalSettings;
using TMProOld;
using TeamCherry.Localization;
using UnityEngine;
using UnityEngine.UI;

public class CollectableUIMsg : UIMsgPopupBase<ICollectableUIMsgItem, CollectableUIMsg>
{
	[Space]
	[SerializeField]
	private SpriteRenderer icon;

	[SerializeField]
	private SpriteRenderer upgradeIcon;

	[SerializeField]
	private TextMeshPro nameText;

	[SerializeField]
	private LayoutGroup layoutGroup;

	private Coroutine replaceRoutine;

	public static CollectableUIMsg Spawn(ICollectableUIMsgItem item, CollectableUIMsg replacing = null, bool forceReplacingEffect = false)
	{
		return Spawn(item, Color.white, replacing, forceReplacingEffect);
	}

	public static CollectableUIMsg Spawn(ICollectableUIMsgItem item, Color textColor, CollectableUIMsg replacing = null, bool forceReplacingEffect = false)
	{
		CollectableUIMsg collectableUIMsgPrefab = UI.CollectableUIMsgPrefab;
		if (!collectableUIMsgPrefab)
		{
			return null;
		}
		CollectableUIMsg collectableUIMsg = UIMsgPopupBase<ICollectableUIMsgItem, CollectableUIMsg>.SpawnInternal(collectableUIMsgPrefab, item, replacing, forceReplacingEffect);
		if (!collectableUIMsg)
		{
			return null;
		}
		if ((bool)collectableUIMsg.nameText)
		{
			collectableUIMsg.nameText.color = textColor;
		}
		return collectableUIMsg;
	}

	public static void ShowTakeMsg(ICollectableUIMsgItem item, TakeItemTypes takeItemType)
	{
		LocalisedString localisedString;
		switch (takeItemType)
		{
		case TakeItemTypes.Silent:
			return;
		case TakeItemTypes.Taken:
			localisedString = UI.ItemTakenPopup;
			break;
		case TakeItemTypes.Given:
			localisedString = UI.ItemGivenPopup;
			break;
		case TakeItemTypes.Deposited:
			localisedString = UI.ItemDepositedPopup;
			break;
		default:
			throw new ArgumentOutOfRangeException("takeItemType", takeItemType, null);
		}
		string text = localisedString;
		string text2;
		Sprite uIMsgSprite;
		if (item is CollectableItem collectableItem)
		{
			text2 = collectableItem.GetDisplayName(CollectableItem.ReadSource.TakePopup);
			uIMsgSprite = collectableItem.GetIcon(CollectableItem.ReadSource.TakePopup);
		}
		else
		{
			text2 = item.GetUIMsgName();
			uIMsgSprite = item.GetUIMsgSprite();
		}
		text.TryFormat(out var outText, text2);
		UIMsgDisplay uIMsgDisplay = default(UIMsgDisplay);
		uIMsgDisplay.Name = outText;
		uIMsgDisplay.Icon = uIMsgSprite;
		uIMsgDisplay.IconScale = item.GetUIMsgIconScale();
		uIMsgDisplay.RepresentingObject = item.GetRepresentingObject();
		Spawn(uIMsgDisplay);
	}

	protected override void UpdateDisplay(ICollectableUIMsgItem item)
	{
		if (item == null)
		{
			Debug.LogError("item was null", this);
			return;
		}
		if ((bool)icon)
		{
			icon.sprite = item.GetUIMsgSprite();
			icon.transform.localScale = Vector3.one * item.GetUIMsgIconScale();
		}
		if ((bool)nameText)
		{
			nameText.text = item.GetUIMsgName().ToSingleLine();
		}
		if ((bool)layoutGroup)
		{
			layoutGroup.ForceUpdateLayoutNoCanvas();
		}
		if ((bool)upgradeIcon)
		{
			upgradeIcon.gameObject.SetActive(item.HasUpgradeIcon());
		}
	}

	public void Replace(float delay, ICollectableUIMsgItem item)
	{
		if (replaceRoutine != null)
		{
			StopCoroutine(replaceRoutine);
		}
		replaceRoutine = StartCoroutine(ReplaceDelayed(delay, item));
	}

	private IEnumerator ReplaceDelayed(float delay, ICollectableUIMsgItem item)
	{
		yield return new WaitForSeconds(delay);
		Spawn(item, this, forceReplacingEffect: true);
		replaceRoutine = null;
	}
}

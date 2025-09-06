using System;
using System.Linq;
using System.Text;
using TeamCherry.Localization;
using TeamCherry.SharedUtils;
using UnityEngine;

[CreateAssetMenu(menuName = "Hornet/Collectable Items/Collectable Item (PlayerData Stack)")]
public class CollectableItemPlayerDataStack : CollectableItemStack
{
	[Serializable]
	private class StackItemInfo
	{
		[PlayerDataField(typeof(bool), false)]
		public string PlayerDataBool;

		[LocalisedString.NotRequired]
		public LocalisedString Name;

		public bool IsUnlocked
		{
			get
			{
				if (Name.IsEmpty)
				{
					return false;
				}
				if (!string.IsNullOrEmpty(PlayerDataBool))
				{
					return PlayerData.instance.GetVariable<bool>(PlayerDataBool);
				}
				return true;
			}
		}
	}

	[Space]
	[SerializeField]
	[LocalisedString.NotRequired]
	private LocalisedString stackDescHeader;

	[SerializeField]
	[TextArea]
	private string stackItemListFormat;

	[SerializeField]
	private StackItemInfo[] stackItems;

	public override int CollectedAmount => stackItems.Count((StackItemInfo station) => station.IsUnlocked);

	public override string GetDisplayName(ReadSource readSource)
	{
		PlayerData instance = PlayerData.instance;
		if (readSource == ReadSource.GetPopup || readSource == ReadSource.TakePopup)
		{
			StackItemInfo[] array = stackItems;
			foreach (StackItemInfo stackItemInfo in array)
			{
				if (!string.IsNullOrEmpty(stackItemInfo.PlayerDataBool) && !string.IsNullOrEmpty(instance.LastSetFieldName) && instance.LastSetFieldName == stackItemInfo.PlayerDataBool)
				{
					return stackItemInfo.Name;
				}
			}
		}
		return base.GetDisplayName(readSource);
	}

	public override string GetDescription(ReadSource readSource)
	{
		StringBuilder tempStringBuilder = Helper.GetTempStringBuilder(base.GetDescription(readSource));
		if (CollectedAmount > 1)
		{
			tempStringBuilder.AppendLine();
			if (!stackDescHeader.IsEmpty)
			{
				tempStringBuilder.AppendLine();
				tempStringBuilder.Append(stackDescHeader);
			}
			StackItemInfo[] array = stackItems;
			foreach (StackItemInfo stackItemInfo in array)
			{
				if (!stackItemInfo.Name.IsEmpty && stackItemInfo.IsUnlocked)
				{
					tempStringBuilder.AppendLine();
					tempStringBuilder.AppendFormat(stackItemListFormat, stackItemInfo.Name);
				}
			}
		}
		return tempStringBuilder.ToString();
	}
}

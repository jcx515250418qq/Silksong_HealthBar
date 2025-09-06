using System;
using System.Collections.Generic;
using TeamCherry.Localization;
using TeamCherry.SharedUtils;
using UnityEngine;

public class CaravanTroupeHunter : SimpleShopMenuOwner
{
	public enum PinGroups
	{
		None = -1,
		Marrowlands = 0,
		Midlands = 1,
		Blastedlands = 2,
		Citadel = 3,
		Peaklands = 4,
		Mucklands = 5
	}

	[Serializable]
	public class ShopItemInfo : ISimpleShopItem
	{
		public LocalisedString DisplayName;

		public CostReference Cost;

		public string GetDisplayName()
		{
			return DisplayName;
		}

		public Sprite GetIcon()
		{
			return null;
		}

		public int GetCost()
		{
			return Cost.Value;
		}

		public bool DelayPurchase()
		{
			return false;
		}
	}

	[SerializeField]
	[ArrayForEnum(typeof(PinGroups))]
	private ShopItemInfo[] shopItems;

	private List<ISimpleShopItem> currentList;

	private List<PinGroups> currentListGroups;

	public static IReadOnlyDictionary<PinGroups, string> PdBools { get; } = new Dictionary<PinGroups, string>
	{
		{
			PinGroups.Marrowlands,
			"hasPinFleaMarrowlands"
		},
		{
			PinGroups.Midlands,
			"hasPinFleaMidlands"
		},
		{
			PinGroups.Blastedlands,
			"hasPinFleaBlastedlands"
		},
		{
			PinGroups.Citadel,
			"hasPinFleaCitadel"
		},
		{
			PinGroups.Peaklands,
			"hasPinFleaPeaklands"
		},
		{
			PinGroups.Mucklands,
			"hasPinFleaMucklands"
		}
	};

	public override bool ClosePaneOnPurchase => false;

	private void OnValidate()
	{
		ArrayForEnumAttribute.EnsureArraySize(ref shopItems, typeof(PinGroups));
	}

	private void Awake()
	{
		OnValidate();
		base.ClosedPurchase += delegate
		{
			GameManager.instance.UpdateGameMapWithPopup();
		};
	}

	protected override List<ISimpleShopItem> GetItems()
	{
		PlayerData instance = PlayerData.instance;
		if (currentList == null)
		{
			currentList = new List<ISimpleShopItem>();
		}
		else
		{
			currentList.Clear();
		}
		if (currentListGroups == null)
		{
			currentListGroups = new List<PinGroups>();
		}
		else
		{
			currentListGroups.Clear();
		}
		GameMap gameMap = GameManager.instance.gameMap;
		for (int i = 0; i < shopItems.Length; i++)
		{
			PinGroups pinGroups = (PinGroups)i;
			ShopItemInfo item = shopItems[i];
			if (!instance.GetVariable<bool>(PdBools[pinGroups]) && gameMap.HasRemainingPinFor(pinGroups))
			{
				currentList.Add(item);
				currentListGroups.Add(pinGroups);
			}
		}
		return currentList;
	}

	protected override void OnPurchasedItem(int itemIndex)
	{
		if (itemIndex < 0 || itemIndex >= currentListGroups.Count)
		{
			Debug.LogError("Can't handle purchase for itemIndex out of range: " + itemIndex, this);
		}
		else
		{
			PlayerData.instance.SetVariable(PdBools[currentListGroups[itemIndex]], value: true);
		}
	}
}

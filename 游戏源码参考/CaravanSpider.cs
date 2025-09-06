using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using TeamCherry.Localization;
using UnityEngine;

public class CaravanSpider : SimpleShopMenuOwner
{
	[Serializable]
	public class TravelLocationInfo : ISimpleShopItem
	{
		public LocalisedString DisplayName;

		public string TargetScene;

		[Range(0f, 360f)]
		public float DirectionTo;

		public PlayerDataTest AppearCondition;

		public CostReference Cost;

		public CostReference AltCost;

		public PlayerDataTest AltCostCondition;

		public bool DelayAltPurchase;

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
			CostReference costReference = ((!AltCostCondition.IsDefined || !AltCostCondition.IsFulfilled) ? Cost : AltCost);
			if (!costReference)
			{
				return 0;
			}
			return costReference.Value;
		}

		public bool DelayPurchase()
		{
			if (!AltCostCondition.IsDefined)
			{
				return false;
			}
			if (AltCostCondition.IsFulfilled)
			{
				return DelayAltPurchase;
			}
			return false;
		}
	}

	[Serializable]
	public class TravelLocations
	{
		public TravelLocationInfo Bonetown;

		public TravelLocationInfo Wilds;

		public TravelLocationInfo Belltown;

		public TravelLocationInfo Coral;
	}

	[SerializeField]
	private TravelLocations travelLocations;

	private List<TravelLocationInfo> travelLocationsOrdered;

	private bool isSceneLocked;

	protected override void Start()
	{
		base.Start();
		bool flag = isSceneLocked;
		isSceneLocked = false;
		string sceneNameString = GameManager.instance.GetSceneNameString();
		travelLocationsOrdered = new List<TravelLocationInfo>();
		if (!string.IsNullOrEmpty(sceneNameString))
		{
			FieldInfo[] fields = travelLocations.GetType().GetFields(BindingFlags.Instance | BindingFlags.Public);
			foreach (FieldInfo fieldInfo in fields)
			{
				if (!(fieldInfo.FieldType != typeof(TravelLocationInfo)))
				{
					TravelLocationInfo location = (TravelLocationInfo)fieldInfo.GetValue(travelLocations);
					SetupTravelLocation(location, sceneNameString);
				}
			}
		}
		if (isSceneLocked)
		{
			base.gameObject.SetActive(value: false);
		}
		else if (flag)
		{
			FSMUtility.SendEventToGameObject(base.gameObject, "JUST UNLOCKED");
		}
	}

	private void SetupTravelLocation(TravelLocationInfo location, string currentSceneName)
	{
		if (location.TargetScene == currentSceneName)
		{
			if (!location.AppearCondition.IsFulfilled)
			{
				isSceneLocked = true;
			}
		}
		else if (location.AppearCondition.IsFulfilled)
		{
			travelLocationsOrdered.Add(location);
		}
	}

	protected override List<ISimpleShopItem> GetItems()
	{
		return travelLocationsOrdered.Cast<ISimpleShopItem>().ToList();
	}

	protected override void OnPurchasedItem(int itemIndex)
	{
		TravelLocationInfo travelLocationInfo = travelLocationsOrdered.ElementAtOrDefault(itemIndex);
		PlayerData instance = PlayerData.instance;
		instance.CaravanSpiderTargetScene = string.Empty;
		if (travelLocationInfo != null)
		{
			instance.CaravanSpiderTravelDirection = travelLocationInfo.DirectionTo;
			instance.CaravanSpiderTargetScene = travelLocationInfo.TargetScene;
		}
	}

	public int GetCostForScene(string sceneName)
	{
		foreach (TravelLocationInfo item in travelLocationsOrdered)
		{
			if (item.TargetScene == sceneName)
			{
				return item.GetCost();
			}
		}
		return 0;
	}
}

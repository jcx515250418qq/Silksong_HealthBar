using System.Collections.Generic;
using System.Linq;
using TeamCherry.Localization;
using UnityEngine;

public class CollectableItemRelicType : CollectableItem, ICollectionViewerItemList
{
	public enum RelicPlayTypes
	{
		None = 0,
		Gramaphone = 1
	}

	[Space]
	[SerializeField]
	private LocalisedString typeName;

	[SerializeField]
	private LocalisedString typeDescription;

	[SerializeField]
	[LocalisedString.NotRequired]
	private LocalisedString appendDescription;

	[SerializeField]
	private Sprite typeIcon;

	[SerializeField]
	private CustomInventoryItemCollectableDisplay iconOverridePrefab;

	[Space]
	[SerializeField]
	private RelicPlayTypes relicPlayType;

	[SerializeField]
	private int rewardAmount;

	[Space]
	[SerializeField]
	private List<CollectableRelic> relics = new List<CollectableRelic>();

	public override bool DisplayAmount => CollectedAmount > 1;

	public override int CollectedAmount => relics.Count((CollectableRelic relic) => relic.IsInInventory);

	public CustomInventoryItemCollectableDisplay IconOverridePrefab => iconOverridePrefab;

	public RelicPlayTypes RelicPlayType => relicPlayType;

	public int RewardAmount => rewardAmount;

	public IEnumerable<CollectableRelic> Relics => relics;

	private void OnEnable()
	{
		for (int num = relics.Count - 1; num >= 0; num--)
		{
			CollectableRelic collectableRelic = relics[num];
			if (collectableRelic == null)
			{
				relics.RemoveAt(num);
			}
			else
			{
				collectableRelic.RelicType = this;
			}
		}
	}

	public override string GetDisplayName(ReadSource readSource)
	{
		return typeName;
	}

	public override string GetDescription(ReadSource readSource)
	{
		if (!appendDescription.IsEmpty)
		{
			return $"{typeDescription}\n\n{appendDescription}";
		}
		return typeDescription;
	}

	public override Sprite GetIcon(ReadSource readSource)
	{
		return typeIcon;
	}

	public IEnumerable<ICollectionViewerItem> GetCollectionViewerItems()
	{
		foreach (CollectableRelic relic in relics)
		{
			yield return relic;
		}
	}
}

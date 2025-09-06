using UnityEngine;

public class ToolPouchInventoryDescription : MonoBehaviour
{
	[SerializeField]
	private GameObject pouchUpgradesGroup;

	[SerializeField]
	private IconCounter pouchUpgradesCounter;

	[SerializeField]
	private GameObject kitUpgradesGroup;

	[SerializeField]
	private IconCounter kitUpgradesCounter;

	private void OnEnable()
	{
		PlayerData instance = PlayerData.instance;
		UpdateDisplay(instance.ToolPouchUpgrades, pouchUpgradesGroup, pouchUpgradesCounter);
		UpdateDisplay(instance.ToolKitUpgrades, kitUpgradesGroup, kitUpgradesCounter);
	}

	private void UpdateDisplay(int upgrades, GameObject group, IconCounter counter)
	{
		if (upgrades > 0)
		{
			if ((bool)group)
			{
				group.SetActive(value: true);
			}
			if ((bool)counter)
			{
				counter.CurrentValue = upgrades;
			}
		}
		else if ((bool)group)
		{
			group.SetActive(value: false);
		}
	}
}

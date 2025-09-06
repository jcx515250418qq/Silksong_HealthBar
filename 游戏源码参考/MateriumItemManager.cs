using UnityEngine;

public class MateriumItemManager : ManagerSingleton<MateriumItemManager>
{
	[SerializeField]
	private MateriumItemList masterList;

	[SerializeField]
	private GameObject updateMessagePrefab;

	private GameObject updateMessage;

	public MateriumItemList MasterList => masterList;

	public static bool ConstructedMaterium => PlayerData.instance.ConstructedMaterium;

	private void Start()
	{
		if ((bool)updateMessagePrefab)
		{
			updateMessage = Object.Instantiate(updateMessagePrefab, base.transform, worldPositionStays: true);
			updateMessage.SetActive(value: false);
		}
	}

	public static void CheckAchievements()
	{
		CheckAchievements(queue: false);
	}

	public static void CheckAchievements(bool queue)
	{
		if (!PlayerData.instance.ConstructedMaterium || !ManagerSingleton<MateriumItemManager>.Instance)
		{
			return;
		}
		int num = 0;
		int num2 = 0;
		foreach (MateriumItem master in ManagerSingleton<MateriumItemManager>.Instance.masterList)
		{
			if (master.IsRequiredForCompletion)
			{
				num2++;
				if (master.IsCollected)
				{
					num++;
				}
			}
		}
		if (num >= num2)
		{
			if (queue)
			{
				GameManager.instance.QueueAchievement("MATERIUM_FULL");
			}
			else
			{
				GameManager.instance.AwardAchievement("MATERIUM_FULL");
			}
		}
	}

	public static void ShowUpdateMessage()
	{
		MateriumItemManager instance = ManagerSingleton<MateriumItemManager>.Instance;
		if ((bool)instance.updateMessage)
		{
			if (instance.updateMessage.activeSelf)
			{
				instance.updateMessage.SetActive(value: false);
			}
			instance.updateMessage.SetActive(value: true);
		}
	}
}

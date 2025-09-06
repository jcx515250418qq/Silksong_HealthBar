using System.Linq;
using UnityEngine;

[CreateAssetMenu(menuName = "Hornet/Collectable Items/Collectable Item (Tool Damage)")]
public class CollectableItemToolDamage : CollectableItemBasic
{
	[Space]
	[SerializeField]
	private ToolDamageFlags fromDamageType;

	[SerializeField]
	[ArrayForEnum(typeof(HealthManager.EnemySize))]
	private int[] dropAmounts;

	private void OnValidate()
	{
		ArrayForEnumAttribute.EnsureArraySize(ref dropAmounts, typeof(HealthManager.EnemySize));
	}

	public static CollectableItemToolDamage GetItem(ToolDamageFlags damageFlags, HealthManager.EnemySize enemySize, out int amount)
	{
		amount = 0;
		if (damageFlags == ToolDamageFlags.None)
		{
			return null;
		}
		foreach (CollectableItemToolDamage item in QuestManager.GetActiveQuests().SelectMany((FullQuestBase quest) => quest.Targets.Select((FullQuestBase.QuestTarget target) => target.Counter)).OfType<CollectableItemToolDamage>())
		{
			if ((damageFlags & item.fromDamageType) != 0)
			{
				amount = item.dropAmounts[(int)enemySize];
				return item;
			}
		}
		return null;
	}

	public override Sprite GetQuestCounterSprite(int index)
	{
		return GetIcon(ReadSource.Inventory);
	}
}

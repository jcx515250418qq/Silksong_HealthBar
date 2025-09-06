using UnityEngine;

[CreateAssetMenu(fileName = "New Boss Sequence", menuName = "Hollow Knight/Boss Sequence List")]
public class BossSequence : ScriptableObject
{
	[SerializeField]
	private BossScene[] bossScenes;

	public bool useSceneUnlocks = true;

	public BossScene.BossTest[] tests;

	[Space]
	public string achievementKey;

	[Space]
	public string customEndScene;

	public string customEndScenePlayerData;

	[Header("Bindings")]
	public int nailDamage = 5;

	[Tooltip("If nail damage is already at or below the above nailDamage value, use a percentage instead.")]
	[Range(0f, 1f)]
	public float lowerNailDamagePercentage = 0.8f;

	public int maxHealth = 5;

	public int Count => bossScenes.Length;

	public string GetSceneAt(int index)
	{
		return GetBossScene(index).sceneName;
	}

	public string GetSceneObjectName(int index)
	{
		return GetBossScene(index).name;
	}

	public bool CanLoad(int index)
	{
		if (GetBossScene(index).isHidden)
		{
			return GetBossScene(index).IsUnlocked(BossSceneCheckSource.Sequence);
		}
		return true;
	}

	public BossScene GetBossScene(int index)
	{
		BossScene bossScene = bossScenes[index];
		if (!bossScene.IsUnlockedSelf(BossSceneCheckSource.Sequence) && (bool)bossScene.baseBoss && bossScene.substituteBoss)
		{
			bossScene = bossScene.baseBoss;
		}
		return bossScene;
	}

	public bool IsUnlocked()
	{
		if (useSceneUnlocks)
		{
			BossScene[] array = bossScenes;
			foreach (BossScene bossScene in array)
			{
				if (!bossScene.isHidden && !bossScene.IsUnlocked(BossSceneCheckSource.Sequence))
				{
					return false;
				}
			}
		}
		if (tests != null && tests.Length != 0)
		{
			BossScene.BossTest[] array2 = tests;
			for (int i = 0; i < array2.Length; i++)
			{
				if (array2[i].IsUnlocked())
				{
					return true;
				}
			}
			return false;
		}
		return true;
	}

	public bool IsSceneHidden(int index)
	{
		return GetBossScene(index).isHidden;
	}
}

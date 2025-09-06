using System;
using UnityEngine;

[CreateAssetMenu(fileName = "New Boss Scene", menuName = "Hollow Knight/Boss Scene")]
public class BossScene : ScriptableObject
{
	[Serializable]
	public class BossTest
	{
		[Serializable]
		public struct BoolTest
		{
			public string playerDataBool;

			public bool value;
		}

		[Serializable]
		public struct IntTest
		{
			public enum Comparison
			{
				Equal = 0,
				NotEqual = 1,
				MoreThan = 2,
				LessThan = 3
			}

			public string playerDataInt;

			public int value;

			public Comparison comparison;
		}

		public enum SharedDataTest
		{
			GGUnlock = 0
		}

		public BoolTest[] boolTests;

		public IntTest[] intTests;

		public SharedDataTest[] sharedData;

		public bool IsUnlocked()
		{
			bool flag = true;
			if (flag)
			{
				BoolTest[] array = boolTests;
				for (int i = 0; i < array.Length; i++)
				{
					BoolTest boolTest = array[i];
					if (GameManager.instance.GetPlayerDataBool(boolTest.playerDataBool) != boolTest.value)
					{
						flag = false;
						break;
					}
				}
			}
			if (flag)
			{
				IntTest[] array2 = intTests;
				for (int i = 0; i < array2.Length; i++)
				{
					IntTest intTest = array2[i];
					int playerDataInt = GameManager.instance.GetPlayerDataInt(intTest.playerDataInt);
					if (playerDataInt > -9999)
					{
						bool flag2 = false;
						switch (intTest.comparison)
						{
						case IntTest.Comparison.Equal:
							flag2 = playerDataInt == intTest.value;
							break;
						case IntTest.Comparison.NotEqual:
							flag2 = playerDataInt != intTest.value;
							break;
						case IntTest.Comparison.LessThan:
							flag2 = playerDataInt < intTest.value;
							break;
						case IntTest.Comparison.MoreThan:
							flag2 = playerDataInt > intTest.value;
							break;
						}
						if (!flag2)
						{
							flag = false;
							break;
						}
					}
				}
			}
			if (flag && sharedData != null)
			{
				SharedDataTest[] array3 = sharedData;
				for (int i = 0; i < array3.Length; i++)
				{
					if (array3[i] == SharedDataTest.GGUnlock && GameManager.instance.GetStatusRecordInt("RecBossRushMode") == 1)
					{
						flag = false;
					}
				}
			}
			return flag;
		}
	}

	[Tooltip("The name of the scene to load.")]
	public string sceneName;

	[Tooltip("Tests that need to succeed in order for this boss the be considered \"unlocked\". (for old save files - new saves will set each boss scene unlocked by name)")]
	public BossTest[] bossTests;

	[Header("Sequence Only")]
	public BossScene baseBoss;

	public bool substituteBoss;

	[SerializeField]
	private Sprite displayIcon;

	[Tooltip("If this is checked this scene will not count toward overall sequence unlock, but will still only be loaded if it's unlocked.")]
	public bool isHidden;

	[Header("Boss Statue Only")]
	public bool requireUnlock;

	[SerializeField]
	private BossScene tier1Scene;

	[SerializeField]
	private BossScene tier2Scene;

	[SerializeField]
	private BossScene tier3Scene;

	public Sprite DisplayIcon => displayIcon;

	public string Tier1Scene
	{
		get
		{
			if (!tier1Scene)
			{
				return sceneName;
			}
			return tier1Scene.sceneName;
		}
	}

	public string Tier2Scene
	{
		get
		{
			if (!tier2Scene)
			{
				return sceneName;
			}
			return tier2Scene.sceneName;
		}
	}

	public string Tier3Scene
	{
		get
		{
			if (!tier3Scene)
			{
				return sceneName;
			}
			return tier3Scene.sceneName;
		}
	}

	public bool IsUnlocked(BossSceneCheckSource source)
	{
		if (source == BossSceneCheckSource.Sequence && (bool)baseBoss && baseBoss.IsUnlocked(source))
		{
			return true;
		}
		return IsUnlockedSelf(source);
	}

	public bool IsUnlockedSelf(BossSceneCheckSource source)
	{
		if (GameManager.instance.playerData.unlockedBossScenes.Contains(base.name))
		{
			return true;
		}
		if (bossTests != null && bossTests.Length != 0)
		{
			BossTest[] array = bossTests;
			for (int i = 0; i < array.Length; i++)
			{
				if (array[i].IsUnlocked())
				{
					return true;
				}
			}
		}
		else if (!requireUnlock || source != BossSceneCheckSource.Statue)
		{
			return true;
		}
		return false;
	}
}

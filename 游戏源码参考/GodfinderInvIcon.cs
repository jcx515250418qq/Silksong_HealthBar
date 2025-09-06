using System;
using UnityEngine;

public class GodfinderInvIcon : MonoBehaviour
{
	[Serializable]
	public class BossSceneExtra
	{
		public BossScene bossScene;

		[Tooltip("If any of these tests fail the boss scene will be skipped.")]
		public BossScene.BossTest[] extraTests;

		public bool IsUnlocked()
		{
			if (extraTests != null)
			{
				BossScene.BossTest[] array = extraTests;
				for (int i = 0; i < array.Length; i++)
				{
					if (!array[i].IsUnlocked())
					{
						return true;
					}
				}
			}
			if ((bool)bossScene)
			{
				return bossScene.IsUnlocked(BossSceneCheckSource.Godfinder);
			}
			return false;
		}
	}

	public Sprite normalSprite;

	public Sprite newBossSprite;

	public Sprite allBossesSprite;

	[Tooltip("Once all listed bosses are unlocked, godfinder is in complete state.")]
	public BossScene[] bosses;

	[Tooltip("Boss scenes with conditions as to whether they are counted or not.")]
	public BossSceneExtra[] extraBosses;

	private SpriteRenderer spriteRenderer;

	private void Awake()
	{
		spriteRenderer = GetComponent<SpriteRenderer>();
	}

	private void OnEnable()
	{
		if (!spriteRenderer)
		{
			return;
		}
		if (!GameManager.instance.playerData.bossRushMode)
		{
			spriteRenderer.sprite = (GameManager.instance.playerData.unlockedNewBossStatue ? newBossSprite : normalSprite);
			BossScene[] array = bosses;
			for (int i = 0; i < array.Length; i++)
			{
				if (!array[i].IsUnlocked(BossSceneCheckSource.Godfinder))
				{
					return;
				}
			}
			BossSceneExtra[] array2 = extraBosses;
			for (int i = 0; i < array2.Length; i++)
			{
				if (!array2[i].IsUnlocked())
				{
					return;
				}
			}
		}
		spriteRenderer.sprite = allBossesSprite;
	}
}

using UnityEngine;

public class BossStatueSetUnlockIndicator : MonoBehaviour
{
	private int newStatueCount;

	private void Start()
	{
		BossStatue[] array = Object.FindObjectsOfType<BossStatue>();
		foreach (BossStatue bossStatue in array)
		{
			if (!CheckIfNewBossStatue(bossStatue.StatueState) && !CheckIfNewBossStatue(bossStatue.DreamStatueState))
			{
				continue;
			}
			newStatueCount++;
			bossStatue.OnSeenNewStatue += delegate
			{
				newStatueCount--;
				if (newStatueCount == 0)
				{
					GameManager.instance.playerData.unlockedNewBossStatue = false;
				}
				else if (newStatueCount < 0)
				{
					Debug.LogError("New statue count fell below zero. This means something has gone wrong!");
				}
			};
		}
	}

	private bool CheckIfNewBossStatue(BossStatue.Completion completion)
	{
		if (completion.isUnlocked)
		{
			return !completion.hasBeenSeen;
		}
		return false;
	}
}

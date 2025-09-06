using System;
using System.Collections.Generic;
using UnityEngine;

public class AchievementHandler : MonoBehaviour
{
	public delegate void AchievementAwarded(string key);

	private struct QueuedAchievementProgress : IEquatable<QueuedAchievementProgress>
	{
		public string key;

		public int value;

		public int maxValue;

		public QueuedAchievementProgress(string key, int value, int maxValue)
		{
			this.key = key;
			this.value = value;
			this.maxValue = maxValue;
		}

		public bool Equals(QueuedAchievementProgress other)
		{
			return string.Equals(key, other.key);
		}

		public override bool Equals(object obj)
		{
			if (obj is QueuedAchievementProgress other)
			{
				return Equals(other);
			}
			return false;
		}

		public override int GetHashCode()
		{
			if (key == null)
			{
				return 0;
			}
			return key.GetHashCode();
		}

		public static bool operator ==(QueuedAchievementProgress left, QueuedAchievementProgress right)
		{
			return left.Equals(right);
		}

		public static bool operator !=(QueuedAchievementProgress left, QueuedAchievementProgress right)
		{
			return !(left == right);
		}
	}

	private GameManager gm;

	[SerializeField]
	private AchievementsList achievementsList;

	private readonly List<string> queuedAchievements = new List<string>();

	private readonly List<QueuedAchievementProgress> queuedAchievementProgressList = new List<QueuedAchievementProgress>();

	private readonly Dictionary<string, List<string>> achievementWhiteLists = new Dictionary<string, List<string>> { 
	{
		"GODS_GLORY",
		new List<string> { "PANTHEON1", "PANTHEON2", "PANTHEON3", "PANTHEON4", "ENDINGD", "COMPLETIONGG" }
	} };

	public List<string> QueuedAchievements => queuedAchievements;

	public AchievementsList AchievementsList => achievementsList;

	public event AchievementAwarded AwardAchievementEvent;

	private void Start()
	{
		gm = GameManager.instance;
	}

	public void AwardAchievementToPlayer(string key)
	{
		if (!DemoHelper.IsDemoMode && AchievementsList.FindAchievement(key) != null && CanAwardAchievement(key) && (CheatManager.AlwaysAwardAchievement || Platform.Current.IsAchievementUnlocked(key) != true))
		{
			Platform.Current.PushAchievementUnlock(key);
			if (gm.gameSettings.showNativeAchievementPopups == 1 && this.AwardAchievementEvent != null)
			{
				this.AwardAchievementEvent(key);
			}
		}
	}

	public void UpdateAchievementProgress(string key, int value, int max)
	{
		Platform.Current.UpdateAchievementProgress(key, value, max);
	}

	public bool AchievementWasAwarded(string key)
	{
		if (AchievementsList.FindAchievement(key) != null)
		{
			return Platform.Current.IsAchievementUnlocked(key) == true;
		}
		return false;
	}

	public void ResetAllAchievements()
	{
		Platform.Current.ResetAchievements();
	}

	public void FlushRecordsToDisk()
	{
		Platform.Current.RoamingSharedData.Save();
	}

	public void QueueAchievement(string key)
	{
		if (!QueuedAchievements.Contains(key))
		{
			QueuedAchievements.Add(key);
		}
	}

	public void QueueAchievementProgress(string key, int value, int max)
	{
		QueuedAchievementProgress queuedAchievementProgress = new QueuedAchievementProgress(key, value, max);
		int num = queuedAchievementProgressList.IndexOf(queuedAchievementProgress);
		if (num < 0)
		{
			queuedAchievementProgressList.Add(queuedAchievementProgress);
		}
		else
		{
			queuedAchievementProgressList[num] = queuedAchievementProgress;
		}
	}

	public void AwardQueuedAchievements()
	{
		foreach (string queuedAchievement in QueuedAchievements)
		{
			AwardAchievementToPlayer(queuedAchievement);
		}
		QueuedAchievements.Clear();
		if (!gm)
		{
			return;
		}
		foreach (QueuedAchievementProgress queuedAchievementProgress in queuedAchievementProgressList)
		{
			gm.UpdateAchievementProgress(queuedAchievementProgress.key, queuedAchievementProgress.value, queuedAchievementProgress.maxValue);
		}
		queuedAchievementProgressList.Clear();
	}

	public void AwardAllAchievements()
	{
		foreach (Achievement achievement in AchievementsList.Achievements)
		{
			AwardAchievementToPlayer(achievement.PlatformKey);
		}
	}

	[ContextMenu("Award Random Achievement", true)]
	private bool CanAwardRandomAchievement()
	{
		return Application.isPlaying;
	}

	[ContextMenu("Award Random Achievement")]
	private void AwardRandomAchievement()
	{
		Achievement achievement = AchievementsList.Achievements[UnityEngine.Random.Range(0, AchievementsList.Achievements.Count)];
		AwardAchievementToPlayer(achievement.PlatformKey);
	}

	private bool CanAwardAchievement(string key)
	{
		if ((bool)GameManager.instance)
		{
			string currentMapZone = GameManager.instance.GetCurrentMapZone();
			if (achievementWhiteLists.ContainsKey(currentMapZone))
			{
				if (achievementWhiteLists[currentMapZone].Contains(key))
				{
					return true;
				}
				return false;
			}
		}
		return true;
	}
}

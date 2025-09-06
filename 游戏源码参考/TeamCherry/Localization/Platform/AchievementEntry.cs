using System;

namespace TeamCherry.Localization.Platform
{
	[Serializable]
	public class AchievementEntry
	{
		public string internalAchievementID;

		public string achievementId;

		public string stableGuid;

		public string achievementNameId;

		public string lockedDescriptionId;

		public string unlockedDescriptionId;

		public bool isHidden;

		public int displayOrder;

		public int gamerscore;

		public bool baseAchievement;

		public string iconImageId;

		public void MergeFrom(AchievementEntry other)
		{
			if (!string.IsNullOrEmpty(other.internalAchievementID))
			{
				internalAchievementID = other.internalAchievementID;
			}
			if (!string.IsNullOrEmpty(other.achievementId))
			{
				achievementId = other.achievementId;
			}
			if (!string.IsNullOrEmpty(other.achievementNameId))
			{
				achievementNameId = other.achievementNameId;
			}
			if (!string.IsNullOrEmpty(other.lockedDescriptionId))
			{
				lockedDescriptionId = other.lockedDescriptionId;
			}
			if (!string.IsNullOrEmpty(other.unlockedDescriptionId))
			{
				unlockedDescriptionId = other.unlockedDescriptionId;
			}
			if (!string.IsNullOrEmpty(other.iconImageId))
			{
				iconImageId = other.iconImageId;
			}
			displayOrder = other.displayOrder;
			gamerscore = other.gamerscore;
			isHidden = other.isHidden;
			baseAchievement = other.baseAchievement;
		}
	}
}

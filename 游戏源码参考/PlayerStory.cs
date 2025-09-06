using System;
using System.Collections.Generic;

public static class PlayerStory
{
	public enum EventTypes
	{
		None = -1,
		HeartPiece = 0,
		SpoolPiece = 1,
		SimpleKey = 2,
		MemoryLocket = 3
	}

	[Serializable]
	public struct EventInfo
	{
		public EventTypes EventType;

		public string SceneName;

		public float PlayTime;
	}

	public static void RecordEvent(EventTypes eventTypes)
	{
		if (eventTypes != EventTypes.None)
		{
			GameManager instance = GameManager.instance;
			PlayerData playerData;
			PlayerData playerData2 = (playerData = instance.playerData);
			if (playerData.StoryEvents == null)
			{
				playerData.StoryEvents = new List<EventInfo>();
			}
			playerData2.StoryEvents.Add(new EventInfo
			{
				EventType = eventTypes,
				SceneName = instance.GetSceneNameString(),
				PlayTime = instance.PlayTime
			});
		}
	}
}

using System;

[Serializable]
public class QuestRumourData : SerializableNamedList<QuestRumourData.Data, QuestRumourData.NamedData>
{
	[Serializable]
	public class NamedData : SerializableNamedData<Data>
	{
	}

	[Serializable]
	public struct Data
	{
		public bool HasBeenSeen;

		public bool IsAccepted;
	}
}

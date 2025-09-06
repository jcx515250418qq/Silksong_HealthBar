using System;

[Serializable]
public class CollectableRelicsData : SerializableNamedList<CollectableRelicsData.Data, CollectableRelicsData.NamedData>
{
	[Serializable]
	public struct Data
	{
		public bool IsCollected;

		public bool IsDeposited;

		public bool HasSeenInRelicBoard;
	}

	[Serializable]
	public class NamedData : SerializableNamedData<Data>
	{
	}
}

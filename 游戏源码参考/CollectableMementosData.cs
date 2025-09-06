using System;

[Serializable]
public class CollectableMementosData : SerializableNamedList<CollectableMementosData.Data, CollectableMementosData.NamedData>
{
	[Serializable]
	public struct Data
	{
		public bool IsDeposited;

		public bool HasSeenInRelicBoard;
	}

	[Serializable]
	public class NamedData : SerializableNamedData<Data>
	{
	}
}

using System;

[Serializable]
public class MateriumItemsData : SerializableNamedList<MateriumItemsData.Data, MateriumItemsData.NamedData>
{
	[Serializable]
	public struct Data
	{
		public bool IsCollected;

		public bool HasSeenInRelicBoard;
	}

	[Serializable]
	public class NamedData : SerializableNamedData<Data>
	{
	}
}

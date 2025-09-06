using System;

[Serializable]
public class CollectableItemsData : SerializableNamedList<CollectableItemsData.Data, CollectableItemsData.NamedData>
{
	[Serializable]
	public struct Data
	{
		public int Amount;

		public int IsSeenMask;

		public int AmountWhileHidden;
	}

	[Serializable]
	public class NamedData : SerializableNamedData<Data>
	{
	}
}

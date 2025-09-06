using System;

[Serializable]
public class ToolItemLiquidsData : SerializableNamedList<ToolItemLiquidsData.Data, ToolItemLiquidsData.NamedData>
{
	[Serializable]
	public struct Data
	{
		public int RefillsLeft;

		public bool SeenEmptyState;

		public bool UsedExtra;
	}

	[Serializable]
	public class NamedData : SerializableNamedData<Data>
	{
	}
}

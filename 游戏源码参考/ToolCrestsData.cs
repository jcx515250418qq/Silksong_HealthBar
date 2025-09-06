using System;
using System.Collections.Generic;

[Serializable]
public class ToolCrestsData : SerializableNamedList<ToolCrestsData.Data, ToolCrestsData.NamedData>
{
	[Serializable]
	public struct SlotData
	{
		public string EquippedTool;

		public bool IsUnlocked;
	}

	[Serializable]
	public struct Data
	{
		public bool IsUnlocked;

		public List<SlotData> Slots;

		public bool DisplayNewIndicator;
	}

	[Serializable]
	public class NamedData : SerializableNamedData<Data>
	{
	}
}

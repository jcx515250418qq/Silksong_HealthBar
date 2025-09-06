using System;

[Serializable]
public class FloatingCrestSlotsData : SerializableNamedList<ToolCrestsData.SlotData, FloatingCrestSlotsData.NamedData>
{
	[Serializable]
	public class NamedData : SerializableNamedData<ToolCrestsData.SlotData>
	{
	}
}

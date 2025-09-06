using UnityEngine;

public class MemoryOrbSourceGroup : MemoryOrbSource
{
	[Space]
	[SerializeField]
	private MemoryOrbGroup[] groups;

	protected override bool IsActive
	{
		get
		{
			MemoryOrbGroup[] array = groups;
			for (int i = 0; i < array.Length; i++)
			{
				if (!array[i].IsAllCollected)
				{
					return true;
				}
			}
			return false;
		}
	}
}

using TeamCherry.SharedUtils;
using UnityEngine;

public class MemoryOrbSceneMapConditional : MonoBehaviour
{
	[SerializeField]
	[PlayerDataField(typeof(bool), false)]
	private string pdBool;

	[SerializeField]
	private bool targetValue;

	[SerializeField]
	[PlayerDataField(typeof(ulong), false)]
	private string pdBitmask;

	[SerializeField]
	private MemoryOrbSceneMapConditional other;

	public bool IsActive
	{
		get
		{
			if (!string.IsNullOrEmpty(pdBool) && PlayerData.instance.GetBool(pdBool) != targetValue)
			{
				return false;
			}
			if (!string.IsNullOrEmpty(pdBitmask) && PlayerData.instance.GetVariable<ulong>(pdBitmask) != 0L)
			{
				return false;
			}
			if ((bool)other)
			{
				return !other.IsActive;
			}
			return true;
		}
	}
}

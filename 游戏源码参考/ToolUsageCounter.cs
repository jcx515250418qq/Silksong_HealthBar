using UnityEngine;

public sealed class ToolUsageCounter : MonoBehaviour
{
	[SerializeField]
	private ToolItem toolItem;

	[SerializeField]
	private int usageAmount;

	private void OnEnable()
	{
		if (!ObjectPool.IsCreatingPool && toolItem != null)
		{
			toolItem.CustomUsage(usageAmount);
		}
	}
}

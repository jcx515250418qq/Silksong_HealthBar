using UnityEngine;

public class SilkSpoolInventoryDescription : MonoBehaviour
{
	[SerializeField]
	private GameObject silkHeartsGroup;

	[SerializeField]
	private IconCounter silkHeartsCounter;

	private void OnEnable()
	{
		int silkRegenMax = PlayerData.instance.silkRegenMax;
		if (silkRegenMax > 0)
		{
			if ((bool)silkHeartsGroup)
			{
				silkHeartsGroup.SetActive(value: true);
			}
			if ((bool)silkHeartsCounter)
			{
				silkHeartsCounter.MaxValue = Mathf.Max(3, silkRegenMax);
				silkHeartsCounter.CurrentValue = silkRegenMax;
			}
		}
		else if ((bool)silkHeartsGroup)
		{
			silkHeartsGroup.SetActive(value: false);
		}
	}
}

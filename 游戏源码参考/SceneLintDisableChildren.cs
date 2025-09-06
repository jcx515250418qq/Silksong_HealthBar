using UnityEngine;

public class SceneLintDisableChildren : MonoBehaviour, ISceneLintUpgrader
{
	public string OnSceneLintUpgrade(bool doUpgrade)
	{
		bool flag = false;
		foreach (Transform item in base.transform)
		{
			if (item.gameObject.activeSelf)
			{
				item.gameObject.SetActive(value: false);
				flag = true;
			}
		}
		if (!flag)
		{
			return null;
		}
		return "Deactivated child GameObjects so they start disabled";
	}
}

using UnityEngine;

public class MenuStyleUnlock : MonoBehaviour
{
	[SerializeField]
	private string unlockKey;

	[SerializeField]
	private bool forceChange = true;

	private void Start()
	{
		Unlock(unlockKey, forceChange);
		if (GameManager.instance.GetPlayerDataInt("permadeathMode") == 1)
		{
			Unlock("COMPLETED_STEEL", forceChange: false);
		}
	}

	public static void Unlock(string key, bool forceChange)
	{
		if (!string.IsNullOrEmpty(key) && GameManager.CanUnlockMenuStyle(key))
		{
			Platform current = Platform.Current;
			if (forceChange || !current.RoamingSharedData.GetBool(key, def: false))
			{
				current.RoamingSharedData.SetString("unlockedMenuStyle", key);
			}
			current.RoamingSharedData.SetBool(key, val: true);
			current.RoamingSharedData.Save();
		}
	}
}

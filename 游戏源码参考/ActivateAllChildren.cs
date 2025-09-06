using UnityEngine;

public class ActivateAllChildren : MonoBehaviour
{
	public string playerdataBoolTest;

	public bool reverseTest;

	public bool deactivateIfTestFailed;

	private void OnEnable()
	{
		if (playerdataBoolTest == "")
		{
			DoActivation();
		}
		else if ((PlayerData.instance.GetBool(playerdataBoolTest) && !reverseTest) || (!PlayerData.instance.GetBool(playerdataBoolTest) && reverseTest))
		{
			DoActivation();
		}
		else if (deactivateIfTestFailed)
		{
			DoDeactivation();
		}
	}

	private void DoActivation()
	{
		foreach (Transform item in base.transform)
		{
			item.gameObject.SetActive(value: true);
		}
	}

	private void DoDeactivation()
	{
		foreach (Transform item in base.transform)
		{
			item.gameObject.SetActive(value: false);
		}
	}
}

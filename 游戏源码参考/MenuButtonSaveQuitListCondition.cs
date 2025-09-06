using GlobalSettings;
using UnityEngine;
using UnityEngine.UI;

public class MenuButtonSaveQuitListCondition : MenuButtonListCondition
{
	[SerializeField]
	private Text buttonText;

	public override bool IsFulfilled()
	{
		if (!PlayerData.instance.disableSaveQuit)
		{
			return !ScenePreloader.HasPendingOperations;
		}
		return false;
	}

	public override bool AlwaysVisible()
	{
		return true;
	}

	public override void OnActiveStateSet(bool isActive)
	{
		if ((bool)buttonText)
		{
			buttonText.color = (isActive ? Color.white : UI.DisabledUiTextColor);
		}
	}
}

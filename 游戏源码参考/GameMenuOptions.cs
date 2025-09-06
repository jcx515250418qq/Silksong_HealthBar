using GlobalEnums;
using HKMenu;
using UnityEngine;
using UnityEngine.UI;

public class GameMenuOptions : MonoBehaviour, IMenuOptionLayout
{
	public MenuScreen gameOptionsMenuScreen;

	public MenuSelectable languageOption;

	public GameObject languageOptionDescription;

	public MenuSelectable backerOption;

	public MenuSelectable nativeAchievementsOption;

	public MenuSelectable resetButton;

	public MenuSelectable applyButton;

	public bool reconfigureOnEnable;

	private void OnEnable()
	{
		if (reconfigureOnEnable)
		{
			ConfigureNavigation();
		}
	}

	public void ConfigureNavigation()
	{
		if (GameManager.instance.GameState != GameState.MAIN_MENU)
		{
			languageOption.interactable = false;
			languageOption.transform.parent.gameObject.SetActive(value: true);
			languageOptionDescription.SetActive(value: true);
			Navigation navigation = backerOption.navigation;
			Navigation navigation2 = applyButton.navigation;
			navigation.selectOnUp = applyButton;
			navigation2.selectOnDown = backerOption;
			backerOption.navigation = navigation;
			applyButton.navigation = navigation2;
			gameOptionsMenuScreen.defaultHighlight = backerOption;
		}
		else
		{
			languageOption.interactable = true;
			languageOption.transform.parent.gameObject.SetActive(value: true);
			languageOptionDescription.SetActive(value: false);
			gameOptionsMenuScreen.defaultHighlight = languageOption;
		}
		if ((bool)languageOption && languageOption is MenuLanguageSetting)
		{
			((MenuLanguageSetting)languageOption).UpdateAlpha();
		}
		if (GameManager.instance.gameConfig.hideAchievementsMenu)
		{
			nativeAchievementsOption.transform.parent.gameObject.SetActive(value: false);
		}
	}
}

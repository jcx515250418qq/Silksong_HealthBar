using UnityEngine.UI;

public class MainMenuOptions : PreselectOption
{
	public MenuButton startButton;

	public MenuButton optionsButton;

	public MenuButton achievementsButton;

	public MenuButton extrasButton;

	public MenuButton quitButton;

	public void ConfigureNavigation()
	{
		if (Platform.Current.HasNativeAchievementsDialog && GameManager.instance.gameConfig.hideExtrasMenu)
		{
			achievementsButton.gameObject.SetActive(value: false);
			extrasButton.gameObject.SetActive(value: false);
			Navigation navigation = optionsButton.navigation;
			Navigation navigation2 = quitButton.navigation;
			navigation.selectOnDown = quitButton;
			navigation2.selectOnUp = optionsButton;
			optionsButton.navigation = navigation;
			quitButton.navigation = navigation2;
		}
		else if (Platform.Current.HasNativeAchievementsDialog)
		{
			achievementsButton.gameObject.SetActive(value: false);
			Navigation navigation3 = optionsButton.navigation;
			Navigation navigation4 = extrasButton.navigation;
			navigation3.selectOnDown = extrasButton;
			navigation4.selectOnUp = optionsButton;
			optionsButton.navigation = navigation3;
			extrasButton.navigation = navigation4;
		}
		else if (GameManager.instance.gameConfig.hideExtrasMenu)
		{
			extrasButton.gameObject.SetActive(value: false);
			Navigation navigation5 = achievementsButton.navigation;
			Navigation navigation6 = quitButton.navigation;
			navigation5.selectOnDown = quitButton;
			navigation6.selectOnUp = achievementsButton;
			achievementsButton.navigation = navigation5;
			quitButton.navigation = navigation6;
		}
	}
}

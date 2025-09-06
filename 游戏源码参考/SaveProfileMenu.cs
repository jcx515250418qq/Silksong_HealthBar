using UnityEngine;
using UnityEngine.UI;

public class SaveProfileMenu : MonoBehaviour
{
	private UIManager ui;

	private void Start()
	{
		ui = UIManager.instance;
	}

	public void BackAction()
	{
		if (!RestoreSaveButton.GoBack())
		{
			ui.UIGoToMainMenu();
		}
	}
}

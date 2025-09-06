using UnityEngine;
using UnityEngine.UI;

public class OverscanSetting : MonoBehaviour
{
	private GameSettings gs;

	public Slider slider;

	public MenuButton doneButton;

	public MenuButton backButton;

	public Text textUI;

	public float value;

	private void Start()
	{
		gs = GameManager.instance.gameSettings;
		textUI.text = slider.value.ToString();
	}

	public void UpdateValue()
	{
		textUI.text = slider.value.ToString();
	}

	public void UpdateTextValue(float value)
	{
		textUI.text = value.ToString();
	}

	public void SetOverscan(float value)
	{
		float overscan = value * 0.01f;
		GameCameras.instance.SetOverscan(overscan);
	}

	public void RefreshValueFromSettings()
	{
		if (gs == null)
		{
			gs = GameManager.instance.gameSettings;
		}
		slider.value = gs.overScanAdjustment * 100f;
	}

	public void DoneMode()
	{
		doneButton.gameObject.SetActive(value: true);
		backButton.gameObject.SetActive(value: false);
		slider.navigation = new Navigation
		{
			mode = Navigation.Mode.Explicit,
			selectOnDown = doneButton,
			selectOnUp = doneButton
		};
	}

	public void NormalMode()
	{
		doneButton.gameObject.SetActive(value: false);
		backButton.gameObject.SetActive(value: true);
		slider.navigation = new Navigation
		{
			mode = Navigation.Mode.Explicit,
			selectOnDown = backButton,
			selectOnUp = backButton
		};
	}
}

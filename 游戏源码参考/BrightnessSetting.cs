using UnityEngine;
using UnityEngine.UI;

public class BrightnessSetting : MonoBehaviour
{
	private GameSettings gs;

	private float valueMultiplier = 5f;

	public Slider slider;

	public MenuButton doneButton;

	public MenuButton backButton;

	public Text textUI;

	private void Start()
	{
		gs = GameManager.instance.gameSettings;
		UpdateValue();
	}

	public void UpdateValue()
	{
		textUI.text = slider.value * valueMultiplier + "%";
	}

	public void UpdateTextValue(float value)
	{
		textUI.text = value * valueMultiplier + "%";
	}

	public void SetBrightness(float value)
	{
		GameCameras.instance.brightnessEffect.SetBrightness(value / 20f);
		gs.brightnessAdjustment = value;
	}

	public void RefreshValueFromSettings()
	{
		if (gs == null)
		{
			gs = GameManager.instance.gameSettings;
		}
		slider.value = gs.brightnessAdjustment;
		slider.onValueChanged.Invoke(slider.value);
		UpdateValue();
	}

	public void DoneMode()
	{
		doneButton.gameObject.SetActive(value: true);
		backButton.gameObject.SetActive(value: false);
		GetComponentInParent<MenuScreen>(includeInactive: true).backButton = doneButton;
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
		GetComponentInParent<MenuScreen>(includeInactive: true).backButton = backButton;
		slider.navigation = new Navigation
		{
			mode = Navigation.Mode.Explicit,
			selectOnDown = backButton,
			selectOnUp = backButton
		};
	}
}

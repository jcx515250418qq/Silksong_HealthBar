using UnityEngine;
using UnityEngine.UI;

public class UpdateTextWithSliderValue : MonoBehaviour
{
	public Slider slider;

	private Text textUI;

	public float value;

	private void Start()
	{
		textUI = GetComponent<Text>();
		textUI.text = slider.value.ToString();
	}

	public void UpdateValue()
	{
		textUI.text = slider.value.ToString();
	}
}

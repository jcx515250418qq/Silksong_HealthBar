using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

[DisallowMultipleComponent]
public sealed class SliderRightStickInput : MonoBehaviour, ISelectHandler, IEventSystemHandler, IDeselectHandler
{
	[SerializeField]
	private Slider slider;

	[SerializeField]
	private float threshold = 0.7f;

	private bool isSelected;

	private bool hasInputHandler;

	private InputHandler ih;

	private void Start()
	{
		if (!isSelected)
		{
			SetSelected(EventSystem.current.currentSelectedGameObject == base.gameObject);
		}
		ManagerSingleton<InputHandler>.Instance.GetSticksInput(out var _);
	}

	private void OnValidate()
	{
		if (slider == null)
		{
			slider = GetComponent<Slider>();
		}
	}

	private void Update()
	{
		if (!hasInputHandler)
		{
			return;
		}
		bool isRightStick;
		Vector2 sticksInput = ih.GetSticksInput(out isRightStick);
		if (isRightStick && Mathf.Abs(sticksInput.x) >= threshold)
		{
			if (sticksInput.x < 0f)
			{
				slider.value = slider.minValue;
			}
			else
			{
				slider.value = slider.maxValue;
			}
		}
	}

	private void OnDisable()
	{
		isSelected = false;
	}

	private void SetSelected(bool isSelected)
	{
		this.isSelected = isSelected;
		if (isSelected)
		{
			ih = ManagerSingleton<InputHandler>.Instance;
			hasInputHandler = ih != null;
		}
	}

	public void OnSelect(BaseEventData eventData)
	{
		SetSelected(isSelected: true);
	}

	public void OnDeselect(BaseEventData eventData)
	{
		SetSelected(isSelected: false);
	}
}

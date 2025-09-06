using UnityEngine;

public class PressurePlateBinarySwitcher : MonoBehaviour
{
	[SerializeField]
	private TempPressurePlate defaultPlate;

	[SerializeField]
	private TempPressurePlate altPlate;

	private bool isCurrentlyAlt;

	private IBinarySwitchable[] switchables;

	private void Awake()
	{
		switchables = GetComponentsInChildren<IBinarySwitchable>();
	}

	private void Start()
	{
		if ((bool)defaultPlate)
		{
			defaultPlate.Activated += Toggle;
		}
		if ((bool)altPlate)
		{
			altPlate.Activated += Toggle;
		}
		SetPlatesActivated(isAlt: false);
	}

	[ContextMenu("Test", true)]
	private bool CanTest()
	{
		return Application.isPlaying;
	}

	[ContextMenu("Test")]
	public void Toggle()
	{
		SetPlatesActivated(!isCurrentlyAlt);
	}

	private void SetPlatesActivated(bool isAlt)
	{
		isCurrentlyAlt = isAlt;
		if (isAlt)
		{
			if ((bool)defaultPlate)
			{
				defaultPlate.Deactivate();
			}
			if ((bool)altPlate)
			{
				altPlate.ActivateSilent();
			}
		}
		else
		{
			if ((bool)defaultPlate)
			{
				defaultPlate.ActivateSilent();
			}
			if ((bool)altPlate)
			{
				altPlate.Deactivate();
			}
		}
		IBinarySwitchable[] array = switchables;
		for (int i = 0; i < array.Length; i++)
		{
			array[i].SwitchBinaryState(isCurrentlyAlt);
		}
	}
}

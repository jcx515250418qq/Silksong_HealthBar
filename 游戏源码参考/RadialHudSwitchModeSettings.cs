using UnityEngine;
using UnityEngine.UI;

public class RadialHudSwitchModeSettings : SwitchPlatformModeUpdateHandler
{
	[SerializeField]
	private ToolHudIcon hudIcon;

	[SerializeField]
	private Image[] circleImages;

	[Space]
	[SerializeField]
	private Sprite regularMode;

	[SerializeField]
	private Sprite handheldMode;

	[SerializeField]
	private Sprite glowRegular;

	[SerializeField]
	private Sprite glowHandheld;

	private bool isHandheld;

	private void Awake()
	{
		hudIcon.Updated += OnHudIconUpdated;
	}

	protected override void OnOperationModeChanged(bool newIsHandheld)
	{
		isHandheld = newIsHandheld;
		UpdateSprites();
	}

	private void OnHudIconUpdated()
	{
		UpdateSprites();
	}

	private void UpdateSprites()
	{
		ToolItem currentTool = hudIcon.CurrentTool;
		if (!currentTool)
		{
			return;
		}
		Sprite sprite;
		Sprite sprite2;
		if (currentTool is ToolItemSkill && !hudIcon.GetIsEmpty())
		{
			sprite = glowRegular;
			sprite2 = glowHandheld;
		}
		else
		{
			sprite = regularMode;
			sprite2 = handheldMode;
		}
		if (isHandheld)
		{
			Image[] array = circleImages;
			for (int i = 0; i < array.Length; i++)
			{
				array[i].sprite = sprite2;
			}
		}
		else
		{
			Image[] array = circleImages;
			for (int i = 0; i < array.Length; i++)
			{
				array[i].sprite = sprite;
			}
		}
	}
}

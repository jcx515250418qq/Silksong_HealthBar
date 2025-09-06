using TeamCherry.Localization;
using UnityEngine;

[CreateAssetMenu(fileName = "New Tool", menuName = "Hornet/Tool Item (Basic)")]
public class ToolItemBasic : ToolItem
{
	[Header("Basic")]
	[SerializeField]
	private LocalisedString displayName;

	[SerializeField]
	private LocalisedString description;

	[SerializeField]
	private Sprite inventorySprite;

	[SerializeField]
	private Sprite inventorySpritePoison;

	[SerializeField]
	private Sprite hudSprite;

	[SerializeField]
	private Sprite hudSpritePoison;

	[Space]
	[SerializeField]
	[LocalisedString.NotRequired]
	private LocalisedString popupNameOverride;

	[SerializeField]
	private Sprite popupIconOverride;

	[Space]
	[SerializeField]
	private UsageOptions usageOptions;

	public override LocalisedString DisplayName => displayName;

	public override LocalisedString Description => description;

	public override UsageOptions Usage => usageOptions;

	public override string GetPopupName()
	{
		if (popupNameOverride.IsEmpty)
		{
			return base.GetPopupName();
		}
		return popupNameOverride;
	}

	public override Sprite GetPopupIcon()
	{
		if (!popupIconOverride)
		{
			return base.GetPopupIcon();
		}
		return popupIconOverride;
	}

	public override Sprite GetInventorySprite(IconVariants iconVariant)
	{
		if (iconVariant == IconVariants.Poison)
		{
			return inventorySpritePoison ? inventorySpritePoison : inventorySprite;
		}
		return inventorySprite;
	}

	public override Sprite GetHudSprite(IconVariants iconVariant)
	{
		if (iconVariant == IconVariants.Poison)
		{
			return hudSpritePoison ? hudSpritePoison : hudSprite;
		}
		return hudSprite;
	}
}

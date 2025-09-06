using System;
using TeamCherry.Localization;
using UnityEngine;

[CreateAssetMenu(fileName = "New Tool", menuName = "Hornet/Tool Item (Lerp States)")]
public class ToolItemLerpStates : ToolItem
{
	[Serializable]
	private struct StateInfo
	{
		public Sprite InventorySprite;

		public Sprite InventorySpritePoison;

		public Sprite HudSprite;

		public Sprite HudSpritePoison;
	}

	[Header("States")]
	[SerializeField]
	private LocalisedString displayName;

	[SerializeField]
	private LocalisedString description;

	[Space]
	[SerializeField]
	private StateInfo emptyState;

	[SerializeField]
	private StateInfo[] states;

	[SerializeField]
	private StateInfo fullState;

	[Space]
	[SerializeField]
	private UsageOptions usageOptions;

	public override LocalisedString DisplayName => displayName;

	public override LocalisedString Description => description;

	public override UsageOptions Usage => usageOptions;

	public override Sprite GetInventorySprite(IconVariants iconVariant)
	{
		StateInfo currentState = GetCurrentState();
		if (iconVariant == IconVariants.Poison)
		{
			return currentState.InventorySpritePoison ? currentState.InventorySpritePoison : currentState.InventorySprite;
		}
		return currentState.InventorySprite;
	}

	public override Sprite GetHudSprite(IconVariants iconVariant)
	{
		StateInfo currentState = GetCurrentState();
		if (iconVariant == IconVariants.Poison)
		{
			return currentState.HudSpritePoison ? currentState.HudSpritePoison : currentState.HudSprite;
		}
		return currentState.HudSprite;
	}

	private StateInfo GetCurrentState()
	{
		if (!Application.isPlaying)
		{
			return fullState;
		}
		int amountLeft = base.SavedData.AmountLeft;
		int toolStorageAmount = ToolItemManager.GetToolStorageAmount(this);
		if (amountLeft <= 0)
		{
			return emptyState;
		}
		if (amountLeft >= toolStorageAmount)
		{
			return fullState;
		}
		if (states == null || states.Length == 0)
		{
			return emptyState;
		}
		int num = Mathf.RoundToInt((float)amountLeft / (float)toolStorageAmount * (float)states.Length);
		if (num >= states.Length)
		{
			num = states.Length - 1;
		}
		return states[num];
	}
}

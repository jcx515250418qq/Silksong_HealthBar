using System;
using TeamCherry.Localization;
using UnityEngine;

[CreateAssetMenu(fileName = "New Tool", menuName = "Hornet/Tool Item (States)")]
public class ToolItemStates : ToolItem
{
	[Serializable]
	protected struct StateInfo
	{
		public LocalisedString DisplayName;

		public LocalisedString Description;

		public Sprite InventorySprite;

		public Sprite InventorySpritePoison;

		public Sprite HudSprite;

		public Sprite HudSpritePoison;

		public UsageOptions Usage;
	}

	[Header("States")]
	[SerializeField]
	private StateInfo fullState;

	[SerializeField]
	private bool hasUsableEmptyState;

	[SerializeField]
	private StateInfo usableEmptyState;

	[SerializeField]
	private StateInfo emptyState;

	public override LocalisedString DisplayName => GetCurrentStateSafe().DisplayName;

	public override LocalisedString Description => GetCurrentStateSafe().Description;

	public override UsageOptions Usage => GetCurrentStateSafe().Usage;

	public override bool UsableWhenEmpty
	{
		get
		{
			UsageOptions usageOptions = (hasUsableEmptyState ? usableEmptyState.Usage : emptyState.Usage);
			if (!(usageOptions.ThrowPrefab != null))
			{
				return !string.IsNullOrEmpty(usageOptions.FsmEventName);
			}
			return true;
		}
	}

	public override Sprite GetInventorySprite(IconVariants iconVariant)
	{
		StateInfo currentStateSafe = GetCurrentStateSafe();
		if (iconVariant == IconVariants.Poison)
		{
			return currentStateSafe.InventorySpritePoison ? currentStateSafe.InventorySpritePoison : currentStateSafe.InventorySprite;
		}
		return currentStateSafe.InventorySprite;
	}

	public override Sprite GetHudSprite(IconVariants iconVariant)
	{
		StateInfo currentStateSafe = GetCurrentStateSafe();
		if (iconVariant == IconVariants.Poison)
		{
			return currentStateSafe.HudSpritePoison ? currentStateSafe.HudSpritePoison : currentStateSafe.HudSprite;
		}
		return currentStateSafe.HudSprite;
	}

	private StateInfo GetCurrentStateSafe()
	{
		if (!Application.isPlaying)
		{
			return fullState;
		}
		return GetCurrentState();
	}

	protected StateInfo GetCurrentState()
	{
		if (!base.IsEmpty)
		{
			return fullState;
		}
		if (hasUsableEmptyState && UsableWhenEmpty)
		{
			return usableEmptyState;
		}
		return emptyState;
	}

	protected override Sprite GetFullIcon()
	{
		return fullState.InventorySprite;
	}
}

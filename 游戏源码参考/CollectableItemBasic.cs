using TeamCherry.Localization;
using TeamCherry.SharedUtils;
using UnityEngine;

[CreateAssetMenu(menuName = "Hornet/Collectable Items/Collectable Item (Basic)")]
public class CollectableItemBasic : CollectableItem
{
	[Space]
	[SerializeField]
	private LocalisedString displayName;

	[SerializeField]
	private LocalisedString description;

	[SerializeField]
	private Sprite icon;

	[SerializeField]
	private Sprite tinyIcon;

	[SerializeField]
	private bool isAlwaysUnlocked;

	[Space]
	[SerializeField]
	[PlayerDataField(typeof(bool), false)]
	private string uniqueCollectBool;

	[SerializeField]
	private PlayerDataBoolOperation[] setExtraPlayerDataBools;

	[SerializeField]
	private PlayerDataIntOperation[] setExtraPlayerDataInts;

	[Space]
	[SerializeField]
	public bool displayButtonPrompt;

	[ModifiableProperty]
	[Conditional("displayButtonPrompt", true, false, false)]
	public InventoryItemButtonPromptData[] buttonPromptData;

	public override int CollectedAmount => base.CollectedAmount + (isAlwaysUnlocked ? 1 : 0);

	public override bool DisplayAmount => CollectedAmount > 1;

	public override string GetDisplayName(ReadSource readSource)
	{
		return displayName;
	}

	public override string GetDescription(ReadSource readSource)
	{
		return description;
	}

	public override Sprite GetIcon(ReadSource readSource)
	{
		if (readSource == ReadSource.Tiny && (bool)tinyIcon)
		{
			return tinyIcon;
		}
		return icon;
	}

	public override InventoryItemButtonPromptData[] GetButtonPromptData()
	{
		if (displayButtonPrompt)
		{
			return buttonPromptData;
		}
		return null;
	}

	protected override void OnCollected()
	{
		SetUniqueBool();
		PlayerDataBoolOperation[] array = setExtraPlayerDataBools;
		foreach (PlayerDataBoolOperation playerDataBoolOperation in array)
		{
			playerDataBoolOperation.Execute();
		}
		PlayerDataIntOperation[] array2 = setExtraPlayerDataInts;
		foreach (PlayerDataIntOperation playerDataIntOperation in array2)
		{
			playerDataIntOperation.Execute();
		}
	}

	public override void ReportPreviouslyCollected()
	{
		base.ReportPreviouslyCollected();
		SetUniqueBool();
	}

	private void SetUniqueBool()
	{
		if (!string.IsNullOrEmpty(uniqueCollectBool))
		{
			PlayerData.instance.SetVariable(uniqueCollectBool, value: true);
		}
	}

	public override bool CanGetMore()
	{
		if (!string.IsNullOrEmpty(uniqueCollectBool))
		{
			return !PlayerData.instance.GetVariable<bool>(uniqueCollectBool);
		}
		return base.CanGetMore();
	}

	public override bool ShouldStopCollectNoMsg()
	{
		if (!string.IsNullOrEmpty(uniqueCollectBool))
		{
			return PlayerData.instance.GetVariable<bool>(uniqueCollectBool);
		}
		return base.ShouldStopCollectNoMsg();
	}
}

using TeamCherry.Localization;
using TeamCherry.SharedUtils;
using UnityEngine;

public class InventoryItemBasic : InventoryItemUpdateable
{
	[SerializeField]
	[PlayerDataField(typeof(bool), false)]
	private string hasSeenPdBool;

	[Space]
	[SerializeField]
	private LocalisedString displayName;

	[SerializeField]
	private LocalisedString description;

	public override string DisplayName => displayName;

	public override string Description => description;

	protected override bool IsSeen
	{
		get
		{
			if (string.IsNullOrEmpty(hasSeenPdBool))
			{
				return true;
			}
			return PlayerData.instance.GetVariable<bool>(hasSeenPdBool);
		}
		set
		{
			if (!string.IsNullOrEmpty(hasSeenPdBool))
			{
				PlayerData.instance.SetVariable(hasSeenPdBool, value);
			}
		}
	}

	public void SetDisplayProperties(LocalisedString newDisplayName, LocalisedString newDescription)
	{
		displayName = newDisplayName;
		description = newDescription;
	}
}

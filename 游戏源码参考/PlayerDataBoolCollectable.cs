using TeamCherry.SharedUtils;
using UnityEngine;

[CreateAssetMenu(menuName = "Hornet/Collectable Items/Collectable Item (PlayerData Bool)")]
public class PlayerDataBoolCollectable : FakeCollectable
{
	[Space]
	[SerializeField]
	[PlayerDataField(typeof(bool), true)]
	private string boolName;

	public override bool CanGetMore()
	{
		return !PlayerData.instance.GetVariable<bool>(boolName);
	}

	public override void Get(bool showPopup = true)
	{
		base.Get(showPopup);
		PlayerData.instance.SetVariable(boolName, value: true);
		CollectableItemManager.IncrementVersion();
	}
}

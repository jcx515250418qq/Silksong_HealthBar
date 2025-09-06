using TeamCherry.SharedUtils;
using UnityEngine;

[CreateAssetMenu(menuName = "Hornet/Collectable Items/Collectable Item (PlayerData)")]
public class PlayerDataCollectable : FakeCollectable
{
	[Space]
	[SerializeField]
	[PlayerDataField(typeof(bool), false)]
	private string linkedPDBool;

	[SerializeField]
	[PlayerDataField(typeof(int), false)]
	private string linkedPDInt;

	[SerializeField]
	private PlayerDataBoolOperation[] setPlayerDataBools;

	[SerializeField]
	private PlayerDataIntOperation[] setPlayerDataInts;

	[Space]
	[SerializeField]
	private bool isMap;

	[SerializeField]
	private bool isToolPouch;

	public override void Get(bool showPopup = true)
	{
		base.Get(showPopup);
		PlayerData instance = PlayerData.instance;
		if (!string.IsNullOrEmpty(linkedPDBool))
		{
			instance.SetVariable(linkedPDBool, value: true);
		}
		if (!string.IsNullOrEmpty(linkedPDInt))
		{
			int variable = instance.GetVariable<int>(linkedPDInt);
			variable++;
			instance.SetVariable(linkedPDInt, variable);
		}
		PlayerDataBoolOperation[] array = setPlayerDataBools;
		foreach (PlayerDataBoolOperation playerDataBoolOperation in array)
		{
			playerDataBoolOperation.Execute();
		}
		PlayerDataIntOperation[] array2 = setPlayerDataInts;
		foreach (PlayerDataIntOperation playerDataIntOperation in array2)
		{
			playerDataIntOperation.Execute();
		}
		if (isMap)
		{
			GameManager instance2 = GameManager.instance;
			instance2.UpdateGameMapWithPopup(1f);
			instance2.CheckMapAchievements();
		}
		CollectableItemManager.IncrementVersion();
		if (isToolPouch)
		{
			ToolItemManager.SendEquippedChangedEvent(force: true);
		}
	}

	public override int GetSavedAmount()
	{
		PlayerData instance = PlayerData.instance;
		if (!string.IsNullOrEmpty(linkedPDInt))
		{
			return instance.GetVariable<int>(linkedPDInt);
		}
		if (!string.IsNullOrEmpty(linkedPDBool) && instance.GetVariable<bool>(linkedPDBool))
		{
			return 1;
		}
		return base.GetSavedAmount();
	}

	public override bool CanGetMore()
	{
		if (!string.IsNullOrEmpty(linkedPDBool))
		{
			return !PlayerData.instance.GetVariable<bool>(linkedPDBool);
		}
		return base.CanGetMore();
	}
}

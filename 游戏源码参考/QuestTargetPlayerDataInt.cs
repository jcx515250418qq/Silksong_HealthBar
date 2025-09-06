using TeamCherry.SharedUtils;
using UnityEngine;

[CreateAssetMenu(menuName = "Hornet/Quests/Quest Target PlayerData Int")]
public class QuestTargetPlayerDataInt : QuestTargetCounter
{
	[SerializeField]
	[PlayerDataField(typeof(int), true)]
	private string playerDataInt;

	[SerializeField]
	private Sprite questCounterSprite;

	public override bool CanGetMore()
	{
		return true;
	}

	public override int GetCompletionAmount(QuestCompletionData.Completion sourceCompletion)
	{
		return PlayerData.instance.GetVariable<int>(playerDataInt);
	}

	public override void Get(bool showPopup = true)
	{
		PlayerData instance = PlayerData.instance;
		VariableExtensions.SetVariable(value: instance.GetVariable<int>(playerDataInt) + 1, obj: instance, fieldName: playerDataInt);
	}

	public override Sprite GetPopupIcon()
	{
		return questCounterSprite;
	}
}

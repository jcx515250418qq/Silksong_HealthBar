using TeamCherry.Localization;
using UnityEngine;

[CreateAssetMenu(menuName = "Hornet/Quests/Quest Target Currency")]
public class QuestTargetCurrency : QuestTargetCounter
{
	[SerializeField]
	private CurrencyType currencyType;

	[SerializeField]
	private LocalisedString givePromptText;

	[SerializeField]
	private Sprite questCounterSprite;

	public CurrencyType CurrencyType => currencyType;

	public string GivePromptText => givePromptText;

	protected override bool ShowCounterOnConsume => true;

	public override bool CanConsume => true;

	public override bool CanGetMore()
	{
		return true;
	}

	public override int GetCompletionAmount(QuestCompletionData.Completion sourceCompletion)
	{
		return CurrencyManager.GetCurrencyAmount(currencyType);
	}

	public override void Consume(int amount, bool showCounter)
	{
		CurrencyManager.TakeCurrency(amount, currencyType, showCounter);
	}

	public override Sprite GetPopupIcon()
	{
		return questCounterSprite;
	}
}

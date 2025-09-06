using TeamCherry.Localization;
using UnityEngine;

public class DeliveryQuestItemStandalone : DeliveryQuestItem
{
	[SerializeField]
	private int targetCount;

	[SerializeField]
	[LocalisedString.NotRequired]
	private LocalisedString invItemAppendDesc;

	public int TargetCount => targetCount;

	public LocalisedString InvItemAppendDesc => invItemAppendDesc;
}

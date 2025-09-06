using UnityEngine;

[CreateAssetMenu(menuName = "Hornet/Collectable Items/Collectable Item (Currency)")]
public class CurrencyCollectable : FakeCollectable
{
	[Space]
	[SerializeField]
	private CurrencyType currencyType;

	public override void Get(bool showPopup = true)
	{
		GetMultiple(1, showPopup);
	}

	protected override void GetMultiple(int amount, bool showPopup)
	{
		base.Get(showPopup);
		CurrencyManager.AddCurrency(amount, currencyType);
	}
}

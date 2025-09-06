using HutongGames.PlayMaker;

public class TakeCrawbellCurrency : FsmStateAction
{
	[UIHint(UIHint.Variable)]
	public FsmInt StoreRosaries;

	[UIHint(UIHint.Variable)]
	public FsmInt StoreShellShards;

	public override void Reset()
	{
		StoreRosaries = null;
		StoreShellShards = null;
	}

	public override void OnEnter()
	{
		PlayerData instance = PlayerData.instance;
		ArrayForEnumAttribute.EnsureArraySize(ref instance.CrawbellCurrency, typeof(CurrencyType));
		StoreRosaries.Value = instance.CrawbellCurrency[0];
		instance.CrawbellCurrency[0] = 0;
		StoreShellShards.Value = instance.CrawbellCurrency[1];
		instance.CrawbellCurrency[1] = 0;
		if (instance.CrawbellCurrencyCaps != null)
		{
			for (int i = 0; i < instance.CrawbellCurrencyCaps.Length; i++)
			{
				instance.CrawbellCurrencyCaps[i] = 0;
			}
		}
		Finish();
	}
}

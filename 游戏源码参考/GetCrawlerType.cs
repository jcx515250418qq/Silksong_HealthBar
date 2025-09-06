using HutongGames.PlayMaker;

[ActionCategory("Enemy AI")]
public class GetCrawlerType : FSMUtility.GetComponentFsmStateAction<Crawler>
{
	[ObjectType(typeof(Crawler.CrawlerTypes))]
	[UIHint(UIHint.Variable)]
	public FsmEnum StoreType;

	public override void Reset()
	{
		base.Reset();
		StoreType = null;
	}

	protected override void DoAction(Crawler crawler)
	{
		StoreType.Value = crawler.Type;
	}
}

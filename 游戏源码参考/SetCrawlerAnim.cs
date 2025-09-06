using HutongGames.PlayMaker;

[ActionCategory("Enemy AI")]
public class SetCrawlerAnim : FSMUtility.GetComponentFsmStateAction<Crawler>
{
	public FsmString CrawlAnim;

	public FsmString TurnAnim;

	public override void Reset()
	{
		base.Reset();
		CrawlAnim = null;
		TurnAnim = null;
	}

	protected override void DoAction(Crawler crawler)
	{
		if (!CrawlAnim.IsNone)
		{
			crawler.crawlAnimName = CrawlAnim.Value;
		}
		if (!TurnAnim.IsNone)
		{
			crawler.turnAnimName = TurnAnim.Value;
		}
	}
}

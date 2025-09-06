using HutongGames.PlayMaker;

[ActionCategory("Enemy AI")]
public class SetCrawlerSpeed : FSMUtility.GetComponentFsmStateAction<Crawler>
{
	public FsmFloat Speed;

	public override void Reset()
	{
		base.Reset();
		Speed = null;
	}

	protected override void DoAction(Crawler crawler)
	{
		crawler.Speed = Speed.Value;
	}
}

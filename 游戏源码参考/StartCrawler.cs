using HutongGames.PlayMaker;
using UnityEngine;

[ActionCategory("Enemy AI")]
public class StartCrawler : FsmStateAction
{
	public FsmOwnerDefault Target;

	private Crawler crawler;

	public FsmBool ScheduleTurn;

	public override void Reset()
	{
		Target = null;
		crawler = null;
		ScheduleTurn = null;
	}

	public override void OnEnter()
	{
		GameObject safe = Target.GetSafe(this);
		if ((bool)safe)
		{
			crawler = safe.GetComponent<Crawler>();
		}
		if ((bool)crawler)
		{
			crawler.StartCrawling(ScheduleTurn.Value);
		}
		Finish();
	}
}

using HutongGames.PlayMaker;
using UnityEngine;

[ActionCategory("Enemy AI")]
public class StopCrawler : FsmStateAction
{
	public FsmOwnerDefault Target;

	private Crawler crawler;

	public FsmBool WaitForTurn;

	public override void Reset()
	{
		Target = null;
		crawler = null;
		WaitForTurn = new FsmBool(true);
	}

	public override void OnEnter()
	{
		GameObject safe = Target.GetSafe(this);
		if ((bool)safe)
		{
			crawler = safe.GetComponent<Crawler>();
		}
		if (!crawler)
		{
			Finish();
		}
		if (WaitForTurn.Value)
		{
			Evaluate();
			return;
		}
		crawler.StopCrawling();
		Finish();
	}

	public override void OnUpdate()
	{
		Evaluate();
	}

	private void Evaluate()
	{
		if (!crawler.IsTurning)
		{
			crawler.StopCrawling();
			Finish();
		}
	}
}

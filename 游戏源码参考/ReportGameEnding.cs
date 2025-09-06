using HutongGames.PlayMaker;

public class ReportGameEnding : FsmStateAction
{
	public override void OnEnter()
	{
		GameManager instance = GameManager.instance;
		if ((bool)instance)
		{
			instance.RecordGameComplete();
		}
	}
}

using HutongGames.PlayMaker;

[ActionCategory("Hollow Knight/GG")]
public class CheckGGBossLevel : FsmStateAction
{
	public FsmEvent notGG;

	public FsmEvent level1;

	public FsmEvent level2;

	public FsmEvent level3;

	public override void Reset()
	{
		notGG = null;
		level1 = null;
		level2 = null;
		level3 = null;
	}

	public override void OnEnter()
	{
		if ((bool)BossSceneController.Instance)
		{
			switch (BossSceneController.Instance.BossLevel)
			{
			case 0:
				base.Fsm.Event(level1);
				break;
			case 1:
				base.Fsm.Event(level2);
				break;
			case 2:
				base.Fsm.Event(level3);
				break;
			}
		}
		else
		{
			base.Fsm.Event(notGG);
		}
		Finish();
	}
}

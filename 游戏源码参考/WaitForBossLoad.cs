using HutongGames.PlayMaker;

[ActionCategory("Hollow Knight")]
public class WaitForBossLoad : FsmStateAction
{
	public FsmEvent sendEvent;

	public override void Reset()
	{
		sendEvent = null;
	}

	public override void OnEnter()
	{
		if (!WorldInfo.NameLooksLikeAdditiveLoadScene(base.Owner.scene.name) && (bool)GameManager.instance && SceneAdditiveLoadConditional.ShouldLoadBoss)
		{
			GameManager.BossLoad temp = null;
			temp = delegate
			{
				base.Fsm.Event(sendEvent);
				GameManager.instance.OnLoadedBoss -= temp;
				Finish();
			};
			GameManager.instance.OnLoadedBoss += temp;
		}
		else
		{
			Finish();
		}
	}
}

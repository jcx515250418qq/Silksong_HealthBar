using HutongGames.PlayMaker;

public sealed class ShopClosedGameMapUpdate : FsmStateAction
{
	public FsmEvent didUpdateEvent;

	public override void Reset()
	{
		didUpdateEvent = null;
	}

	public override void OnEnter()
	{
		GameManager instance = GameManager.instance;
		if ((bool)instance)
		{
			instance.DoShopCloseGameMapUpdate();
			base.Fsm.Event(didUpdateEvent);
		}
		Finish();
	}
}

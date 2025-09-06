using HutongGames.PlayMaker;

public class CheckSendEventLimit : FsmStateAction
{
	public FsmGameObject gameObject;

	public FsmEventTarget target;

	public FsmEvent trueEvent;

	public FsmEvent falseEvent;

	public override void Reset()
	{
		gameObject = new FsmGameObject();
		target = new FsmEventTarget();
		trueEvent = null;
		falseEvent = null;
	}

	public override void OnEnter()
	{
		if ((bool)gameObject.Value)
		{
			LimitSendEvents component = base.Owner.gameObject.GetComponent<LimitSendEvents>();
			if ((bool)component && !component.Add(gameObject.Value))
			{
				base.Fsm.Event(target, falseEvent);
			}
			else
			{
				base.Fsm.Event(target, trueEvent);
			}
		}
		Finish();
	}
}

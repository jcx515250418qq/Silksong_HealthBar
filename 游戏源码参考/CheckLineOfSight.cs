using HutongGames.PlayMaker;
using UnityEngine;

public class CheckLineOfSight : FsmStateAction
{
	public FsmOwnerDefault From;

	public FsmGameObject To;

	[UIHint(UIHint.LayerMask)]
	public FsmInt blockingLayerMask;

	[UIHint(UIHint.Variable)]
	public FsmBool storeResult;

	public FsmEvent inSightEvent;

	public FsmEvent noSightEvent;

	public FsmBool everyFrame;

	private bool isValid;

	private GameObject from;

	private GameObject to;

	public override void Reset()
	{
		From = null;
		To = null;
		blockingLayerMask = 8;
		storeResult = null;
		inSightEvent = null;
		noSightEvent = null;
		everyFrame = false;
	}

	public override void OnEnter()
	{
		from = From.GetSafe(this);
		isValid = from != null;
		if (isValid)
		{
			to = To.Value;
			isValid = to != null;
			_ = isValid;
		}
		DoCheck();
		if (!isValid || !everyFrame.Value)
		{
			Finish();
		}
	}

	private void DoCheck()
	{
		bool flag = false;
		if (isValid)
		{
			flag = !Physics2D.Linecast(from.transform.position, to.transform.position, blockingLayerMask.Value);
		}
		storeResult.Value = flag;
		if (flag)
		{
			Event(inSightEvent);
		}
		else
		{
			Event(noSightEvent);
		}
	}

	public override void OnUpdate()
	{
		if (!isValid)
		{
			Finish();
		}
		else
		{
			DoCheck();
		}
	}
}

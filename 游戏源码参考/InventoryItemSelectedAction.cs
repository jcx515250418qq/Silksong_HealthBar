using System;
using HutongGames.PlayMaker;

public class InventoryItemSelectedAction : FSMUtility.GetComponentFsmStateAction<InventoryItemManager>
{
	public enum ItemAction
	{
		Submit = 0,
		Cancel = 1,
		Option = 2,
		SubmitRelease = 3,
		Extra = 4,
		ExtraRelease = 5,
		Super = 6
	}

	[ObjectType(typeof(ItemAction))]
	public FsmEnum Action;

	public FsmEvent SuccessEvent;

	public FsmEvent FailureEvent;

	public override void Reset()
	{
		base.Reset();
		Action = null;
		SuccessEvent = null;
		FailureEvent = null;
	}

	protected override void DoAction(InventoryItemManager itemManager)
	{
		bool flag = false;
		switch ((ItemAction)(object)Action.Value)
		{
		case ItemAction.Submit:
			flag = itemManager.SubmitButtonSelected();
			break;
		case ItemAction.SubmitRelease:
			flag = itemManager.SubmitButtonReleaseSelected();
			break;
		case ItemAction.Cancel:
			flag = itemManager.CancelButtonSelected();
			break;
		case ItemAction.Extra:
			flag = itemManager.ExtraButtonSelected();
			break;
		case ItemAction.ExtraRelease:
			flag = itemManager.ExtraButtonReleaseSelected();
			break;
		case ItemAction.Super:
			flag = itemManager.SuperButtonSelected();
			break;
		default:
			throw new ArgumentOutOfRangeException();
		case ItemAction.Option:
			break;
		}
		base.Fsm.Event(flag ? SuccessEvent : FailureEvent);
	}
}

using HutongGames.PlayMaker;

public class GetJournalKillData : FsmStateAction
{
	[ObjectType(typeof(EnemyJournalRecord))]
	public FsmObject Record;

	[UIHint(UIHint.Variable)]
	public FsmInt KillCount;

	[UIHint(UIHint.Variable)]
	public FsmBool IsKilled;

	[UIHint(UIHint.Variable)]
	public FsmBool IsCompleted;

	public FsmEvent KilledEvent;

	public FsmEvent CompletedEvent;

	public override void Reset()
	{
		Record = null;
		KillCount = null;
		IsKilled = null;
		IsCompleted = null;
		KilledEvent = null;
		CompletedEvent = null;
	}

	public override void OnEnter()
	{
		if (!Record.IsNone)
		{
			EnemyJournalRecord obj = (EnemyJournalRecord)Record.Value;
			int killCount = obj.KillCount;
			int killsRequired = obj.KillsRequired;
			if (!KillCount.IsNone)
			{
				KillCount.Value = killCount;
			}
			if (!IsCompleted.IsNone)
			{
				IsCompleted.Value = killCount >= killsRequired;
				base.Fsm.Event(CompletedEvent);
			}
			if (!IsKilled.IsNone)
			{
				IsKilled.Value = killCount > 0;
				base.Fsm.Event(KilledEvent);
			}
		}
		Finish();
	}
}

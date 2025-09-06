using HutongGames.PlayMaker;

public class RecordJournalKill : FsmStateAction
{
	[ObjectType(typeof(EnemyJournalRecord))]
	public FsmObject Record;

	public override void Reset()
	{
		Record = null;
	}

	public override void OnEnter()
	{
		if (!Record.IsNone)
		{
			EnemyJournalRecord enemyJournalRecord = Record.Value as EnemyJournalRecord;
			if ((bool)enemyJournalRecord)
			{
				EnemyJournalManager.RecordKill(enemyJournalRecord);
			}
		}
		Finish();
	}
}

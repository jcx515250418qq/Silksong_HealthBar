using HutongGames.PlayMaker;

public class CompleteJournalRecordV2 : FsmStateAction
{
	[ObjectType(typeof(EnemyJournalRecord))]
	public FsmObject Record;

	public FsmBool ShowPopup;

	public FsmBool ForcePopup;

	public override void Reset()
	{
		Record = null;
		ShowPopup = true;
		ForcePopup = null;
	}

	public override void OnEnter()
	{
		if (!Record.IsNone)
		{
			EnemyJournalRecord enemyJournalRecord = Record.Value as EnemyJournalRecord;
			if ((bool)enemyJournalRecord)
			{
				while (enemyJournalRecord.KillCount < enemyJournalRecord.KillsRequired - 1)
				{
					EnemyJournalManager.RecordKill(enemyJournalRecord, showPopup: false);
				}
				EnemyJournalManager.RecordKill(enemyJournalRecord, ShowPopup.Value, ForcePopup.Value);
			}
		}
		Finish();
	}
}

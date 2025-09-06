using HutongGames.PlayMaker;

public class RecordJournalKillV2 : FsmStateAction
{
	[ObjectType(typeof(EnemyJournalRecord))]
	public FsmObject Record;

	public FsmBool ShowPopup;

	public override void Reset()
	{
		Record = null;
		ShowPopup = true;
	}

	public override void OnEnter()
	{
		if (!Record.IsNone)
		{
			EnemyJournalRecord enemyJournalRecord = Record.Value as EnemyJournalRecord;
			if ((bool)enemyJournalRecord)
			{
				EnemyJournalManager.RecordKill(enemyJournalRecord, ShowPopup.Value);
			}
		}
		Finish();
	}
}

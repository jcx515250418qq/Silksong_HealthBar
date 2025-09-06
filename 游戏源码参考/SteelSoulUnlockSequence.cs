public class SteelSoulUnlockSequence : AnimatorSequence
{
	public override bool ShouldShow
	{
		get
		{
			if (base.ShouldShow)
			{
				return GameManager.instance.GetStatusRecordInt("RecPermadeathMode") == 0;
			}
			return false;
		}
	}

	public override void Begin()
	{
		base.Begin();
		GameManager.instance.SetStatusRecordInt("RecPermadeathMode", 1);
		GameManager.instance.SaveStatusRecords();
	}
}

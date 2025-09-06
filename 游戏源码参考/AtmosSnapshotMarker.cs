public class AtmosSnapshotMarker : SnapshotMarker
{
	protected override void AddMarker()
	{
		AudioManager.AddAtmosMarker(this);
	}

	protected override void RemoveMarker()
	{
		AudioManager.RemoveAtmosMarker(this);
	}
}

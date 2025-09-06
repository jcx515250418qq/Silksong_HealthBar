using UnityEngine;

public class MusicSnapshotMarker : SnapshotMarker
{
	[SerializeField]
	private bool forceUpdateOnAdd;

	protected override void AddMarker()
	{
		AudioManager.AddMusicMarker(this);
		if (forceUpdateOnAdd)
		{
			AudioManager.ForceMarkerUpdate();
		}
	}

	protected override void RemoveMarker()
	{
		AudioManager.RemoveMusicMarker(this);
	}
}

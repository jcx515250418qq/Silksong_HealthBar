using UnityEngine;

public class WorldRumbleArea : TrackTriggerObjects, WorldRumbleManager.IWorldRumblePreventer
{
	[Space]
	[SerializeField]
	private bool allowRumble;

	public bool AllowRumble => allowRumble;

	protected override void OnInsideStateChanged(bool isInside)
	{
		GameCameras silentInstance = GameCameras.SilentInstance;
		if ((bool)silentInstance)
		{
			WorldRumbleManager worldRumbleManager = silentInstance.worldRumbleManager;
			if (isInside)
			{
				worldRumbleManager.AddPreventer(this);
			}
			else
			{
				worldRumbleManager.RemovePreventer(this);
			}
		}
	}
}

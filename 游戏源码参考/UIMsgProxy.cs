using UnityEngine;

public class UIMsgProxy : MonoBehaviour, WorldRumbleManager.IWorldRumblePreventer
{
	private bool wasDisablePause;

	public bool AllowRumble => false;

	public void SetIsInMsg(bool value)
	{
		WorldRumbleManager worldRumbleManager = GameCameras.SilentInstance.worldRumbleManager;
		if (value)
		{
			worldRumbleManager.AddPreventer(this);
			wasDisablePause = PlayerData.instance.disablePause;
			PlayerData.instance.disablePause = true;
			CollectableItemPickup.IsPickupPaused = true;
			InventoryPaneInput.IsInputBlocked = true;
		}
		else
		{
			worldRumbleManager.RemovePreventer(this);
			CollectableItemPickup.IsPickupPaused = false;
			InventoryPaneInput.IsInputBlocked = false;
			PlayerData.instance.disablePause = wasDisablePause;
		}
	}
}

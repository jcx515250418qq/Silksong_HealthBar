using UnityEngine;

public class WorldRumblePreventWhileActive : MonoBehaviour, WorldRumbleManager.IWorldRumblePreventer
{
	private bool hasStarted;

	private WorldRumbleManager manager;

	public bool AllowRumble => false;

	private void OnEnable()
	{
		if (!hasStarted)
		{
			return;
		}
		GameCameras silentInstance = GameCameras.SilentInstance;
		if ((bool)silentInstance)
		{
			manager = silentInstance.worldRumbleManager;
			if ((bool)manager)
			{
				manager.AddPreventer(this);
			}
		}
	}

	private void Start()
	{
		hasStarted = true;
		OnEnable();
	}

	private void OnDisable()
	{
		if ((bool)manager)
		{
			manager.RemovePreventer(this);
			manager = null;
		}
	}
}

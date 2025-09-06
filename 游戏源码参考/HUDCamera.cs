using UnityEngine;

public class HUDCamera : MonoBehaviour
{
	[SerializeField]
	private GameObject gameplayChild;

	[SerializeField]
	private InventoryMapManager mapManager;

	private GameCameras gc;

	private InputHandler ih;

	private bool shouldEnablePause;

	public GameObject GameplayChild => gameplayChild;

	private void OnEnable()
	{
		if (!gc)
		{
			gc = GameCameras.instance;
		}
		if (!ih)
		{
			ih = GameManager.instance.inputHandler;
		}
		if (ih.PauseAllowed)
		{
			shouldEnablePause = true;
			ih.PreventPause();
		}
		else
		{
			shouldEnablePause = false;
		}
		Invoke("MoveMenuToHudCamera", 0.5f);
	}

	private void MoveMenuToHudCamera()
	{
		gc.MoveMenuToHUDCamera();
		if (shouldEnablePause)
		{
			ih.AllowPause();
			shouldEnablePause = false;
		}
	}

	public void SetIsGameplayMode(bool isGameplayMode)
	{
		gameplayChild.SetActive(isGameplayMode);
	}

	public void EnsureGameMapSpawned()
	{
		mapManager.EnsureMapsSpawned();
	}
}

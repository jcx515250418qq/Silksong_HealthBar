using GlobalEnums;
using InControl;
using UnityEngine;

public class IdleMemoryCleaner : MonoBehaviour
{
	private const float IDLE_TIME_THRESHOLD = 300f;

	private const float CLEANUP_INTERVAL = 120f;

	private const float TWO_AXIS_INPUT_RELEVANCE_THRESHOLD = 0.3f;

	private float lastInteractionTime;

	private float lastCleanupTime;

	private bool isCleaning;

	private void OnEnable()
	{
		base.gameObject.hideFlags = HideFlags.HideAndDontSave;
		Object.DontDestroyOnLoad(this);
	}

	private void Update()
	{
		if (GameManager.instance.GameState switch
		{
			GameState.PLAYING => 1, 
			GameState.PAUSED => 1, 
			_ => 0, 
		} == 0)
		{
			lastInteractionTime = Time.realtimeSinceStartup;
			return;
		}
		if (HadAnyInputLastFrame())
		{
			UpdateLastInteractionTime();
		}
		float realtimeSinceStartup = Time.realtimeSinceStartup;
		bool flag = realtimeSinceStartup - lastInteractionTime >= 300f;
		bool flag2 = realtimeSinceStartup - lastCleanupTime <= 120f;
		if (!(!flag || flag2) && !isCleaning)
		{
			double num = (double)GCManager.GetMonoHeapUsage() / 1024.0 / 1024.0;
			double num2 = GCManager.HeapUsageThreshold * 0.5;
			if (!(num < num2))
			{
				CleanMemory();
			}
		}
	}

	private void UpdateLastInteractionTime()
	{
		lastInteractionTime = Time.realtimeSinceStartup;
	}

	private void CleanMemory()
	{
		isCleaning = true;
		GCManager.ForceCollect(blocking: true, compacting: true);
		lastCleanupTime = Time.realtimeSinceStartup;
		isCleaning = false;
	}

	private bool HadAnyInputLastFrame()
	{
		InputDevice gameController = ManagerSingleton<InputHandler>.Instance.gameController;
		if (gameController == null)
		{
			return false;
		}
		bool flag = (gameController.LeftStick?.Value.magnitude ?? 0f) > 0.3f;
		bool flag2 = (gameController.RightStick?.Value.magnitude ?? 0f) > 0.3f;
		bool flag3 = (gameController.DPad?.Value.magnitude ?? 0f) > 0.3f;
		bool flag4 = gameController.AnyButton?.WasPressed ?? false;
		bool anyKeyDown = Input.anyKeyDown;
		return flag || flag2 || flag3 || flag4 || anyKeyDown;
	}
}

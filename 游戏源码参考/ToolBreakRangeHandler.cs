using GlobalSettings;
using UnityEngine;
using UnityEngine.Events;

public class ToolBreakRangeHandler : MonoBehaviour
{
	private const float ENABLE_GRACE_TIME = 0.1f;

	public UnityEvent OnExitRange;

	public UnityEvent OnOutsideRange;

	public UnityEvent OnEnterRange;

	public UnityEvent OnInsideRange;

	private Transform camera;

	private float maxRange;

	private bool wasOutsideRange;

	private float graceTimeLeft;

	private void OnEnable()
	{
		camera = GameCameras.instance.mainCamera.transform;
		maxRange = Gameplay.ToolCameraDistanceBreak;
		graceTimeLeft = 0.1f;
		if (wasOutsideRange)
		{
			OnEnterRange.Invoke();
			wasOutsideRange = false;
		}
	}

	private void Update()
	{
		if (graceTimeLeft > 0f)
		{
			graceTimeLeft -= Time.deltaTime;
			return;
		}
		Vector2 a = camera.position;
		Vector2 b = base.transform.position;
		bool flag = Vector2.Distance(a, b) >= maxRange;
		if (flag)
		{
			if (!wasOutsideRange)
			{
				OnExitRange.Invoke();
			}
			OnOutsideRange.Invoke();
		}
		else
		{
			if (wasOutsideRange)
			{
				OnEnterRange.Invoke();
			}
			OnInsideRange.Invoke();
		}
		wasOutsideRange = flag;
	}
}

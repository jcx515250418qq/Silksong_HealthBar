using UnityEngine;

public sealed class VibrationManagerUpdater : MonoBehaviour
{
	private void Awake()
	{
		Object.DontDestroyOnLoad(base.gameObject);
	}

	private void Update()
	{
		if (!VibrationManager.Update())
		{
			base.enabled = false;
		}
	}
}

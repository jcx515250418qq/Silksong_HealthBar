using System.Collections.Generic;
using GlobalSettings;
using UnityEngine;

public class CameraControlAnimationEvents : MonoBehaviour
{
	public bool IsActive = true;

	[SerializeField]
	private bool requireVisible;

	[SerializeField]
	private bool vibrate = true;

	[SerializeField]
	private CameraManagerReference overrideCamera;

	[SerializeField]
	private bool sendWorldForce = true;

	private Renderer[] childRenderers;

	private List<CameraShakeProfile> startedRumbles;

	private CameraManagerReference CurrentCamera
	{
		get
		{
			if (!overrideCamera)
			{
				return GlobalSettings.Camera.MainCameraShakeManager;
			}
			return overrideCamera;
		}
	}

	private void Awake()
	{
		childRenderers = GetComponentsInChildren<Renderer>();
	}

	private void Start()
	{
		StopRumble();
	}

	private void OnDisable()
	{
		StopRumble();
	}

	public void BigShake()
	{
		DoShake(GlobalSettings.Camera.BigShake);
	}

	public void BigShakeQuick()
	{
		DoShake(GlobalSettings.Camera.BigShakeQuick);
	}

	public void TinyShake()
	{
		DoShake(GlobalSettings.Camera.TinyShake);
	}

	public void SmallShake()
	{
		DoShake(GlobalSettings.Camera.SmallShake);
	}

	public void AverageShake()
	{
		DoShake(GlobalSettings.Camera.AverageShake);
	}

	public void AverageShakeQuick()
	{
		DoShake(GlobalSettings.Camera.AverageShakeQuick);
	}

	public void EnemyKillShake()
	{
		DoShake(GlobalSettings.Camera.EnemyKillShake);
	}

	public void TinyRumble()
	{
		if (DoShake(GlobalSettings.Camera.TinyRumble))
		{
			TrackRumble(GlobalSettings.Camera.TinyRumble);
		}
	}

	public void SmallRumble()
	{
		if (DoShake(GlobalSettings.Camera.SmallRumble))
		{
			TrackRumble(GlobalSettings.Camera.SmallRumble);
		}
	}

	public void MedRumble()
	{
		if (DoShake(GlobalSettings.Camera.MedRumble))
		{
			TrackRumble(GlobalSettings.Camera.MedRumble);
		}
	}

	public void BigRumble()
	{
		if (DoShake(GlobalSettings.Camera.BigRumble))
		{
			TrackRumble(GlobalSettings.Camera.BigRumble);
		}
	}

	public void StopRumble()
	{
		if (startedRumbles == null)
		{
			return;
		}
		foreach (CameraShakeProfile startedRumble in startedRumbles)
		{
			CancelShake(startedRumble);
		}
		startedRumbles.Clear();
	}

	private void TrackRumble(CameraShakeProfile profile)
	{
		if (startedRumbles == null)
		{
			startedRumbles = new List<CameraShakeProfile>();
		}
		startedRumbles.Add(profile);
	}

	private bool DoShake(CameraShakeProfile profile)
	{
		if (!IsActive)
		{
			return false;
		}
		if (requireVisible)
		{
			bool flag = false;
			Renderer[] array = childRenderers;
			foreach (Renderer renderer in array)
			{
				if ((bool)renderer && renderer.isVisible)
				{
					flag = true;
					break;
				}
			}
			if (!flag)
			{
				return false;
			}
		}
		if (!base.enabled)
		{
			return false;
		}
		CurrentCamera.DoShake(profile, this, doFreeze: false, vibrate, sendWorldForce);
		return true;
	}

	private void CancelShake(ICameraShake profile)
	{
		CurrentCamera.CancelShake(profile);
	}
}

using UnityEngine;

public class CameraShakeAnimator : MonoBehaviour
{
	[SerializeField]
	private GameObject requiredVisible;

	[SerializeField]
	private TrackTriggerObjects range;

	[SerializeField]
	private CameraShakeTarget[] cameraShakeTargets;

	private Renderer[] requiredVisibleRenderers;

	private void Awake()
	{
		if ((bool)requiredVisible)
		{
			requiredVisibleRenderers = requiredVisible.GetComponentsInChildren<Renderer>();
		}
	}

	private bool CanShake()
	{
		if ((bool)range && !range.IsInside)
		{
			return false;
		}
		if (requiredVisibleRenderers == null)
		{
			return true;
		}
		Renderer[] array = requiredVisibleRenderers;
		for (int i = 0; i < array.Length; i++)
		{
			if (array[i].isVisible)
			{
				return true;
			}
		}
		return false;
	}

	public void DoCameraShake(int index)
	{
		if (Application.isPlaying && CanShake() && index >= 0 && index < cameraShakeTargets.Length)
		{
			cameraShakeTargets[index].DoShake(this);
		}
	}

	public void CancelCameraShake(int index)
	{
		if (Application.isPlaying && index >= 0 && index < cameraShakeTargets.Length)
		{
			cameraShakeTargets[index].CancelShake();
		}
	}
}

using System;
using TeamCherry.SharedUtils;
using UnityEngine;

public sealed class ScaleWithAspect : MonoBehaviour
{
	[Serializable]
	private class OverrideVector3 : OverrideValue<Vector3>
	{
	}

	[SerializeField]
	private float baseAspect = 1.7777778f;

	[SerializeField]
	private Vector3 baseScale = new Vector3(1f, 1f, 1f);

	[SerializeField]
	private OverrideVector3 minScale;

	[SerializeField]
	private OverrideVector3 maxScale;

	private void OnEnable()
	{
		Camera mainCamera = GameCameras.instance.mainCamera;
		if ((bool)mainCamera)
		{
			OnCameraAspectChanged(mainCamera.aspect);
		}
		ForceCameraAspect.ViewportAspectChanged += OnCameraAspectChanged;
	}

	private void OnDisable()
	{
		ForceCameraAspect.ViewportAspectChanged -= OnCameraAspectChanged;
	}

	[ContextMenu("Record Scale")]
	private void RecordScale()
	{
		baseScale = base.transform.localScale;
	}

	private void OnCameraAspectChanged(float currentAspect)
	{
		float num = baseAspect / currentAspect;
		Vector3 vector = new Vector3(baseScale.x * num, baseScale.y * num, baseScale.z * num);
		if (minScale.IsEnabled)
		{
			vector = Vector3.Max(vector, minScale.Value);
		}
		if (maxScale.IsEnabled)
		{
			vector = Vector3.Min(vector, maxScale.Value);
		}
		base.transform.localScale = vector;
	}
}

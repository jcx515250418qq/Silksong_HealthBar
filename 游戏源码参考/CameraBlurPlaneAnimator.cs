using UnityEngine;

public class CameraBlurPlaneAnimator : MonoBehaviour
{
	public float Spacing;

	public float Vibrancy;

	private float oldSpacing;

	private float oldVibrancy;

	private void OnEnable()
	{
		CameraBlurPlane.Spacing = Spacing;
		CameraBlurPlane.Vibrancy = Vibrancy;
	}

	private void LateUpdate()
	{
		if (!Mathf.Approximately(Spacing, oldSpacing))
		{
			oldSpacing = Spacing;
			CameraBlurPlane.Spacing = Spacing;
		}
		if (!Mathf.Approximately(Vibrancy, oldVibrancy))
		{
			oldVibrancy = Vibrancy;
			CameraBlurPlane.Vibrancy = Vibrancy;
		}
	}
}

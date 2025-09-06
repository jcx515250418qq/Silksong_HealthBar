using TeamCherry.SharedUtils;
using UnityEngine;

public class CameraAspectScaler : MonoBehaviour
{
	[SerializeField]
	private Vector3 minAspectScale = Vector3.one;

	[SerializeField]
	private Vector3 maxAspectScale = Vector3.one;

	private void OnEnable()
	{
		ForceCameraAspect.ViewportAspectChanged += OnCameraAspectChanged;
		OnCameraAspectChanged(ForceCameraAspect.CurrentViewportAspect);
	}

	private void OnDisable()
	{
		ForceCameraAspect.ViewportAspectChanged -= OnCameraAspectChanged;
	}

	private void OnCameraAspectChanged(float aspect)
	{
		float tBetween = new MinMaxFloat(1.7777778f, 2.3916667f).GetTBetween(aspect);
		Vector3 localScale = Vector3.Lerp(minAspectScale, maxAspectScale, tBetween);
		base.transform.localScale = localScale;
	}
}

using UnityEngine;

public class ScreenHeightAdjustPos : MonoBehaviour
{
	[SerializeField]
	private float heightOffset;

	private Vector3 initialLocalPos;

	private void Awake()
	{
		initialLocalPos = base.transform.localPosition;
	}

	private void OnEnable()
	{
		ForceCameraAspect.MainCamHeightMultChanged += OnMainCamHeightMultChanged;
		OnMainCamHeightMultChanged(ForceCameraAspect.CurrentMainCamHeightMult);
	}

	private void OnDisable()
	{
		ForceCameraAspect.MainCamHeightMultChanged -= OnMainCamHeightMultChanged;
	}

	private void OnMainCamHeightMultChanged(float heightMult)
	{
		float num = heightMult - 1f;
		Vector3 localPosition = initialLocalPos;
		localPosition.y += heightOffset * num;
		base.transform.localPosition = localPosition;
	}
}

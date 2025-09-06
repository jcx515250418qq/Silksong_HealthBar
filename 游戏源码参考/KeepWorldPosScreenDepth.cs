using UnityEngine;

public class KeepWorldPosScreenDepth : MonoBehaviour
{
	[SerializeField]
	private CameraRenderHooks.CameraSource targetCamera = CameraRenderHooks.CameraSource.MainCamera;

	[SerializeField]
	private float renderZ;

	private CameraRenderHooks.CameraSource lastSource;

	private Camera mainCamera;

	private Vector3 preCullPos;

	private Vector3 preCullScale;

	private void Awake()
	{
		mainCamera = GameCameras.instance.mainCamera;
	}

	private void OnEnable()
	{
		CameraRenderHooks.CameraPreCull += OnCameraPreCull;
		CameraRenderHooks.CameraPostRender += OnCameraPostRender;
	}

	private void OnDisable()
	{
		CameraRenderHooks.CameraPreCull -= OnCameraPreCull;
		CameraRenderHooks.CameraPostRender -= OnCameraPostRender;
	}

	private void OnCameraPreCull(CameraRenderHooks.CameraSource cameraType)
	{
		lastSource = (CameraRenderHooks.CameraSource)(-1);
		if (base.isActiveAndEnabled)
		{
			Transform transform = base.transform;
			preCullPos = transform.position;
			preCullScale = transform.localScale;
			lastSource = cameraType;
			if (cameraType != targetCamera)
			{
				transform.position = new Vector3(-2000f, -2000f);
				transform.localScale = new Vector3(0.001f, 0.001f, 1f);
				return;
			}
			Vector3 position = mainCamera.transform.position;
			float num = preCullPos.z - position.z;
			float num2 = (renderZ - position.z) / num;
			Vector3 vector = mainCamera.WorldToViewportPoint(preCullPos);
			Vector3 position2 = new Vector3(vector.x, vector.y, vector.z * num2);
			Vector3 position3 = mainCamera.ViewportToWorldPoint(position2);
			transform.position = position3;
			transform.localScale = preCullScale * num2;
		}
	}

	private void OnCameraPostRender(CameraRenderHooks.CameraSource cameraType)
	{
		if (cameraType == lastSource)
		{
			Transform obj = base.transform;
			obj.position = preCullPos;
			obj.localScale = preCullScale;
		}
	}
}

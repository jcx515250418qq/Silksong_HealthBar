using UnityEngine;

public class AmbientFloat : MonoBehaviour
{
	[SerializeField]
	private AmbientFloatProfile profile;

	[SerializeField]
	private SpeedChanger speedController;

	[SerializeField]
	private float speedMultiplier = 1f;

	[SerializeField]
	[ModifiableProperty]
	[Conditional("IsInEditMode", true, true, false)]
	private bool useCameraRenderHooks;

	[SerializeField]
	private float fpsLimit;

	private float time;

	private float updateOffset;

	private float timeOffset;

	private double nextUpdateTime;

	private Vector3 initialPosition;

	private Vector3 targetPosition;

	private Vector3 preCullPosition;

	public float SpeedMultiplier
	{
		get
		{
			return speedMultiplier;
		}
		set
		{
			speedMultiplier = value;
		}
	}

	private bool IsInEditMode()
	{
		return !Application.isPlaying;
	}

	private void Awake()
	{
		if ((bool)speedController)
		{
			speedController.SpeedChanged += delegate(float speed)
			{
				speedMultiplier = speed;
			};
		}
	}

	private void Start()
	{
		initialPosition = base.transform.localPosition;
		targetPosition = initialPosition;
		timeOffset = Random.Range(-10f, 10f);
		if (useCameraRenderHooks)
		{
			CameraRenderHooks.CameraPreCull += OnCameraPreCull;
			CameraRenderHooks.CameraPostRender += OnCameraPostRender;
		}
	}

	private void OnDestroy()
	{
		if (useCameraRenderHooks)
		{
			CameraRenderHooks.CameraPreCull -= OnCameraPreCull;
			CameraRenderHooks.CameraPostRender -= OnCameraPostRender;
		}
	}

	private void OnEnable()
	{
		if (!profile)
		{
			base.enabled = false;
		}
		updateOffset = Random.Range(0f, 1f / fpsLimit);
	}

	private void Update()
	{
		if (speedMultiplier <= 0f)
		{
			return;
		}
		float deltaTime = Time.deltaTime;
		time += deltaTime * speedMultiplier;
		double num = Time.timeAsDouble + (double)updateOffset;
		if (fpsLimit > 0f)
		{
			if (num < nextUpdateTime)
			{
				return;
			}
			nextUpdateTime = num + (double)(1f / fpsLimit);
		}
		targetPosition = initialPosition + profile.GetOffset(time, timeOffset);
		if (!useCameraRenderHooks)
		{
			base.transform.localPosition = targetPosition;
		}
	}

	private void OnCameraPreCull(CameraRenderHooks.CameraSource cameraType)
	{
		if (cameraType == CameraRenderHooks.CameraSource.MainCamera && base.isActiveAndEnabled && !(Time.timeScale <= Mathf.Epsilon))
		{
			preCullPosition = base.transform.localPosition;
			base.transform.localPosition = targetPosition;
		}
	}

	private void OnCameraPostRender(CameraRenderHooks.CameraSource cameraType)
	{
		if (cameraType == CameraRenderHooks.CameraSource.MainCamera && base.isActiveAndEnabled && !(Time.timeScale <= Mathf.Epsilon))
		{
			base.transform.localPosition = preCullPosition;
		}
	}
}

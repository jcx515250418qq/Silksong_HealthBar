using System;
using System.Collections;
using TeamCherry.SharedUtils;
using UnityEngine;

public class ForceCameraAspect : MonoBehaviour
{
	[SerializeField]
	private Transform anchorTopLeft;

	[SerializeField]
	private Camera clearCamera;

	private tk2dCamera tk2dCam;

	private Camera hudCam;

	private float initialFov;

	private float initialHudCamSize;

	private int lastX;

	private int lastY;

	private float scaleAdjust;

	private float fovOffset;

	private float extraFovOffset;

	private Coroutine fovTransitionRoutine;

	public static float CurrentViewportAspect { get; private set; }

	public static float CurrentMainCamHeightMult { get; private set; }

	public static float CurrentMainCamFov { get; private set; }

	public static event Action<float> ViewportAspectChanged;

	public static event Action<float> MainCamHeightMultChanged;

	public static event Action<float> MainCamFovChanged;

	private void Awake()
	{
		tk2dCam = GetComponent<tk2dCamera>();
		CurrentViewportAspect = 1.7777778f;
		clearCamera.enabled = false;
	}

	private void Start()
	{
		hudCam = GameCameras.instance.hudCamera;
		initialFov = tk2dCam.CameraSettings.fieldOfView;
		initialHudCamSize = hudCam.orthographicSize;
		AutoScaleViewport();
	}

	private void Update()
	{
		if (lastX != Screen.width || lastY != Screen.height)
		{
			float num = AutoScaleViewport();
			lastX = Screen.width;
			lastY = Screen.height;
			ForceCameraAspect.ViewportAspectChanged?.Invoke(num);
			CurrentViewportAspect = num;
		}
	}

	public void SetOverscanViewport(float adjustment)
	{
		scaleAdjust = adjustment;
		AutoScaleViewport();
	}

	private float AutoScaleViewport()
	{
		float num = (float)Screen.width / (float)Screen.height;
		float clampedBetween = new MinMaxFloat(1.6f, 2.3916667f).GetClampedBetween(num);
		float num2 = num / clampedBetween;
		float num3 = 1f + scaleAdjust;
		Rect rect = tk2dCam.CameraSettings.rect;
		if (num2 < 1f)
		{
			rect.width = 1f * num3;
			rect.height = num2 * num3;
			float x = (1f - rect.width) / 2f;
			rect.x = x;
			float y = (1f - rect.height) / 2f;
			rect.y = y;
		}
		else
		{
			float num4 = 1f / num2;
			rect.width = num4 * num3;
			rect.height = 1f * num3;
			float x2 = (1f - rect.width) / 2f;
			rect.x = x2;
			float y2 = (1f - rect.height) / 2f;
			rect.y = y2;
		}
		tk2dCam.CameraSettings.rect = rect;
		hudCam.rect = rect;
		float num5 = ((!(clampedBetween < 1.7777778f)) ? 1f : (1.7777778f / clampedBetween));
		ForceCameraAspect.MainCamHeightMultChanged?.Invoke(num5);
		CurrentMainCamHeightMult = num5;
		float num6 = (initialFov + fovOffset + extraFovOffset) * num5;
		tk2dCam.CameraSettings.fieldOfView = num6;
		ForceCameraAspect.MainCamFovChanged?.Invoke(num6);
		CurrentMainCamFov = num6;
		hudCam.orthographicSize = initialHudCamSize * num5;
		if ((bool)anchorTopLeft)
		{
			anchorTopLeft.localPosition = new Vector3(0f, hudCam.orthographicSize - initialHudCamSize, 0f);
		}
		clearCamera.enabled = rect.x > Mathf.Epsilon || rect.y > Mathf.Epsilon;
		return clampedBetween;
	}

	public void SetFovOffset(float offset, float transitionTime, AnimationCurve curve)
	{
		if (fovTransitionRoutine != null)
		{
			StopCoroutine(fovTransitionRoutine);
			fovTransitionRoutine = null;
		}
		if (!Mathf.Approximately(offset, fovOffset))
		{
			if (transitionTime <= Mathf.Epsilon)
			{
				fovOffset = offset;
				AutoScaleViewport();
			}
			else
			{
				fovTransitionRoutine = StartCoroutine(TransitionFovOffset(offset, transitionTime, curve));
			}
		}
	}

	private IEnumerator TransitionFovOffset(float newOffset, float transitionTime, AnimationCurve curve)
	{
		float initialOffset = fovOffset;
		for (float elapsed = 0f; elapsed < transitionTime; elapsed += Time.deltaTime)
		{
			float t = curve.Evaluate(elapsed / transitionTime);
			fovOffset = Mathf.Lerp(initialOffset, newOffset, t);
			AutoScaleViewport();
			yield return null;
		}
		fovOffset = newOffset;
		AutoScaleViewport();
		fovTransitionRoutine = null;
	}

	public void SetExtraFovOffset(float value)
	{
		extraFovOffset = value;
		AutoScaleViewport();
	}
}

using System;
using UnityEngine;

public class CameraRenderHooks : MonoBehaviour
{
	public enum CameraSource
	{
		Misc = 0,
		MainCamera = 1,
		HudCamera = 2
	}

	[SerializeField]
	private CameraSource cameraType;

	public static event Action<CameraSource> CameraPreCull;

	public static event Action<CameraSource> CameraPostRender;

	private void OnPreCull()
	{
		CameraRenderHooks.CameraPreCull?.Invoke(cameraType);
	}

	private void OnPostRender()
	{
		CameraRenderHooks.CameraPostRender?.Invoke(cameraType);
	}
}

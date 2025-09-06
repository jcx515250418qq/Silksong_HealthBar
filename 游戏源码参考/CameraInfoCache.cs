using System;
using UnityEngine;

public static class CameraInfoCache
{
	private static int cachedFrameCount = -1;

	private static float lastUnitsAspect = -1f;

	private static float lastUnitsZ = float.NaN;

	public static float Aspect { get; private set; }

	public static float PosX { get; private set; }

	public static float PosY { get; private set; }

	public static float PosZ { get; private set; }

	public static float UnitsWidth { get; private set; }

	public static float UnitsHeight { get; private set; }

	public static float HalfWidth { get; private set; }

	public static float HalfHeight { get; private set; }

	public static void UpdateCache()
	{
		if (Time.frameCount == cachedFrameCount)
		{
			return;
		}
		cachedFrameCount = Time.frameCount;
		GameCameras instance = GameCameras.instance;
		if (instance == null)
		{
			return;
		}
		Camera mainCamera = instance.mainCamera;
		if (mainCamera == null)
		{
			return;
		}
		Aspect = mainCamera.aspect;
		Vector3 position = mainCamera.transform.position;
		PosX = position.x;
		PosY = position.y;
		PosZ = position.z;
		if (!Mathf.Approximately(Aspect, lastUnitsAspect) || !Mathf.Approximately(PosZ, lastUnitsZ))
		{
			lastUnitsAspect = Aspect;
			lastUnitsZ = PosZ;
			float num = Mathf.Abs(PosZ);
			if (mainCamera.orthographic)
			{
				UnitsHeight = mainCamera.orthographicSize * 2f;
				UnitsWidth = UnitsHeight * Aspect;
			}
			else
			{
				float num2 = mainCamera.fieldOfView * (MathF.PI / 180f);
				UnitsHeight = 2f * num * Mathf.Tan(num2 / 2f);
				UnitsWidth = UnitsHeight * Aspect;
			}
			HalfWidth = UnitsWidth * 0.5f;
			HalfHeight = UnitsHeight * 0.5f;
		}
	}

	public static bool IsWithinBounds(Vector2 position, Vector2 buffer)
	{
		UpdateCache();
		if (Mathf.Abs(PosX - position.x) > HalfWidth + buffer.x)
		{
			return false;
		}
		if (Mathf.Abs(PosY - position.y) > HalfHeight + buffer.y)
		{
			return false;
		}
		return true;
	}
}

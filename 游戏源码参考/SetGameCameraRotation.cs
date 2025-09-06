using System;
using System.Collections.Generic;
using UnityEngine;

public class SetGameCameraRotation : MonoBehaviour
{
	[SerializeField]
	private float rotation;

	private float previousRotation;

	private static readonly List<SetGameCameraRotation> _activeRotations = new List<SetGameCameraRotation>();

	private void OnEnable()
	{
		previousRotation = rotation;
		_activeRotations.Add(this);
		UpdateRotation();
	}

	private void OnDisable()
	{
		_activeRotations.Remove(this);
		UpdateRotation();
	}

	private void LateUpdate()
	{
		if (!(Math.Abs(rotation - previousRotation) <= Mathf.Epsilon))
		{
			previousRotation = rotation;
			UpdateRotation();
		}
	}

	private static void UpdateRotation()
	{
		GameCameras instance = GameCameras.instance;
		if ((bool)instance)
		{
			Transform t = instance.mainCamera.transform;
			if (_activeRotations.Count == 0)
			{
				t.SetLocalRotation2D(0f);
				return;
			}
			List<SetGameCameraRotation> activeRotations = _activeRotations;
			SetGameCameraRotation setGameCameraRotation = activeRotations[activeRotations.Count - 1];
			t.SetLocalRotation2D(setGameCameraRotation.rotation);
		}
	}
}

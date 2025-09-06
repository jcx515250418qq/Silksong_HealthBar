using UnityEngine;

public interface ICameraShake
{
	bool CanFinish { get; }

	bool PersistThroughScenes => false;

	int FreezeFrames { get; }

	ICameraShakeVibration CameraShakeVibration { get; }

	CameraShakeWorldForceIntensities WorldForceOnStart { get; }

	float Magnitude { get; }

	Vector2 GetOffset(float elapsedTime);

	bool IsDone(float elapsedTime);
}

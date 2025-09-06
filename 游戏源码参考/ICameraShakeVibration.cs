public interface ICameraShakeVibration
{
	VibrationEmission PlayVibration(bool isRealtime);

	float GetVibrationStrength(float timeElapsed);
}

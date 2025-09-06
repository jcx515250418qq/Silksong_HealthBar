public abstract class VibrationMixer
{
	public abstract bool IsPaused { get; set; }

	public abstract int PlayingEmissionCount { get; }

	public abstract VibrationEmission PlayEmission(VibrationData vibrationData, VibrationTarget vibrationTarget, bool isLooping, string tag, bool isRealtime);

	public virtual VibrationEmission PlayEmission(VibrationEmission emission)
	{
		return emission;
	}

	public abstract VibrationEmission GetPlayingEmission(int playingEmissionIndex);

	public abstract void StopAllEmissions();

	public abstract void StopAllEmissionsWithTag(string tag);
}

public abstract class VibrationEmission
{
	protected float strength = 1f;

	public abstract VibrationTarget Target { get; set; }

	public abstract bool IsLooping { get; set; }

	public abstract string Tag { get; set; }

	public abstract bool IsRealtime { get; }

	public virtual float BaseStrength { get; set; } = 1f;

	public virtual float Strength
	{
		get
		{
			return strength * BaseStrength;
		}
		set
		{
			strength = value;
		}
	}

	public virtual float Speed { get; set; } = 1f;

	public virtual float Time { get; set; }

	public abstract bool IsPlaying { get; }

	public abstract void Play();

	public abstract void Stop();

	public void SetStrength(float value)
	{
		Strength = value;
	}

	public void SetPlaybackTime(float time)
	{
		Time = time;
	}

	public void SetSpeed(float speed)
	{
		Speed = speed;
	}
}

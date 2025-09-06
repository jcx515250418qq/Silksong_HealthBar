using System;

public struct PlayTime
{
	public float RawTime;

	private TimeSpan Time => TimeSpan.FromSeconds(RawTime);

	public float Hours => (float)Math.Floor(Time.TotalHours);

	public float Minutes => Time.Minutes;

	public float Seconds => Time.Seconds;

	public bool HasHours => Time.TotalHours >= 1.0;

	public bool HasMinutes => Time.TotalMinutes >= 1.0;

	public PlayTime(float rawTime)
	{
		RawTime = rawTime;
	}

	public override string ToString()
	{
		return $"{(int)Hours:0}:{(int)Minutes:00}";
	}
}

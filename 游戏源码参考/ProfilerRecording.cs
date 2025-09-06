using UnityEngine;

internal class ProfilerRecording
{
	private int count;

	private float startTime;

	private float accumulatedTime;

	private bool started;

	public string id;

	public float Seconds => accumulatedTime;

	public int Count => count;

	public ProfilerRecording(string id)
	{
		this.id = id;
	}

	public void Start()
	{
		if (started)
		{
			BalanceError();
		}
		count++;
		started = true;
		startTime = Time.realtimeSinceStartup;
	}

	public void Stop()
	{
		float realtimeSinceStartup = Time.realtimeSinceStartup;
		if (!started)
		{
			BalanceError();
		}
		started = false;
		float num = realtimeSinceStartup - startTime;
		accumulatedTime += num;
	}

	public void Reset()
	{
		accumulatedTime = 0f;
		count = 0;
		started = false;
	}

	private void BalanceError()
	{
		Debug.LogError("ProfilerRecording start/stops not balanced for '" + id + "'");
	}
}

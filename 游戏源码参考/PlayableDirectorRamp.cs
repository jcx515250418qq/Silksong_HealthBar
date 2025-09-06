using System;
using System.Collections;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Playables;

public class PlayableDirectorRamp : SpeedChanger
{
	[Serializable]
	public class UnityFloatEvent : UnityEvent<float>
	{
	}

	[SerializeField]
	private PlayableDirector director;

	[SerializeField]
	private float startTime;

	[SerializeField]
	private float loopDuration;

	[SerializeField]
	private float rampUpDuration;

	[SerializeField]
	private float rampDownDuration;

	[Space]
	[SerializeField]
	private float fpsLimit;

	[Space]
	public UnityEvent OnPlayed;

	public UnityFloatEvent OnSpeedChanged;

	public UnityEvent OnStopping;

	public UnityEvent OnStopped;

	public UnityEvent OnReset;

	private bool isRunning;

	private bool isReversed;

	private Coroutine runRoutine;

	private double DirectorTime
	{
		get
		{
			if (!director)
			{
				return 0.0;
			}
			return director.time;
		}
		set
		{
			if ((bool)director)
			{
				director.time = value;
				director.Evaluate();
			}
		}
	}

	private void OnEnable()
	{
		runRoutine = StartCoroutine(RunPlayable());
	}

	[ContextMenu("Play", true)]
	[ContextMenu("Stop", true)]
	[ContextMenu("Reset", true)]
	public bool CanPlay()
	{
		if (Application.isPlaying)
		{
			return director;
		}
		return false;
	}

	[ContextMenu("Play", false, 0)]
	public void Play()
	{
		isRunning = true;
	}

	public void Play(bool isReversed)
	{
		isRunning = true;
		this.isReversed = isReversed;
	}

	[ContextMenu("Stop", false, 1)]
	public void Stop()
	{
		isRunning = false;
	}

	[ContextMenu("Reset", false, 2)]
	public void ResetDirector()
	{
		if ((bool)director)
		{
			director.enabled = true;
			director.timeUpdateMode = DirectorUpdateMode.Manual;
			DirectorTime = startTime;
			SendSpeedChanged(0f);
			if (OnReset != null)
			{
				OnReset.Invoke();
			}
		}
	}

	private IEnumerator RunPlayable()
	{
		yield return null;
		ResetDirector();
		while (true)
		{
			if (!isRunning)
			{
				yield return null;
				continue;
			}
			if (OnPlayed != null)
			{
				OnPlayed.Invoke();
			}
			WaitForSeconds wait = ((!(fpsLimit > 0f)) ? null : new WaitForSeconds(1f / fpsLimit));
			float elapsed = 0f;
			float num;
			for (num = 0f; elapsed <= rampUpDuration; elapsed += num)
			{
				float num2 = elapsed / rampUpDuration;
				DirectorTime += Mathf.Lerp(0f, num, num2) * (float)((!isReversed) ? 1 : (-1));
				SendSpeedChanged(num2);
				double previousTime = Time.timeAsDouble;
				yield return wait;
				num = (float)(Time.timeAsDouble - previousTime);
			}
			SendSpeedChanged(1f);
			elapsed = 0f;
			num = 0f;
			while (isRunning && (loopDuration == 0f || elapsed <= loopDuration))
			{
				DirectorTime += num * (float)((!isReversed) ? 1 : (-1));
				double previousTime = Time.timeAsDouble;
				yield return wait;
				num = (float)(Time.timeAsDouble - previousTime);
				elapsed += num;
			}
			isRunning = false;
			if (OnStopping != null)
			{
				OnStopping.Invoke();
			}
			elapsed = 0f;
			for (num = 0f; elapsed <= rampDownDuration; elapsed += num)
			{
				float num3 = elapsed / rampDownDuration;
				DirectorTime += Mathf.Lerp(num, 0f, num3) * (float)((!isReversed) ? 1 : (-1));
				SendSpeedChanged(1f - num3);
				double previousTime = Time.timeAsDouble;
				yield return wait;
				num = (float)(Time.timeAsDouble - previousTime);
			}
			SendSpeedChanged(0f);
			if (OnStopped != null)
			{
				OnStopped.Invoke();
			}
		}
	}

	private void SendSpeedChanged(float speed)
	{
		if (OnSpeedChanged != null)
		{
			OnSpeedChanged.Invoke(speed);
		}
		CallSpeedChangedEvent(speed);
	}
}

using System.Collections.Generic;
using UnityEngine;

public static class TimeManager
{
	public delegate void TimeScaleUpdateDelegate(float timeScale);

	public sealed class TimeControlInstance
	{
		public enum Type
		{
			Multiplicative = 0,
			MinValue = 1
		}

		private float timeScale = 1f;

		private Type controlType;

		public float TimeScale
		{
			get
			{
				return timeScale;
			}
			set
			{
				timeScale = value;
				UpdateTimeScale();
			}
		}

		public Type ControlType
		{
			get
			{
				return controlType;
			}
			set
			{
				controlType = value;
				UpdateTimeScale();
			}
		}

		~TimeControlInstance()
		{
			Release();
		}

		public TimeControlInstance(float timeScale, Type controlType)
		{
			this.timeScale = timeScale;
			this.controlType = controlType;
			_timeControlInstances.Add(this);
			UpdateTimeScale();
		}

		public void Release()
		{
			if (_timeControlInstances.Remove(this))
			{
				UpdateTimeScale();
			}
		}
	}

	private static float _timeScale = 1f;

	private static float _debugTimeScale = 1f;

	private static float _cameraShakeTimeScale = 1f;

	private static float _platformBackgroundTimeScale = 1f;

	private static float _cheatMenuTimeScale = 0f;

	private static bool _isCheatMenuOpen;

	private static List<TimeControlInstance> _timeControlInstances = new List<TimeControlInstance>(20);

	public static float TimeScale
	{
		get
		{
			return _timeScale;
		}
		set
		{
			_timeScale = Mathf.Max(0f, value);
			UpdateTimeScale();
		}
	}

	public static float DebugTimeScale
	{
		get
		{
			return _debugTimeScale;
		}
		set
		{
			_debugTimeScale = Mathf.Clamp(value, 0f, 100f);
			UpdateTimeScale();
		}
	}

	public static float CameraShakeTimeScale
	{
		get
		{
			return _cameraShakeTimeScale;
		}
		set
		{
			_cameraShakeTimeScale = Mathf.Max(0f, value);
			UpdateTimeScale();
		}
	}

	public static float PlatformBackgroundTimeScale
	{
		get
		{
			return _platformBackgroundTimeScale;
		}
		set
		{
			_platformBackgroundTimeScale = Mathf.Max(0f, value);
			UpdateTimeScale();
		}
	}

	public static bool IsCheatMenuOpen
	{
		get
		{
			return _isCheatMenuOpen;
		}
		set
		{
			_isCheatMenuOpen = value;
			UpdateTimeScale();
		}
	}

	public static float CheatMenuTimeScale
	{
		get
		{
			return _cheatMenuTimeScale;
		}
		set
		{
			_cheatMenuTimeScale = Mathf.Max(value, 0f);
			UpdateTimeScale();
		}
	}

	public static event TimeScaleUpdateDelegate OnTimeScaleUpdated;

	private static void UpdateTimeScale()
	{
		float num = ((!_isCheatMenuOpen) ? (TimeScale * CameraShakeTimeScale * DebugTimeScale * PlatformBackgroundTimeScale) : _cheatMenuTimeScale);
		if (_timeControlInstances.Count > 0)
		{
			for (int i = 0; i < _timeControlInstances.Count; i++)
			{
				TimeControlInstance timeControlInstance = _timeControlInstances[i];
				switch (timeControlInstance.ControlType)
				{
				case TimeControlInstance.Type.Multiplicative:
					num *= timeControlInstance.TimeScale;
					break;
				case TimeControlInstance.Type.MinValue:
					num = Mathf.Min(num, timeControlInstance.TimeScale);
					break;
				}
			}
		}
		float num2 = Mathf.Max(0f, num);
		if (Time.timeScale != num2)
		{
			Time.timeScale = num2;
			TimeManager.OnTimeScaleUpdated?.Invoke(num2);
		}
	}

	public static void Reset()
	{
		Time.timeScale = 1f;
		_timeScale = 1f;
		_debugTimeScale = 1f;
		_cameraShakeTimeScale = 1f;
		_cheatMenuTimeScale = 0f;
		TimeManager.OnTimeScaleUpdated?.Invoke(1f);
	}

	public static void SpeedUpDebug()
	{
		DebugTimeScale *= 2f;
	}

	public static void SlowDownDebug()
	{
		DebugTimeScale *= 0.5f;
	}

	public static void ResetDebug()
	{
		DebugTimeScale = 1f;
	}

	public static void ResetCheatMenuTimeScale()
	{
		_cheatMenuTimeScale = 0f;
		UpdateTimeScale();
	}

	public static TimeControlInstance CreateTimeControl(float timeScale, TimeControlInstance.Type controlType = TimeControlInstance.Type.Multiplicative)
	{
		return new TimeControlInstance(timeScale, controlType);
	}
}

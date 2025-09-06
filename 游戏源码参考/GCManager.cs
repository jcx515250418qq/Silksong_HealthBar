using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Profiling;
using UnityEngine.Scripting;

public class GCManager : MonoBehaviour
{
	private bool pauseGCAttempts;

	private static double _lastGCTime;

	private const int CachedAutoGCTimes = 5;

	private const double StutterThresholdTime = 120.0;

	private static List<double> _lastAutoGCTimes = new List<double>();

	public static bool DisabledManualCollect { get; set; }

	public static double HeapUsageThreshold { get; private set; }

	private static event Action OnGCStutter;

	[RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.AfterSceneLoad)]
	private static void Init()
	{
		double num = 0.12102111566341002 * (double)SystemInfo.systemMemorySize;
		if (num < 384.0)
		{
			num = 384.0;
		}
		else if (num > 1024.0)
		{
			num = 1024.0;
		}
		HeapUsageThreshold = num;
		if (!Application.isEditor && !GarbageCollector.isIncremental)
		{
			GameObject obj = new GameObject("GCManager", typeof(GCManager));
			obj.hideFlags |= HideFlags.HideAndDontSave;
			UnityEngine.Object.DontDestroyOnLoad(obj);
		}
	}

	private void Awake()
	{
		GarbageCollector.GCMode = GarbageCollector.Mode.Disabled;
	}

	private void OnDestroy()
	{
		GarbageCollector.GCMode = GarbageCollector.Mode.Enabled;
	}

	private void Update()
	{
		if (GarbageCollector.GCMode == GarbageCollector.Mode.Enabled || Time.realtimeSinceStartupAsDouble - _lastGCTime < 1.0)
		{
			return;
		}
		double num = (double)GetMonoHeapUsage() / 1024.0 / 1024.0;
		if (num > HeapUsageThreshold)
		{
			if (!pauseGCAttempts)
			{
				ForceCollect();
				HandleGCStutter();
				if (num > HeapUsageThreshold)
				{
					pauseGCAttempts = true;
				}
			}
		}
		else
		{
			pauseGCAttempts = false;
		}
	}

	public static void Collect()
	{
		if (!DisabledManualCollect)
		{
			ForceCollect();
		}
	}

	public static void ForceCollect(bool blocking = true, bool compacting = false)
	{
		GarbageCollector.Mode gCMode = GarbageCollector.GCMode;
		GarbageCollector.GCMode = GarbageCollector.Mode.Enabled;
		GC.Collect(GC.MaxGeneration, GCCollectionMode.Forced, blocking, compacting);
		GarbageCollector.GCMode = gCMode;
		_lastGCTime = Time.realtimeSinceStartupAsDouble;
	}

	public static long GetMemoryUsage()
	{
		return GetMemoryTotal() - Profiler.GetTotalUnusedReservedMemoryLong();
	}

	public static long GetMemoryTotal()
	{
		return Profiler.GetTotalReservedMemoryLong();
	}

	public static long GetMonoHeapUsage()
	{
		return Profiler.GetMonoUsedSizeLong();
	}

	public static long GetMonoHeapTotal()
	{
		return Profiler.GetMonoHeapSizeLong();
	}

	private static void HandleGCStutter()
	{
		_lastAutoGCTimes.Add(Time.realtimeSinceStartupAsDouble);
		while (_lastAutoGCTimes.Count > 5)
		{
			_lastAutoGCTimes.RemoveAt(0);
		}
		if (IsGCStutterDetected())
		{
			float num = 0.1f;
			HeapUsageThreshold *= 1f + num;
			Debug.LogError($"GC stutter was detected! Heap size was increased by {(double)num * 100.0}%. Current heap threshold is: {HeapUsageThreshold}MB");
			GCManager.OnGCStutter?.Invoke();
			_lastAutoGCTimes.Clear();
		}
	}

	private static bool IsGCStutterDetected()
	{
		if (_lastAutoGCTimes.Count < 5)
		{
			return false;
		}
		double num = 0.0;
		for (int i = 0; i < _lastAutoGCTimes.Count - 1; i++)
		{
			num += Math.Abs(_lastAutoGCTimes[i] - _lastAutoGCTimes[i + 1]);
		}
		return num / (double)(_lastAutoGCTimes.Count - 1) < 120.0;
	}

	static GCManager()
	{
		GCManager.OnGCStutter = null;
	}
}

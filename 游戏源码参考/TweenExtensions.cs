using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public static class TweenExtensions
{
	private static readonly Dictionary<Transform, Coroutine> _scaleRoutines = new Dictionary<Transform, Coroutine>();

	public static void ScaleTo(this Transform transform, MonoBehaviour runner, Vector3 localScale, float duration, float delay = 0f, bool dontTrack = false, bool isRealtime = false, Action onEnd = null)
	{
		if (!runner)
		{
			Debug.LogError("No runner provided for ScaleTo");
			return;
		}
		if (!dontTrack && _scaleRoutines.TryGetValue(transform, out var value))
		{
			runner.StopCoroutine(value);
			_scaleRoutines.Remove(transform);
		}
		if (!transform)
		{
			Debug.LogError("No transform provided for ScaleTo");
			return;
		}
		if (duration <= 0f || !transform.gameObject.activeInHierarchy)
		{
			transform.localScale = localScale;
			return;
		}
		Vector3 initialScale = transform.localScale;
		Coroutine value2 = runner.StartTimerRoutine(delay, duration, Handler, null, delegate
		{
			onEnd?.Invoke();
			if (!dontTrack)
			{
				_scaleRoutines.Remove(transform);
			}
		}, isRealtime);
		if (!dontTrack)
		{
			_scaleRoutines[transform] = value2;
		}
		void Handler(float time)
		{
			transform.localScale = Vector3.Lerp(initialScale, localScale, time);
		}
	}

	public static void ClenaupInactiveCoroutines()
	{
		if (_scaleRoutines.Count != 0)
		{
			Transform[] array = _scaleRoutines.Keys.Where((Transform transform) => transform == null).ToArray();
			if (array.Length != 0)
			{
				Debug.LogWarning($"Inactive coroutines were cleared: {array.Length}");
			}
			Transform[] array2 = array;
			foreach (Transform key in array2)
			{
				_scaleRoutines.Remove(key);
			}
		}
	}
}

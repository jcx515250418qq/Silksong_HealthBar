using System.Collections;
using UnityEngine;

public static class TimeUtility
{
	public static IEnumerator Wait(float duration)
	{
		for (float timer = 0f; timer < duration; timer += Time.deltaTime)
		{
			yield return null;
		}
	}

	public static IEnumerator WaitForRealSeconds(float time)
	{
		float start = Time.realtimeSinceStartup;
		while (Time.realtimeSinceStartup < start + time)
		{
			yield return null;
		}
	}
}

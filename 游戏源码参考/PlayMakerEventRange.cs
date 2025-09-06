using System.Collections.Generic;
using UnityEngine;

public class PlayMakerEventRange : MonoBehaviour
{
	private static List<PlayMakerEventRange> allRanges = new List<PlayMakerEventRange>();

	[SerializeField]
	private float sendRange;

	[SerializeField]
	private PlayMakerFSM targetFSM;

	private List<string> handledEvents = new List<string>();

	private void OnDrawGizmosSelected()
	{
		if (!(sendRange <= 0f))
		{
			Gizmos.color = Color.yellow;
			Gizmos.DrawWireSphere(base.transform.position, sendRange);
		}
	}

	private void OnEnable()
	{
		allRanges.Add(this);
	}

	private void OnDisable()
	{
		allRanges.Remove(this);
	}

	public void SendEvent(string eventName, bool excludeThis)
	{
		if (string.IsNullOrEmpty(eventName))
		{
			return;
		}
		SendEventRecursive(eventName, excludeThis);
		foreach (PlayMakerEventRange allRange in allRanges)
		{
			allRange.handledEvents.Clear();
		}
	}

	private void SendEventRecursive(string eventName, bool excludeThis)
	{
		if (sendRange <= 0f || handledEvents.Contains(eventName))
		{
			return;
		}
		handledEvents.Add(eventName);
		if (!excludeThis && (bool)targetFSM)
		{
			targetFSM.SendEvent(eventName);
		}
		foreach (PlayMakerEventRange allRange in allRanges)
		{
			if (!(allRange == this) && Vector2.Distance(base.transform.position, allRange.transform.position) <= sendRange)
			{
				allRange.SendEventRecursive(eventName, excludeThis: false);
			}
		}
	}
}

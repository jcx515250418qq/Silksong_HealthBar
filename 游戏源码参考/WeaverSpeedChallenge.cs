using System;
using UnityEngine;

public class WeaverSpeedChallenge : MonoBehaviour
{
	[Serializable]
	private class Target
	{
		public int Threshold;

		public string Event;
	}

	[SerializeField]
	private Transform panelsParent;

	[SerializeField]
	private PlayMakerFSM completionFsm;

	[SerializeField]
	private Target[] targets;

	private WeaverSpeedPanel[] panels;

	private int[] maxTracker;

	private void Awake()
	{
		panels = panelsParent.GetComponentsInChildren<WeaverSpeedPanel>();
		maxTracker = new int[panels.Length];
		for (int i = 0; i < panels.Length; i++)
		{
			int index = i;
			panels[i].RecordedSpeedThreshold += delegate(int threshold)
			{
				WeaverSpeedPanel[] array = panels;
				for (int j = 0; j < array.Length; j++)
				{
					if (array[j].ForceStayLit)
					{
						return;
					}
				}
				maxTracker[index] = threshold;
				if (index >= panels.Length - 1)
				{
					CheckCompletion();
				}
			};
		}
	}

	private void CheckCompletion()
	{
		bool flag = false;
		int num = int.MaxValue;
		int[] array = maxTracker;
		foreach (int num2 in array)
		{
			if (!flag || num2 <= num)
			{
				flag = true;
				num = num2;
			}
		}
		Target target = null;
		Target[] array2 = targets;
		foreach (Target target2 in array2)
		{
			if (target2.Threshold <= num && target2.Threshold > (target?.Threshold ?? 0))
			{
				target = target2;
			}
		}
		if (target != null)
		{
			completionFsm.SendEvent(target.Event);
		}
	}

	public void CapturePanels()
	{
		WeaverSpeedPanel[] array = panels;
		for (int i = 0; i < array.Length; i++)
		{
			array[i].ForceStayLit = true;
		}
	}

	public void ReleasePanels()
	{
		WeaverSpeedPanel[] array = panels;
		for (int i = 0; i < array.Length; i++)
		{
			array[i].ForceStayLit = false;
		}
	}
}

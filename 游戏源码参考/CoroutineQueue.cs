using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CoroutineQueue
{
	private readonly List<IEnumerator> pendingCoroutines;

	private readonly MonoBehaviour runner;

	private bool isRunning;

	public CoroutineQueue(MonoBehaviour runner)
	{
		this.runner = runner;
		pendingCoroutines = new List<IEnumerator>();
	}

	public void Enqueue(IEnumerator coroutine)
	{
		if ((bool)runner && runner.isActiveAndEnabled)
		{
			pendingCoroutines.Add(coroutine);
			if (!isRunning)
			{
				runner.StartCoroutine(Run());
			}
		}
	}

	public void Clear()
	{
		pendingCoroutines.Clear();
	}

	public IEnumerator Run()
	{
		isRunning = true;
		while (pendingCoroutines.Count > 0)
		{
			IEnumerator coroutine = pendingCoroutines[0];
			pendingCoroutines.RemoveAt(0);
			while (true)
			{
				bool flag;
				try
				{
					flag = coroutine.MoveNext();
				}
				catch (Exception exception)
				{
					Debug.LogException(exception);
					break;
				}
				if (!flag)
				{
					break;
				}
				yield return coroutine.Current;
			}
		}
		isRunning = false;
	}
}

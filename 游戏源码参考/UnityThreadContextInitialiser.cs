using System;
using System.Threading;
using UnityEngine;

public sealed class UnityThreadContextInitialiser : MonoBehaviour
{
	private bool init;

	private int threadId;

	private Action<int> initialisationCallback;

	private void Awake()
	{
		threadId = Thread.CurrentThread.ManagedThreadId;
		initialisationCallback?.Invoke(threadId);
		initialisationCallback = null;
		init = true;
	}

	public void SetCallback(Action<int> callback)
	{
		if (!init)
		{
			initialisationCallback = (Action<int>)Delegate.Combine(initialisationCallback, callback);
		}
		else
		{
			callback?.Invoke(threadId);
		}
	}
}

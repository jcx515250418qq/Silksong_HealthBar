using System;
using System.Collections.Generic;
using UnityEngine;

public class CoreLoop : MonoBehaviour
{
	private class DelayedInvoke
	{
		public float TimeRemaining;

		public Action Action;
	}

	private static CoreLoop instance;

	private static List<Action> invokeNextActions = new List<Action>();

	private static List<Action> invokeNextActionsBuffer = new List<Action>();

	private static bool isFiringInvokeNext = false;

	private static List<DelayedInvoke> delayedInvokes = new List<DelayedInvoke>();

	private static object invokeOnGameThreadMutex = new object();

	private static List<Action> pendingActions = new List<Action>();

	private static List<Action> executingActions = new List<Action>();

	[RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
	private static void Init()
	{
		GameObject obj = new GameObject("CoreLoop");
		instance = obj.AddComponent<CoreLoop>();
		UnityEngine.Object.DontDestroyOnLoad(obj);
	}

	public static void InvokeNext(Action action)
	{
		invokeNextActions.Add(action);
		EnqueueInvokeNext();
	}

	public static void InvokeSafe(Action action)
	{
		if (action != null)
		{
			InvokeOnGameThread(action);
		}
	}

	private static void EnqueueInvokeNext()
	{
		if (!isFiringInvokeNext)
		{
			isFiringInvokeNext = true;
			instance.Invoke("FireInvokeNext", 0f);
		}
	}

	protected void FireInvokeNext()
	{
		isFiringInvokeNext = false;
		List<Action> list = invokeNextActions;
		List<Action> list2 = invokeNextActionsBuffer;
		invokeNextActionsBuffer = list;
		invokeNextActions = list2;
		for (int i = 0; i < invokeNextActionsBuffer.Count; i++)
		{
			Action action = invokeNextActionsBuffer[i];
			if (action != null)
			{
				try
				{
					action();
				}
				catch (Exception exception)
				{
					Debug.LogException(exception);
				}
			}
		}
		invokeNextActionsBuffer.Clear();
	}

	public static void InvokeOnGameThread(Action action)
	{
		lock (invokeOnGameThreadMutex)
		{
			pendingActions.Add(action);
		}
	}

	protected void Update()
	{
		lock (invokeOnGameThreadMutex)
		{
			List<Action> list = pendingActions;
			List<Action> list2 = executingActions;
			executingActions = list;
			pendingActions = list2;
		}
		if (executingActions.Count > 0)
		{
			for (int i = 0; i < executingActions.Count; i++)
			{
				InvokeNext(executingActions[i]);
			}
			executingActions.Clear();
		}
		for (int j = 0; j < delayedInvokes.Count; j++)
		{
			DelayedInvoke delayedInvoke = delayedInvokes[j];
			delayedInvoke.TimeRemaining -= Time.unscaledDeltaTime;
			if (delayedInvoke.TimeRemaining <= 0f)
			{
				delayedInvokes.RemoveAt(j--);
				InvokeNext(delayedInvoke.Action);
			}
		}
	}
}

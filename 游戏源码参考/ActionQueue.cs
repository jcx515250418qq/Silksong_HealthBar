using System;
using System.Collections.Generic;
using UnityEngine;

public class ActionQueue
{
	public delegate void ActionQueueCallback(Action next);

	private readonly List<ActionQueueCallback> pendingActions;

	private bool isRunning;

	public ActionQueue()
	{
		pendingActions = new List<ActionQueueCallback>();
		isRunning = false;
	}

	public void Next()
	{
		while (pendingActions.Count > 0)
		{
			ActionQueueCallback actionQueueCallback = pendingActions[0];
			pendingActions.RemoveAt(0);
			try
			{
				actionQueueCallback(Next);
				return;
			}
			catch (Exception exception)
			{
				Debug.LogException(exception);
			}
		}
		isRunning = false;
	}

	public void Enqueue(ActionQueueCallback action)
	{
		if (action != null)
		{
			pendingActions.Add(action);
			if (!isRunning)
			{
				isRunning = true;
				Next();
			}
		}
	}
}

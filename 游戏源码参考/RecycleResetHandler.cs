using System;
using System.Collections.Generic;
using UnityEngine;

[DisallowMultipleComponent]
public class RecycleResetHandler : MonoBehaviour
{
	private List<Action> resetActions;

	private List<Action<GameObject>> resetSelfActions;

	public void OnPreRecycle()
	{
		if (resetActions != null)
		{
			foreach (Action resetAction in resetActions)
			{
				resetAction();
			}
			resetActions.Clear();
		}
		if (resetSelfActions == null)
		{
			return;
		}
		foreach (Action<GameObject> resetSelfAction in resetSelfActions)
		{
			resetSelfAction(base.gameObject);
		}
		resetSelfActions.Clear();
	}

	private void OnDisable()
	{
		OnPreRecycle();
	}

	public static void Add(GameObject target, Action resetAction)
	{
		if (resetAction != null)
		{
			target.AddComponentIfNotPresent<RecycleResetHandler>().AddTempAction(resetAction);
		}
	}

	public void AddTempAction(Action resetAction)
	{
		if (resetAction != null)
		{
			if (resetActions == null)
			{
				resetActions = new List<Action>();
			}
			resetActions.Add(resetAction);
		}
	}

	public static void Add(GameObject target, Action<GameObject> resetAction)
	{
		if (resetAction != null)
		{
			target.AddComponentIfNotPresent<RecycleResetHandler>().AddTempAction(resetAction);
		}
	}

	public void AddTempAction(Action<GameObject> resetAction)
	{
		if (resetAction != null)
		{
			if (resetSelfActions == null)
			{
				resetSelfActions = new List<Action<GameObject>>();
			}
			resetSelfActions.Add(resetAction);
		}
	}
}

using System;
using System.Collections.Generic;
using UnityEngine.ResourceManagement.AsyncOperations;

public static class AsyncLoadOrderingManager
{
	private static List<(int, AsyncOperationHandle)> _orderedLoads;

	private static List<(int, AsyncOperationHandle)> _tempList;

	private static int _lastLoadHandle;

	private static Queue<Action> _onLoadsCompleteActionQueue;

	public static void OnStartedLoad(AsyncOperationHandle loadQueueItem, out int loadHandle)
	{
		_lastLoadHandle++;
		loadHandle = _lastLoadHandle;
		if (_orderedLoads == null)
		{
			_orderedLoads = new List<(int, AsyncOperationHandle)>();
		}
		_orderedLoads.Add((loadHandle, loadQueueItem));
	}

	public static void CompleteUpTo(AsyncOperationHandle loadQueueItem, int loadHandle)
	{
		if (_orderedLoads == null)
		{
			return;
		}
		if (_tempList == null)
		{
			_tempList = new List<(int, AsyncOperationHandle)>(_orderedLoads.Count);
		}
		foreach (var (num, item) in _orderedLoads)
		{
			if (num == loadHandle)
			{
				break;
			}
			_tempList.Add((num, item));
		}
		foreach (var temp in _tempList)
		{
			temp.Item2.WaitForCompletion();
		}
		_tempList.Clear();
	}

	public static void OnCompletedLoad(AsyncOperationHandle loadQueueItem, int loadHandle)
	{
		if (_orderedLoads == null)
		{
			return;
		}
		for (int num = _orderedLoads.Count - 1; num >= 0; num--)
		{
			if (_orderedLoads[num].Item1 == loadHandle)
			{
				_orderedLoads.RemoveAt(num);
			}
		}
		if (_orderedLoads.Count != 0)
		{
			return;
		}
		_orderedLoads = null;
		if (_onLoadsCompleteActionQueue != null)
		{
			Action result;
			while (_onLoadsCompleteActionQueue.TryDequeue(out result))
			{
				result();
			}
			_onLoadsCompleteActionQueue = null;
		}
	}

	public static void DoActionAfterAllLoadsComplete(Action action)
	{
		List<(int, AsyncOperationHandle)> orderedLoads = _orderedLoads;
		if (orderedLoads == null || orderedLoads.Count <= 0)
		{
			action();
			return;
		}
		if (_onLoadsCompleteActionQueue == null)
		{
			_onLoadsCompleteActionQueue = new Queue<Action>();
		}
		_onLoadsCompleteActionQueue.Enqueue(action);
	}
}

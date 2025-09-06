using System;
using System.Collections.Generic;
using UnityEngine;

public class UpdateBatcher : MonoBehaviour
{
	private sealed class UpdateList<T>
	{
		private readonly HashSet<T> hashset = new HashSet<T>();

		private readonly List<T> list = new List<T>();

		private bool dirty;

		public List<T> List
		{
			get
			{
				Update();
				return list;
			}
		}

		private void Update()
		{
			if (dirty)
			{
				dirty = false;
				List.Clear();
				List.AddRange(hashset);
			}
		}

		public void Add(MonoBehaviour monoBehaviour)
		{
			if (monoBehaviour is T item && hashset.Add(item))
			{
				dirty = true;
			}
		}

		public bool Remove(MonoBehaviour monoBehaviour)
		{
			if (!(monoBehaviour is T item))
			{
				return false;
			}
			if (!hashset.Remove(item))
			{
				return false;
			}
			dirty = true;
			return true;
		}
	}

	private readonly UpdateList<IUpdateBatchableLateUpdate> lateUpdates = new UpdateList<IUpdateBatchableLateUpdate>();

	private readonly UpdateList<IUpdateBatchableUpdate> updates = new UpdateList<IUpdateBatchableUpdate>();

	private readonly UpdateList<IUpdateBatchableFixedUpdate> fixedUpdates = new UpdateList<IUpdateBatchableFixedUpdate>();

	private void Update()
	{
		List<IUpdateBatchableUpdate> list = updates.List;
		for (int num = list.Count - 1; num >= 0; num--)
		{
			IUpdateBatchableUpdate updateBatchableUpdate = list[num];
			try
			{
				if (updateBatchableUpdate.ShouldUpdate)
				{
					updateBatchableUpdate.BatchedUpdate();
				}
			}
			catch (Exception)
			{
			}
		}
	}

	private void LateUpdate()
	{
		List<IUpdateBatchableLateUpdate> list = lateUpdates.List;
		for (int num = list.Count - 1; num >= 0; num--)
		{
			IUpdateBatchableLateUpdate updateBatchableLateUpdate = list[num];
			try
			{
				if (updateBatchableLateUpdate.ShouldUpdate)
				{
					updateBatchableLateUpdate.BatchedLateUpdate();
				}
			}
			catch (Exception)
			{
			}
		}
	}

	private void FixedUpdate()
	{
		List<IUpdateBatchableFixedUpdate> list = fixedUpdates.List;
		for (int num = list.Count - 1; num >= 0; num--)
		{
			IUpdateBatchableFixedUpdate updateBatchableFixedUpdate = list[num];
			try
			{
				if (updateBatchableFixedUpdate.ShouldUpdate)
				{
					updateBatchableFixedUpdate.BatchedFixedUpdate();
				}
			}
			catch (Exception)
			{
			}
		}
	}

	public void Add(MonoBehaviour behaviour)
	{
		lateUpdates.Add(behaviour);
		updates.Add(behaviour);
		fixedUpdates.Add(behaviour);
	}

	public bool Remove(MonoBehaviour behaviour)
	{
		bool result = lateUpdates.Remove(behaviour);
		if (updates.Remove(behaviour))
		{
			result = true;
		}
		if (fixedUpdates.Remove(behaviour))
		{
			result = true;
		}
		return result;
	}
}

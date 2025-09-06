using System.Collections.Generic;

public sealed class UniqueList<T>
{
	private readonly HashSet<T> hashSet = new HashSet<T>();

	private readonly List<T> list = new List<T>();

	private bool dirty;

	private bool pendingClear;

	private int reservationCount;

	public List<T> List
	{
		get
		{
			UpdateList();
			return list;
		}
	}

	public int Count => hashSet.Count;

	public void ReserveListUsage()
	{
		reservationCount++;
	}

	public void ReleaseListUsage()
	{
		reservationCount--;
		if (reservationCount <= 0)
		{
			reservationCount = 0;
			if (pendingClear)
			{
				list.Clear();
				dirty = false;
				pendingClear = false;
			}
		}
	}

	public void UpdateList()
	{
		if (dirty)
		{
			dirty = false;
			list.Clear();
			list.AddRange(hashSet);
		}
	}

	public bool Add(T element)
	{
		if (hashSet.Add(element))
		{
			dirty = true;
			return true;
		}
		return false;
	}

	public bool Remove(T element)
	{
		if (hashSet.Remove(element))
		{
			dirty = true;
			if (hashSet.Count == 0)
			{
				if (reservationCount == 0)
				{
					list.Clear();
					dirty = false;
				}
				else
				{
					pendingClear = true;
				}
			}
			return true;
		}
		return false;
	}

	public void Clear()
	{
		hashSet.Clear();
		if (reservationCount == 0)
		{
			list.Clear();
			dirty = false;
		}
		else
		{
			dirty = true;
			pendingClear = true;
		}
	}

	public void FullClear()
	{
		hashSet.Clear();
		list.Clear();
		dirty = false;
		pendingClear = false;
		reservationCount = 0;
	}
}

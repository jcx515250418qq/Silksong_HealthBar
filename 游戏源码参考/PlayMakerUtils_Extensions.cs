using System;
using System.Collections;
using UnityEngine;

public static class PlayMakerUtils_Extensions
{
	public static int IndexOf(ArrayList target, object value)
	{
		return IndexOf(target, value, 0, target.Count);
	}

	public static int IndexOf(ArrayList target, object value, int startIndex)
	{
		if (startIndex > target.Count)
		{
			throw new ArgumentOutOfRangeException("startIndex", "ArgumentOutOfRange_Index");
		}
		return IndexOf(target, value, startIndex, target.Count - startIndex);
	}

	public static int IndexOf(ArrayList target, object value, int startIndex, int count)
	{
		Debug.Log(startIndex + " " + count);
		if (startIndex < 0 || startIndex >= target.Count)
		{
			throw new ArgumentOutOfRangeException("startIndex", "ArgumentOutOfRange_Index");
		}
		if (count < 0 || startIndex > target.Count - count)
		{
			throw new ArgumentOutOfRangeException("count", "ArgumentOutOfRange_Count");
		}
		if (target.Count == 0)
		{
			return -1;
		}
		int num = startIndex + count;
		if (value == null)
		{
			for (int i = startIndex; i < num; i++)
			{
				if (target[i] == null)
				{
					return i;
				}
			}
			return -1;
		}
		for (int j = startIndex; j < num; j++)
		{
			if (target[j] != null && target[j].Equals(value))
			{
				return j;
			}
		}
		return -1;
	}

	public static int LastIndexOf(ArrayList target, object value)
	{
		return LastIndexOf(target, value, target.Count - 1, target.Count);
	}

	public static int LastIndexOf(ArrayList target, object value, int startIndex)
	{
		return LastIndexOf(target, value, startIndex, startIndex + 1);
	}

	public static int LastIndexOf(ArrayList target, object value, int startIndex, int count)
	{
		if (target.Count == 0)
		{
			return -1;
		}
		if (startIndex < 0 || startIndex >= target.Count)
		{
			throw new ArgumentOutOfRangeException("startIndex", "ArgumentOutOfRange_Index");
		}
		if (count < 0 || startIndex > target.Count - count)
		{
			throw new ArgumentOutOfRangeException("count", "ArgumentOutOfRange_Count");
		}
		int num = startIndex + count - 1;
		if (value == null)
		{
			for (int num2 = num; num2 >= startIndex; num2--)
			{
				if (target[num2] == null)
				{
					return num2;
				}
			}
			return -1;
		}
		for (int num3 = num; num3 >= startIndex; num3--)
		{
			if (target[num3] != null && target[num3].Equals(value))
			{
				return num3;
			}
		}
		return -1;
	}
}

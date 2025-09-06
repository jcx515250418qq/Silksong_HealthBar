using System;
using System.Collections.Generic;
using UnityEngine;

public static class GCArtificialMemoryPressure
{
	public static List<byte[]> allocated1KBChunks = new List<byte[]>();

	public static long AllocatedBytes => allocated1KBChunks.Count * 1024;

	public static long FreeBytesCount
	{
		get
		{
			long num = (long)GCManager.HeapUsageThreshold * 1024 * 1024;
			long monoHeapUsage = GCManager.GetMonoHeapUsage();
			return num - monoHeapUsage;
		}
	}

	public static long MaxUsageThreshold => (long)GCManager.HeapUsageThreshold * 1024 * 1024;

	public static void ClearAllocatedMemory()
	{
		allocated1KBChunks.Clear();
	}

	public static void IncreaseGCPressure(float t)
	{
		if (t < 0f || t > 1f)
		{
			throw new InvalidOperationException("Invalid argument");
		}
		long bytesToThreshold = GetBytesToThreshold();
		if (bytesToThreshold < 0)
		{
			Debug.LogWarning("Memory usage exceeds the threshold.");
		}
		else
		{
			Allocate((long)((double)bytesToThreshold * (double)t));
		}
	}

	public static void DecreaseGCPressure(float t)
	{
		if (t < 0f || t > 1f)
		{
			throw new InvalidOperationException("Invalid argument");
		}
		Free((long)((double)AllocatedBytes * (double)t));
	}

	public static long GetBytesToThreshold()
	{
		long num = (long)GCManager.HeapUsageThreshold * 1024 * 1024;
		long monoHeapUsage = GCManager.GetMonoHeapUsage();
		return num - monoHeapUsage;
	}

	public static void Free(long bytesCount)
	{
		bytesCount -= bytesCount % 1024;
		long num = bytesCount / 1024;
		if (num > allocated1KBChunks.Count)
		{
			allocated1KBChunks.Clear();
		}
		else
		{
			allocated1KBChunks.RemoveRange(0, (int)num);
		}
	}

	public static void Allocate(long bytesCount)
	{
		bytesCount -= bytesCount % 1024;
		long num = bytesCount / 1024;
		for (int i = 0; i < num; i++)
		{
			Allocate1KB();
		}
	}

	public static void Allocate1KB()
	{
		byte[] item = new byte[1008];
		allocated1KBChunks.Add(item);
	}
}

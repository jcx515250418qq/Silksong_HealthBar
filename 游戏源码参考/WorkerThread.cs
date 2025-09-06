using System;
using System.Collections.Concurrent;
using System.Threading;
using UnityEngine;

public sealed class WorkerThread : IDisposable
{
	private readonly ConcurrentQueue<Action> workQueue;

	private readonly ManualResetEvent workReadyHandle;

	private readonly Thread workerThread;

	private bool isDisposed;

	private bool cleanUpRequested;

	public System.Threading.ThreadPriority ThreadPriority
	{
		get
		{
			return workerThread.Priority;
		}
		set
		{
			workerThread.Priority = value;
		}
	}

	public WorkerThread(System.Threading.ThreadPriority threadPriority = System.Threading.ThreadPriority.Normal)
	{
		workQueue = new ConcurrentQueue<Action>();
		workReadyHandle = new ManualResetEvent(initialState: false);
		workerThread = new Thread(RunWork)
		{
			IsBackground = true,
			Priority = threadPriority
		};
		workerThread.Start();
	}

	~WorkerThread()
	{
		Dispose(disposing: false);
	}

	public void Dispose()
	{
		Dispose(disposing: true);
		GC.SuppressFinalize(this);
	}

	private void Dispose(bool disposing)
	{
		if (!isDisposed)
		{
			isDisposed = true;
			RequestShutdown();
		}
	}

	private void RequestShutdown()
	{
		cleanUpRequested = true;
		workReadyHandle.Set();
	}

	private void CleanupResources()
	{
		workReadyHandle.Close();
	}

	public void EnqueueWork(Action work)
	{
		if (work != null && !cleanUpRequested)
		{
			workQueue.Enqueue(work);
			workReadyHandle.Set();
		}
	}

	private void RunWork()
	{
		Action result = null;
		while (true)
		{
			if (!workQueue.TryDequeue(out result))
			{
				if (cleanUpRequested)
				{
					break;
				}
				workReadyHandle.Reset();
			}
			if (result != null)
			{
				try
				{
					result();
				}
				catch (Exception exception)
				{
					Debug.LogException(exception);
				}
			}
			else
			{
				workReadyHandle.WaitOne();
			}
		}
		CleanupResources();
	}
}

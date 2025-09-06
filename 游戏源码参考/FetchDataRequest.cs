using System;
using System.Collections.Generic;
using UnityEngine;

public sealed class FetchDataRequest
{
	public enum Status
	{
		NotStarted = 0,
		InProgress = 1,
		Completed = 2,
		Failed = 3
	}

	private Status status;

	private readonly object statusLock;

	private readonly object listLock;

	private Action<FetchDataRequest> fetchCallbacks;

	private Action<FetchDataRequest> callbacks;

	private List<RestorePointFileWrapper> restorePoints;

	public Status State
	{
		get
		{
			lock (statusLock)
			{
				return status;
			}
		}
		set
		{
			bool flag = false;
			lock (statusLock)
			{
				flag = value == Status.Completed && status != Status.Completed;
				if (!flag)
				{
					status = value;
				}
			}
			if (flag)
			{
				OnFetchCompleted();
				lock (statusLock)
				{
					status = value;
				}
				OnFetchCompleted();
				RunCallbacks();
			}
		}
	}

	public string Name { get; set; }

	public List<RestorePointFileWrapper> RestorePoints
	{
		get
		{
			lock (listLock)
			{
				return restorePoints;
			}
		}
		set
		{
			lock (listLock)
			{
				restorePoints = value;
			}
		}
	}

	public static FetchDataRequest Error => new FetchDataRequest
	{
		status = Status.Failed
	};

	public FetchDataRequest()
	{
		statusLock = new object();
		listLock = new object();
	}

	public void AddResult(RestorePointFileWrapper data)
	{
		lock (listLock)
		{
			for (int i = 0; i < restorePoints.Count; i++)
			{
				RestorePointFileWrapper restorePointFileWrapper = restorePoints[i];
				if (data.number >= restorePointFileWrapper.number)
				{
					restorePoints.Insert(i, data);
					return;
				}
			}
			restorePoints.Add(data);
		}
	}

	public void RunOnFetchComplete(Action<FetchDataRequest> callback)
	{
		lock (statusLock)
		{
			if (status == Status.Completed || status == Status.Failed)
			{
				callback?.Invoke(this);
			}
			else
			{
				callbacks = (Action<FetchDataRequest>)Delegate.Combine(callbacks, callback);
			}
		}
	}

	public void RunOnComplete(Action<FetchDataRequest> callback)
	{
		lock (statusLock)
		{
			if (status == Status.Completed || status == Status.Failed)
			{
				callback?.Invoke(this);
			}
			else
			{
				callbacks = (Action<FetchDataRequest>)Delegate.Combine(callbacks, callback);
			}
		}
	}

	private void OnFetchCompleted()
	{
		if (fetchCallbacks != null)
		{
			Action<FetchDataRequest> action = fetchCallbacks;
			fetchCallbacks = null;
			action(this);
		}
	}

	private void RunCallbacks()
	{
		if (callbacks != null)
		{
			Action<FetchDataRequest> action = callbacks;
			callbacks = null;
			action(this);
		}
	}
}
public sealed class FetchDataRequest<T> where T : new()
{
	public sealed class FetchResult
	{
		public readonly RestorePointFileWrapper sourceData;

		public T loadedObject;

		public FetchResult(RestorePointFileWrapper sourceData)
		{
			this.sourceData = sourceData;
			try
			{
				string jsonForSaveBytesStatic = GameManager.GetJsonForSaveBytesStatic(sourceData.data);
				if (string.IsNullOrEmpty(jsonForSaveBytesStatic))
				{
					Debug.LogError("Failed to load json from bytes.");
					return;
				}
				loadedObject = SaveDataUtility.DeserializeSaveData<T>(jsonForSaveBytesStatic);
				if (loadedObject == null)
				{
					Debug.LogError("Failed to load " + typeof(T).Name + " from " + jsonForSaveBytesStatic + ".");
				}
			}
			catch (Exception message)
			{
				Debug.LogError(message);
			}
		}
	}

	public readonly FetchDataRequest dataSource;

	public readonly List<FetchResult> results;

	private bool isComplete;

	public FetchDataRequest.Status State
	{
		get
		{
			if (dataSource.State == FetchDataRequest.Status.Completed && !isComplete)
			{
				return FetchDataRequest.Status.InProgress;
			}
			return dataSource.State;
		}
	}

	public FetchDataRequest(FetchDataRequest dataSource)
	{
		FetchDataRequest<T> fetchDataRequest = this;
		this.dataSource = dataSource;
		if (dataSource != null)
		{
			results = new List<FetchResult>();
			dataSource.RunOnFetchComplete(delegate(FetchDataRequest fetchResult)
			{
				try
				{
					if (dataSource.RestorePoints != null)
					{
						foreach (RestorePointFileWrapper restorePoint in fetchResult.RestorePoints)
						{
							if (restorePoint == null)
							{
								Debug.LogError(dataSource.Name + " failed to load restore point");
							}
							else if (restorePoint.data == null)
							{
								Debug.LogError(dataSource.Name + " is missing data");
							}
							else
							{
								FetchResult fetchResult2 = new FetchResult(restorePoint);
								if (fetchResult2.loadedObject != null)
								{
									fetchDataRequest.results.Add(fetchResult2);
								}
							}
						}
						return;
					}
				}
				finally
				{
					fetchDataRequest.isComplete = true;
				}
			});
		}
		else
		{
			Debug.LogError("Data source is null");
		}
	}
}

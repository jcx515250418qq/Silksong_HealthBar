using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Newtonsoft.Json.UnityConverters;
using UnityEngine;

public static class SaveDataUtility
{
	public const int MANUAL_REVISION_BREAK = 28104;

	private static JsonSerializer _serializer;

	private static readonly WorkerThread _saveWorkerThread = new WorkerThread();

	private static readonly Queue<Action> _saveTaskQueue = new Queue<Action>();

	private static Task _runningSaveTask;

	public static System.Threading.ThreadPriority ThreadPriority
	{
		get
		{
			return _saveWorkerThread.ThreadPriority;
		}
		set
		{
			_saveWorkerThread.ThreadPriority = value;
		}
	}

	public static string CleanupVersionText(string versionText)
	{
		return Regex.Replace(versionText, "[A-Za-z ]", "");
	}

	public static bool IsVersionIncompatible(string currentGameVersion, string fileVersionText, int fileRevisionBreak, int manualRevisionBreak = 28104)
	{
		if (string.IsNullOrEmpty(fileVersionText))
		{
			Debug.LogWarning("Save slot has no version in file!");
			return true;
		}
		string text = CleanupVersionText(fileVersionText);
		if (string.IsNullOrEmpty(text))
		{
			Debug.LogWarning("Cleaning up save slot version resulted in empty version text!");
			return true;
		}
		Version version = new Version(text);
		Version version2 = new Version(CleanupVersionText(currentGameVersion));
		if (fileRevisionBreak > manualRevisionBreak)
		{
			return true;
		}
		if (version2.Major > version.Major)
		{
			return false;
		}
		if (version2.Major < version.Major)
		{
			return true;
		}
		if (version2.Minor < version.Minor)
		{
			return true;
		}
		return false;
	}

	private static void CreateJsonObjects()
	{
		if (_serializer == null)
		{
			_serializer = JsonSerializer.Create(UnityConverterInitializer.defaultUnityConvertersSettings);
			_serializer.DefaultValueHandling = DefaultValueHandling.Populate;
			_serializer.ReferenceLoopHandling = ReferenceLoopHandling.Ignore;
			_serializer.NullValueHandling = NullValueHandling.Ignore;
			_serializer.MissingMemberHandling = MissingMemberHandling.Ignore;
		}
	}

	public static string SerializeSaveData<T>(T saveData)
	{
		CreateJsonObjects();
		StringBuilder stringBuilder = new StringBuilder();
		using JsonTextWriter jsonWriter = new JsonTextWriter(new StringWriter(stringBuilder));
		_serializer.Serialize(jsonWriter, saveData);
		return stringBuilder.ToString();
	}

	public static void SerializeToJsonAsync<T>(T saveData, Action<bool, string> onComplete)
	{
		AddTaskToAsyncQueue(delegate(TaskCompletionSource<string> tcs)
		{
			string result = SerializeSaveData(saveData);
			tcs.SetResult(result);
		}, onComplete);
	}

	public static T DeserializeSaveData<T>(string json) where T : new()
	{
		CreateJsonObjects();
		JsonTextReader reader = new JsonTextReader(new StringReader(json));
		return _serializer.Deserialize<T>(reader);
	}

	public static void AddTaskToAsyncQueue(Action<TaskCompletionSource<string>> taskToRun, Action<bool, string> onComplete)
	{
		if (_runningSaveTask == null)
		{
			RunNewTask();
		}
		else
		{
			_saveTaskQueue.Enqueue(RunNewTask);
		}
		void RunNewTask()
		{
			TaskCompletionSource<string> tcs = new TaskCompletionSource<string>();
			tcs.Task.ConfigureAwait(continueOnCapturedContext: true).GetAwaiter().UnsafeOnCompleted(delegate
			{
				try
				{
					Task<string> task = tcs.Task;
					if (task.Exception != null)
					{
						onComplete(arg1: false, task.Exception.ToString());
						throw task.Exception;
					}
					if (task.IsCanceled || task.IsFaulted)
					{
						onComplete(arg1: false, null);
					}
					else
					{
						onComplete(arg1: true, tcs.Task.Result);
					}
				}
				catch (Exception exception)
				{
					Debug.LogException(exception);
				}
				finally
				{
					_runningSaveTask = null;
					if (_saveTaskQueue.Count > 0)
					{
						_saveTaskQueue.Dequeue()();
					}
				}
			});
			_runningSaveTask = Task.Run(delegate
			{
				try
				{
					taskToRun(tcs);
				}
				catch (Exception exception2)
				{
					tcs.SetException(exception2);
				}
			});
		}
	}

	public static void AddTaskToAsyncQueue(Action task)
	{
		if (task != null)
		{
			_saveWorkerThread.EnqueueWork(task);
		}
	}
}

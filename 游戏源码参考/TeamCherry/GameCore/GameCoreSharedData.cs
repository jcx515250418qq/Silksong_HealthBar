using System;
using System.Threading.Tasks;
using UnityEngine;

namespace TeamCherry.GameCore
{
	public sealed class GameCoreSharedData : CustomSharedData
	{
		private volatile bool hasLoaded;

		private bool dataRequested;

		private string container;

		private string fileName;

		private bool useEncryption;

		private Action onDataLoaded;

		private object loadLock = new object();

		public bool HasLoaded => hasLoaded;

		public GameCoreSharedData(string container, string fileName)
		{
			this.container = container;
			this.fileName = fileName;
			LoadData();
		}

		~GameCoreSharedData()
		{
		}

		public override void ImportData(Platform.ISharedData otherData)
		{
			lock (loadLock)
			{
				if (!hasLoaded)
				{
					onDataLoaded = (Action)Delegate.Combine(onDataLoaded, (Action)delegate
					{
						ImportData(otherData);
					});
					return;
				}
			}
			base.ImportData(otherData);
		}

		public void LoadData(Action callback = null)
		{
			lock (loadLock)
			{
				if (dataRequested)
				{
					onDataLoaded = (Action)Delegate.Combine(onDataLoaded, callback);
					return;
				}
				dataRequested = true;
				hasLoaded = false;
			}
			if (string.IsNullOrEmpty(container))
			{
				Debug.LogError("Unable to load shared data. Missing container name.");
				return;
			}
			if (string.IsNullOrEmpty(fileName))
			{
				Debug.LogError("Unable to load shared data. Missing file name.");
				return;
			}
			GameCoreRuntimeManager.LoadSaveData(container, fileName, delegate(byte[] byteData)
			{
				if (byteData == null)
				{
					Debug.LogError("Failed to load data " + container + " " + fileName);
					CoreLoop.InvokeSafe(delegate
					{
						base.SharedData.Clear();
						Action action = null;
						lock (loadLock)
						{
							hasLoaded = true;
							dataRequested = false;
							action = onDataLoaded;
							onDataLoaded = null;
						}
						action?.Invoke();
					});
				}
				else
				{
					CoreLoop.InvokeSafe(delegate
					{
						base.SharedData.Clear();
						LoadFromJSON(CustomSharedData.BytesToJson(byteData, useEncryption));
						Action action2 = null;
						lock (loadLock)
						{
							hasLoaded = true;
							dataRequested = false;
							action2 = onDataLoaded;
							onDataLoaded = null;
						}
						action2?.Invoke();
					});
				}
			});
		}

		public override void Save()
		{
			if (string.IsNullOrEmpty(container))
			{
				Debug.LogError("Unable to save shared data. Missing container name.");
			}
			else if (string.IsNullOrEmpty(fileName))
			{
				Debug.LogError("Unable to save shared data. Missing file name.");
			}
			else
			{
				if (!GameCoreRuntimeManager.SaveSystemInitialised)
				{
					return;
				}
				SaveDataUtility.AddTaskToAsyncQueue(delegate(TaskCompletionSource<string> tcs)
				{
					byte[] data = CustomSharedData.JsonToBytes(SaveToJSON(), useEncryption);
					GameCoreRuntimeManager.Save(container, fileName, data, null);
					tcs.SetResult(null);
				}, delegate(bool success, string result)
				{
					if (!success)
					{
						Debug.LogError("Error writing JsonSharedData in thread: " + result);
					}
				});
			}
		}
	}
}

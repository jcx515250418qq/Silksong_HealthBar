using System;
using System.Collections.Generic;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using UnityEngine;

namespace TeamCherry.GameCore
{
	public sealed class GameCoreSaveRestoreHandler : SaveRestoreHandler
	{
		private new sealed class RestorePointList : RestorePointList<GameCoreInfo, GameCoreSaveRestoreHandler>
		{
			public override void PrepareDelete()
			{
			}

			public override void Commit()
			{
			}
		}

		private sealed class GameCoreInfo : Info<GameCoreSaveRestoreHandler>
		{
			public string directory;

			public override void Delete()
			{
				GameCoreRuntimeManager.DeleteBlob(directory, file, null);
			}

			public override void Commit()
			{
			}

			public override void LoadData(GameCoreSaveRestoreHandler handler, Action callback)
			{
				base.LoadState = InfoLoadState.Loading;
				handler.LoadRestorePoint(directory, file, delegate(RestorePointFileWrapper wrapper)
				{
					restorePointWrapper = wrapper;
					base.LoadState = InfoLoadState.LoadComplete;
					callback?.Invoke();
				});
			}
		}

		private struct VersionInfo
		{
			public string file;

			public Version version;
		}

		protected override string GetRestoreDirectory(int slot)
		{
			return GameCoreRuntimeManager.GetRestoreContainerName(slot);
		}

		public override void WriteSaveRestorePoint(int slot, string identifier, bool noDelete, byte[] bytes, Action<bool> callback)
		{
			try
			{
				string directory = GetRestoreDirectory(slot);
				GetRestorePointList(slot, delegate(RestorePointList pointList)
				{
					pointList.TrimAll(19, identifier, this);
					int num = pointList.max + 1;
					string fileName = SaveRestoreHandler.GetFileName(num, noDelete);
					RestorePointFileWrapper restorePointFileWrapper = new RestorePointFileWrapper(bytes, num, identifier);
					restorePointFileWrapper.SetVersion();
					restorePointFileWrapper.SetDateString();
					byte[] bytesForSaveData = SaveRestoreHandler.GetBytesForSaveData(restorePointFileWrapper);
					GameCoreRuntimeManager.Save(directory, fileName, bytesForSaveData, SafeInvoke);
				});
			}
			catch (Exception message)
			{
				Debug.LogError(message);
				SafeInvoke(success: false);
			}
			void SafeInvoke(bool success)
			{
				if (callback != null)
				{
					CoreLoop.InvokeSafe(delegate
					{
						callback(success);
					});
				}
			}
		}

		public override void WriteVersionBackup(int slot, byte[] bytes, Action<bool> callback)
		{
			try
			{
				string directory = GetRestoreDirectory(slot);
				string backupName = GetFullVersionBackupName(slot);
				TrimVersionBackups(slot, 2, delegate
				{
					GameCoreRuntimeManager.Save(directory, backupName, bytes, SafeInvoke);
				});
			}
			catch (Exception message)
			{
				Debug.LogError(message);
				callback?.Invoke(obj: false);
			}
			void SafeInvoke(bool success)
			{
				if (callback != null)
				{
					CoreLoop.InvokeSafe(delegate
					{
						callback(success);
					});
				}
			}
		}

		public override FetchDataRequest FetchRestorePoints(int slot)
		{
			FetchDataRequest request = new FetchDataRequest();
			try
			{
				string directory = GetRestoreDirectory(slot);
				request.State = FetchDataRequest.Status.InProgress;
				GameCoreRuntimeManager.EnumerateFiles(directory, delegate(string[] files)
				{
					if (files == null || files.Length == 0)
					{
						request.State = FetchDataRequest.Status.Completed;
					}
					else
					{
						int returnCount = 0;
						request.RestorePoints = new List<RestorePointFileWrapper>();
						request.State = FetchDataRequest.Status.InProgress;
						foreach (string file in files)
						{
							LoadRestorePoint(directory, file, delegate(RestorePointFileWrapper data)
							{
								if (data != null)
								{
									request.AddResult(data);
								}
								if (Interlocked.Increment(ref returnCount) == files.Length)
								{
									request.State = FetchDataRequest.Status.Completed;
								}
							});
						}
					}
				});
			}
			catch (Exception arg)
			{
				Debug.LogError($"Error while trying to fetch restore points for Slot #{slot} : {arg}");
				request.State = FetchDataRequest.Status.Failed;
			}
			return request;
		}

		public override FetchDataRequest FetchVersionBackupPoints(int slot)
		{
			FetchDataRequest request = new FetchDataRequest();
			try
			{
				string directory = GetRestoreDirectory(slot);
				string searchPattern = GetVersionedBackupName(slot) + "*.dat";
				request.State = FetchDataRequest.Status.InProgress;
				GameCoreRuntimeManager.EnumerateFiles(directory, delegate(string[] files)
				{
					if (files == null || files.Length == 0)
					{
						request.State = FetchDataRequest.Status.Completed;
					}
					else
					{
						files = SaveRestoreHandler.FilterResults(files, searchPattern);
						if (files == null || files.Length == 0)
						{
							request.State = FetchDataRequest.Status.Completed;
						}
						else
						{
							int returnCount = 0;
							request.RestorePoints = new List<RestorePointFileWrapper>();
							request.State = FetchDataRequest.Status.InProgress;
							foreach (string fileName in files)
							{
								LoadBackupPoint(directory, fileName, delegate(RestorePointFileWrapper data)
								{
									if (data != null)
									{
										request.AddResult(data);
									}
									if (Interlocked.Increment(ref returnCount) == files.Length)
									{
										request.State = FetchDataRequest.Status.Completed;
									}
								});
							}
						}
					}
				});
			}
			catch (Exception arg)
			{
				Debug.LogError($"Error while trying to load versioned backups for Slot # {slot} : {arg}");
				request.State = FetchDataRequest.Status.Failed;
			}
			return request;
		}

		public override void DeleteRestorePoints(int slot, Action<bool> callback)
		{
			string restoreDirectory = GetRestoreDirectory(slot);
			try
			{
				GameCoreRuntimeManager.DeleteContainer(restoreDirectory, callback);
			}
			catch (Exception message)
			{
				Debug.LogError(message);
				callback(obj: false);
			}
		}

		public override void DeleteVersionBackups(int slot, Action<bool> callback)
		{
			try
			{
				string directory = GetRestoreDirectory(slot);
				string searchPattern = GetVersionedBackupName(slot) + "*.dat";
				GameCoreRuntimeManager.EnumerateFiles(directory, delegate(string[] files)
				{
					if (files != null && files.Length != 0)
					{
						files = SaveRestoreHandler.FilterResults(files, searchPattern);
						if (files != null && files.Length != 0)
						{
							string[] array = files;
							foreach (string blobName in array)
							{
								try
								{
									GameCoreRuntimeManager.DeleteBlob(directory, blobName, null);
								}
								catch (Exception message)
								{
									Debug.LogError(message);
								}
							}
							callback?.SafeInvoke(value: true);
						}
					}
				});
			}
			catch (Exception message2)
			{
				callback?.Invoke(obj: false);
				Debug.LogError(message2);
			}
		}

		private void TrimVersionBackups(int slot, int count, Action callback)
		{
			Task.Run(delegate
			{
				try
				{
					string directory = GetRestoreDirectory(slot);
					string versionedBackupName = GetVersionedBackupName(slot);
					string searchFilter = versionedBackupName + "*.dat";
					GameCoreRuntimeManager.EnumerateFiles(directory, delegate(string[] files)
					{
						if (files == null || files.Length == 0)
						{
							SafeInvoke();
						}
						else
						{
							files = SaveRestoreHandler.FilterResults(files, searchFilter);
							if (files == null || files.Length == 0)
							{
								SafeInvoke();
							}
							else
							{
								List<VersionInfo> list = new List<VersionInfo>();
								string[] array = files;
								foreach (string text in array)
								{
									VersionInfo item = default(VersionInfo);
									item.file = text;
									string fileNameWithoutExtension = Path.GetFileNameWithoutExtension(text);
									fileNameWithoutExtension = fileNameWithoutExtension.Replace(versionedBackupName, "");
									fileNameWithoutExtension = SaveDataUtility.CleanupVersionText(fileNameWithoutExtension);
									try
									{
										item.version = new Version(fileNameWithoutExtension);
									}
									catch (Exception message)
									{
										Debug.LogError(message);
										item.version = new Version(0, 0, 0, 0);
									}
									bool flag = false;
									for (int j = 0; j < list.Count; j++)
									{
										VersionInfo versionInfo = list[j];
										if (item.version >= versionInfo.version)
										{
											list.Insert(j, item);
											flag = true;
											break;
										}
									}
									if (!flag)
									{
										list.Add(item);
									}
								}
								int num = files.Length - count;
								if (num > 0)
								{
									for (int num2 = list.Count - 1; num2 >= 0; num2--)
									{
										VersionInfo versionInfo2 = list[num2];
										try
										{
											GameCoreRuntimeManager.DeleteBlob(directory, versionInfo2.file, null);
										}
										catch (Exception message2)
										{
											Debug.LogError(message2);
										}
										if (--num == 0)
										{
											break;
										}
									}
								}
								SafeInvoke();
							}
						}
					});
				}
				catch (Exception message3)
				{
					Debug.LogError(message3);
					SafeInvoke();
				}
			});
			void SafeInvoke()
			{
				callback?.Invoke();
			}
		}

		private void GetRestorePointList(int slot, Action<RestorePointList> callback)
		{
			RestorePointList result = new RestorePointList();
			Task.Run(delegate
			{
				try
				{
					string directory = GetRestoreDirectory(slot);
					GameCoreRuntimeManager.EnumerateFiles(directory, delegate(string[] files)
					{
						if (files == null || files.Length == 0)
						{
							SafeInvoke();
						}
						else
						{
							result.infos = new List<GameCoreInfo>(files.Length);
							result.noDeleteInfo = new List<GameCoreInfo>(files.Length);
							foreach (string text in files)
							{
								GameCoreInfo gameCoreInfo = new GameCoreInfo
								{
									file = text,
									directory = directory
								};
								RestorePointFileInfo restorePointFileInfo = new RestorePointFileInfo(text);
								gameCoreInfo.number = restorePointFileInfo.number;
								if (restorePointFileInfo.number > result.max)
								{
									result.max = restorePointFileInfo.number;
								}
								if (restorePointFileInfo.isNoDelete)
								{
									result.noDeleteInfo.Add(gameCoreInfo);
								}
								else
								{
									result.infos.Add(gameCoreInfo);
								}
							}
							result.CompleteLoad(this, SafeInvoke);
						}
					});
				}
				catch (Exception ex)
				{
					SafeInvoke();
					Debug.LogError(ex.ToString());
				}
			});
			void SafeInvoke()
			{
				callback?.Invoke(result);
			}
		}

		private void LoadRestorePoint(string directory, string file, Action<RestorePointFileWrapper> callback)
		{
			try
			{
				GameCoreRuntimeManager.LoadSaveData(directory, file, delegate(byte[] bytes)
				{
					if (bytes != null)
					{
						RestorePointFileWrapper obj = SaveDataUtility.DeserializeSaveData<RestorePointFileWrapper>(SaveRestoreHandler.GetJsonForSaveBytes(bytes));
						callback?.Invoke(obj);
					}
					else
					{
						callback?.Invoke(null);
					}
				});
			}
			catch (Exception message)
			{
				Debug.LogError(message);
				callback?.Invoke(null);
			}
		}

		private void LoadBackupPoint(string directory, string fileName, Action<RestorePointFileWrapper> callback)
		{
			try
			{
				GameCoreRuntimeManager.LoadSaveData(directory, fileName, delegate(byte[] byteData)
				{
					if (byteData != null)
					{
						RestorePointFileWrapper obj = new RestorePointFileWrapper(byteData);
						callback?.Invoke(obj);
					}
					else
					{
						callback?.Invoke(null);
					}
				});
			}
			catch (Exception message)
			{
				Debug.LogError(message);
				callback?.Invoke(null);
			}
		}
	}
}

using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using UnityEngine;

public sealed class DesktopSaveRestoreHandler : SaveRestoreHandler
{
	private sealed class DesktopInfo : Info<DesktopSaveRestoreHandler>
	{
		public override void Delete()
		{
			if (!string.IsNullOrEmpty(file))
			{
				File.Delete(file);
			}
		}

		public override void Commit()
		{
		}
	}

	private new sealed class RestorePointList : RestorePointList<DesktopInfo, DesktopSaveRestoreHandler>
	{
		public override void PrepareDelete()
		{
		}

		public override void Delete(DesktopInfo info)
		{
			base.Delete(info);
		}

		public override void Commit()
		{
		}
	}

	private struct VersionInfo
	{
		public string file;

		public Version version;
	}

	private readonly string DATA_PATH;

	public DesktopSaveRestoreHandler(string dataPath)
	{
		DATA_PATH = dataPath;
	}

	public override void WriteSaveRestorePoint(int slot, string identifier, bool noDelete, byte[] bytes, Action<bool> callback)
	{
		Task.Run(delegate
		{
			try
			{
				string directoryPath = GetDirectoryPath(slot);
				if (!Directory.Exists(directoryPath))
				{
					Directory.CreateDirectory(directoryPath);
					if (!Directory.Exists(directoryPath))
					{
						Debug.LogError("Failed to create directory " + directoryPath);
						callback?.Invoke(obj: false);
						return;
					}
				}
				RestorePointList restorePointList = GetRestorePointList(slot);
				restorePointList.TrimAll(19, identifier, this);
				int num = restorePointList.max + 1;
				string fileName = SaveRestoreHandler.GetFileName(num, noDelete);
				string path = Path.Combine(GetDirectoryPath(slot), fileName);
				RestorePointFileWrapper restorePointFileWrapper = new RestorePointFileWrapper(bytes, num, identifier);
				restorePointFileWrapper.SetVersion();
				restorePointFileWrapper.SetDateString();
				byte[] bytesForSaveData = SaveRestoreHandler.GetBytesForSaveData(restorePointFileWrapper);
				File.WriteAllBytes(path, bytesForSaveData);
				callback?.Invoke(obj: true);
			}
			catch (Exception message)
			{
				Debug.LogError(message);
				callback?.Invoke(obj: false);
			}
		});
	}

	public override void WriteVersionBackup(int slot, byte[] bytes, Action<bool> callback)
	{
		Task.Run(delegate
		{
			try
			{
				string fullVersionBackupPath = GetFullVersionBackupPath(slot);
				TrimVersionBackups(slot, 2);
				File.WriteAllBytes(fullVersionBackupPath, bytes);
				callback?.Invoke(obj: true);
			}
			catch (Exception exception)
			{
				Debug.LogException(exception);
				callback?.Invoke(obj: false);
			}
		});
	}

	public override FetchDataRequest FetchRestorePoints(int slot)
	{
		FetchDataRequest request = new FetchDataRequest();
		request.State = FetchDataRequest.Status.InProgress;
		Task.Run(delegate
		{
			string directoryPath = GetDirectoryPath(slot);
			if (!Directory.Exists(directoryPath))
			{
				request.State = FetchDataRequest.Status.Completed;
			}
			else
			{
				string[] files = Directory.GetFiles(directoryPath, "*.dat");
				if (files.Length == 0)
				{
					request.State = FetchDataRequest.Status.Completed;
				}
				else
				{
					int returnCount = 0;
					request.RestorePoints = new List<RestorePointFileWrapper>();
					foreach (string path in files)
					{
						LoadRestorePoint(path, delegate(RestorePointFileWrapper data)
						{
							returnCount++;
							if (data != null)
							{
								request.AddResult(data);
							}
							if (returnCount == files.Length)
							{
								request.State = FetchDataRequest.Status.Completed;
							}
						});
					}
				}
			}
		});
		return request;
	}

	public override FetchDataRequest FetchVersionBackupPoints(int slot)
	{
		FetchDataRequest request = new FetchDataRequest();
		request.State = FetchDataRequest.Status.InProgress;
		Task.Run(delegate
		{
			string dATA_PATH = DATA_PATH;
			if (!Directory.Exists(dATA_PATH))
			{
				request.State = FetchDataRequest.Status.Completed;
			}
			else
			{
				string searchPattern = GetVersionedBackupName(slot) + "*.dat";
				string[] files = Directory.GetFiles(dATA_PATH, searchPattern);
				if (files.Length == 0)
				{
					request.State = FetchDataRequest.Status.Completed;
				}
				else
				{
					int returnCount = 0;
					request.RestorePoints = new List<RestorePointFileWrapper>();
					foreach (string path in files)
					{
						LoadBackupPoint(path, delegate(RestorePointFileWrapper data)
						{
							returnCount++;
							if (data != null)
							{
								request.AddResult(data);
							}
							if (returnCount == files.Length)
							{
								request.State = FetchDataRequest.Status.Completed;
							}
						});
					}
				}
			}
		});
		return request;
	}

	public override void DeleteRestorePoints(int slot, Action<bool> callback)
	{
		Task.Run(delegate
		{
			string directoryPath = GetDirectoryPath(slot);
			if (!Directory.Exists(directoryPath))
			{
				callback(obj: true);
				return;
			}
			try
			{
				Directory.Delete(directoryPath, recursive: true);
				callback(obj: true);
			}
			catch (Exception message)
			{
				Debug.LogError(message);
				callback(obj: false);
			}
		});
	}

	public override void DeleteVersionBackups(int slot, Action<bool> callback)
	{
		Task.Run(delegate
		{
			string dATA_PATH = DATA_PATH;
			if (Directory.Exists(dATA_PATH))
			{
				string searchPattern = GetVersionedBackupName(slot) + "*.dat";
				string[] files = Directory.GetFiles(dATA_PATH, searchPattern);
				if (files.Length != 0)
				{
					string[] array = files;
					foreach (string path in array)
					{
						try
						{
							File.Delete(path);
						}
						catch (Exception message)
						{
							Debug.LogError(message);
						}
					}
				}
			}
		});
	}

	private string GetDirectoryPath(int slot)
	{
		return Path.Combine(DATA_PATH, SaveRestoreHandler.GetDirectoryName(slot));
	}

	private string GetFullVersionBackupPath(int slot)
	{
		return Path.Combine(DATA_PATH, GetFullVersionBackupName(slot));
	}

	private void TrimVersionBackups(int slot, int count)
	{
		string dATA_PATH = DATA_PATH;
		if (!Directory.Exists(dATA_PATH))
		{
			return;
		}
		string versionedBackupName = GetVersionedBackupName(slot);
		string[] files = Directory.GetFiles(dATA_PATH, versionedBackupName + "*.dat");
		if (files.Length <= count)
		{
			return;
		}
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
		if (num <= 0)
		{
			return;
		}
		int num2 = list.Count - 1;
		while (num2 >= 0)
		{
			VersionInfo versionInfo2 = list[num2];
			try
			{
				File.Delete(versionInfo2.file);
			}
			catch (Exception message2)
			{
				Debug.LogError(message2);
			}
			if (--num > 0)
			{
				num2--;
				continue;
			}
			break;
		}
	}

	private RestorePointList GetRestorePointList(int slot)
	{
		RestorePointList restorePointList = new RestorePointList();
		string directoryPath = GetDirectoryPath(slot);
		if (!Directory.Exists(directoryPath))
		{
			Debug.LogError("Directory: \"" + directoryPath + "\" does not exist");
			return restorePointList;
		}
		string[] files = Directory.GetFiles(directoryPath, "*.dat");
		if (files.Length == 0)
		{
			return restorePointList;
		}
		restorePointList.infos = new List<DesktopInfo>(files.Length);
		restorePointList.noDeleteInfo = new List<DesktopInfo>(files.Length);
		foreach (string text in files)
		{
			DesktopInfo desktopInfo = new DesktopInfo();
			desktopInfo.file = text;
			RestorePointFileInfo restorePointFileInfo = new RestorePointFileInfo(text);
			desktopInfo.number = restorePointFileInfo.number;
			if (restorePointFileInfo.number > restorePointList.max)
			{
				restorePointList.max = restorePointFileInfo.number;
			}
			if (restorePointFileInfo.isNoDelete)
			{
				restorePointList.noDeleteInfo.Add(desktopInfo);
			}
			else
			{
				restorePointList.infos.Add(desktopInfo);
			}
		}
		restorePointList.CompleteLoad(this, null);
		return restorePointList;
	}

	protected override void LoadRestorePoint(string path, Action<RestorePointFileWrapper> callback)
	{
		try
		{
			RestorePointFileWrapper obj = SaveDataUtility.DeserializeSaveData<RestorePointFileWrapper>(SaveRestoreHandler.GetJsonForSaveBytes(File.ReadAllBytes(path)));
			callback?.Invoke(obj);
		}
		catch (Exception message)
		{
			Debug.LogError(message);
			callback?.Invoke(null);
		}
	}

	private void LoadBackupPoint(string path, Action<RestorePointFileWrapper> callback)
	{
		try
		{
			RestorePointFileWrapper obj = new RestorePointFileWrapper(File.ReadAllBytes(path));
			callback?.Invoke(obj);
		}
		catch (Exception message)
		{
			Debug.LogError(message);
			callback?.Invoke(null);
		}
	}
}

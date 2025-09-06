using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using TeamCherry.SharedUtils;
using UnityEngine;

public abstract class SaveRestoreHandler
{
	protected enum InfoLoadState
	{
		None = 0,
		Loading = 1,
		LoadComplete = 2
	}

	protected class Info<T> where T : SaveRestoreHandler
	{
		public string file;

		public int number;

		public RestorePointFileWrapper restorePointWrapper;

		private volatile InfoLoadState loadState;

		public InfoLoadState LoadState
		{
			get
			{
				return loadState;
			}
			protected set
			{
				loadState = value;
			}
		}

		public virtual void Delete()
		{
		}

		public virtual void Commit()
		{
		}

		public virtual void LoadData(T handler, Action callback = null)
		{
			LoadState = InfoLoadState.Loading;
			handler.LoadRestorePoint(file, delegate(RestorePointFileWrapper wrapper)
			{
				restorePointWrapper = wrapper;
				LoadState = InfoLoadState.LoadComplete;
				callback?.Invoke();
			});
		}
	}

	protected abstract class RestorePointList<T, TV> where T : Info<TV> where TV : SaveRestoreHandler
	{
		public int max;

		public List<T> infos;

		public List<T> noDeleteInfo;

		public void TrimAll(int limit, string identifier, TV handler)
		{
			bool flag = false;
			if (Trim(limit, commit: false))
			{
				flag = true;
			}
			if (TrimNoDelete(identifier, handler, commit: false))
			{
				flag = true;
			}
			if (flag)
			{
				Commit();
			}
		}

		public bool Trim(int limit, bool commit = true)
		{
			if (infos == null)
			{
				return false;
			}
			if (infos.Count < limit)
			{
				return false;
			}
			int num = infos.Count - limit;
			if (num <= 0)
			{
				return false;
			}
			infos.Sort(delegate(T a, T b)
			{
				if (a.number > b.number)
				{
					return 1;
				}
				return (a.number < b.number) ? (-1) : 0;
			});
			bool flag = false;
			PrepareDelete();
			foreach (T info in infos)
			{
				if (info.number < 0)
				{
					continue;
				}
				try
				{
					Delete(info);
					flag = true;
					if (--num <= 0)
					{
						break;
					}
				}
				catch (Exception message)
				{
					Debug.LogError(message);
				}
			}
			if (flag && commit)
			{
				Commit();
			}
			return flag;
		}

		public bool TrimNoDelete(string identifier, TV handler, bool commit = true)
		{
			if (noDeleteInfo == null)
			{
				return false;
			}
			bool flag = false;
			if (noDeleteInfo.Count >= 2)
			{
				string.IsNullOrEmpty(identifier);
				int num = 0;
				for (int i = 0; i < noDeleteInfo.Count; i++)
				{
					T val = noDeleteInfo[i];
					if (val.restorePointWrapper == null)
					{
						continue;
					}
					string text = val.restorePointWrapper.identifier;
					if (string.IsNullOrEmpty(text))
					{
						try
						{
							RestorePointData restorePointData = SaveDataUtility.DeserializeSaveData<RestorePointData>(GameManager.GetJsonForSaveBytesStatic(val.restorePointWrapper.data));
							if (restorePointData != null)
							{
								text = restorePointData.autoSaveName.ToString();
								val.restorePointWrapper.identifier = text;
								goto IL_00b4;
							}
						}
						catch (Exception)
						{
						}
						continue;
					}
					goto IL_00b4;
					IL_00b4:
					if (!(text != identifier))
					{
						noDeleteInfo[num++] = val;
					}
				}
				if (num < noDeleteInfo.Count)
				{
					noDeleteInfo.RemoveRange(num, noDeleteInfo.Count - num);
				}
				if (noDeleteInfo.Count >= 2)
				{
					noDeleteInfo = (from e in noDeleteInfo
						orderby e.restorePointWrapper.date descending, e.number descending
						select e).ToList();
					for (int j = 1; j < noDeleteInfo.Count; j++)
					{
						noDeleteInfo[j].Delete();
						flag = true;
					}
					if (flag && commit)
					{
						Commit();
					}
				}
			}
			return flag;
		}

		public abstract void PrepareDelete();

		public virtual void Delete(T info)
		{
			info?.Delete();
		}

		public abstract void Commit();

		public virtual void CompleteLoad(TV handler, Action callback)
		{
			int count;
			int expected;
			if (noDeleteInfo.Count > 0)
			{
				count = 0;
				expected = noDeleteInfo.Count;
				{
					foreach (T item in noDeleteInfo)
					{
						item.LoadData(handler, Complete);
					}
					return;
				}
			}
			callback?.Invoke();
			void Complete()
			{
				bool flag = true;
				Interlocked.Increment(ref count);
				foreach (T item2 in noDeleteInfo)
				{
					if (item2.LoadState != InfoLoadState.LoadComplete)
					{
						flag = false;
						break;
					}
				}
				if (!flag && count >= expected)
				{
					Debug.LogError($"Complete and load count miss matched {count}");
				}
				if (flag)
				{
					callback?.Invoke();
				}
			}
		}
	}

	protected readonly struct RestorePointFileInfo
	{
		public readonly int number;

		public readonly bool isNoDelete;

		private static readonly int FILE_NAME_LENGTH = "restoreData".Length;

		private static readonly int NO_DELETE_LENGTH = "NODEL".Length;

		public RestorePointFileInfo(string path)
		{
			string text = Path.GetFileNameWithoutExtension(path);
			isNoDelete = text.StartsWith("NODEL");
			if (isNoDelete)
			{
				text = text.Substring(NO_DELETE_LENGTH, text.Length - NO_DELETE_LENGTH);
			}
			text = text.Substring(FILE_NAME_LENGTH, text.Length - FILE_NAME_LENGTH);
			if (!int.TryParse(text, out number))
			{
				Debug.LogError("Failed to parse number from \"" + path + "\"");
				number = -1;
			}
		}
	}

	public const string FOLDER_NAME = "Restore_Points";

	public const string FILE_NAME = "restoreData";

	public const string NO_DELETE = "NODEL";

	public const string SAVE_FILE_EXT = ".dat";

	protected const int FILE_LIMIT = 20;

	protected const int NO_DELETE_TRIM_THRESHOLD = 2;

	public const int VERSION_BACKUP_LIMIT = 3;

	public static int TotalFileLimit => 30;

	public virtual int FileLimit => 5;

	protected virtual string GetRestoreDirectory(int slot)
	{
		return GetDirectoryName(slot);
	}

	public static string GetTitle(int slot)
	{
		return $"Restore Data #{slot}";
	}

	public static string GetFileName(int slot, bool noDelete = false)
	{
		if (noDelete)
		{
			return GetNoDeleteFileName(slot);
		}
		return string.Format("{0}{1}{2}", "restoreData", slot, ".dat");
	}

	public static string GetNoDeleteFileName(int slot)
	{
		return "NODEL" + GetFileName(slot);
	}

	public static string GetDirectoryName(int slot)
	{
		return string.Format("{0}{1}", "Restore_Points", slot);
	}

	protected virtual string GetVersionedBackupName(int slot)
	{
		return $"user{slot}_";
	}

	protected string GetFullVersionBackupName(int slot)
	{
		return GetVersionedBackupName(slot) + "1.0.28324.dat";
	}

	protected static string GetJsonForSaveBytes(byte[] fileBytes)
	{
		if (GameManager.instance.gameConfig.useSaveEncryption && !Platform.Current.IsFileSystemProtected)
		{
			BinaryFormatter binaryFormatter = new BinaryFormatter();
			MemoryStream serializationStream = new MemoryStream(fileBytes);
			return Encryption.Decrypt((string)binaryFormatter.Deserialize(serializationStream));
		}
		return Encoding.UTF8.GetString(fileBytes);
	}

	protected static byte[] GetBytesForSaveJson(string jsonData)
	{
		byte[] result;
		if (GameManager.instance.gameConfig.useSaveEncryption && !Platform.Current.IsFileSystemProtected)
		{
			string graph = Encryption.Encrypt(jsonData);
			BinaryFormatter binaryFormatter = new BinaryFormatter();
			MemoryStream memoryStream = new MemoryStream();
			binaryFormatter.Serialize(memoryStream, graph);
			result = memoryStream.ToArray();
			memoryStream.Close();
		}
		else
		{
			result = Encoding.UTF8.GetBytes(jsonData);
		}
		return result;
	}

	protected static byte[] GetBytesForSaveData<T>(T data)
	{
		return GetBytesForSaveJson(SaveDataUtility.SerializeSaveData(data));
	}

	public virtual void WriteSaveRestorePoint(int slot, string identifier, bool noDelete, byte[] bytes, Action<bool> callback)
	{
		callback?.Invoke(obj: false);
	}

	public abstract void WriteVersionBackup(int slot, byte[] bytes, Action<bool> callback);

	public abstract FetchDataRequest FetchRestorePoints(int slot);

	public abstract FetchDataRequest FetchVersionBackupPoints(int slot);

	protected virtual void LoadRestorePoint(string path, Action<RestorePointFileWrapper> callback)
	{
		callback?.Invoke(null);
	}

	public abstract void DeleteRestorePoints(int slot, Action<bool> callback);

	public abstract void DeleteVersionBackups(int slot, Action<bool> callback);

	protected static string[] FilterResults(string[] source, string searchPattern)
	{
		if (source == null || string.IsNullOrEmpty(searchPattern))
		{
			return source;
		}
		string pattern = WildcardToRegex(searchPattern);
		List<string> list = new List<string>();
		foreach (string text in source)
		{
			if (Regex.IsMatch(text, pattern))
			{
				list.Add(text);
			}
			else if (text.Contains(searchPattern))
			{
				list.Add(text);
			}
		}
		return list.ToArray();
	}

	private static string WildcardToRegex(string pattern)
	{
		return "^" + Regex.Escape(pattern).Replace("\\*", ".*").Replace("\\?", ".") + "$";
	}
}

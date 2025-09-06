using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;
using TeamCherry.SharedUtils;
using UnityEngine;

public abstract class CustomSharedData : Platform.ISharedData
{
	[Serializable]
	private class SharedDataSerializableBlob
	{
		[SerializeField]
		private SharedDataSerializablePair[] pairs;

		public static SharedDataSerializableBlob FromSharedData(Dictionary<string, string> sharedData)
		{
			List<SharedDataSerializablePair> list = new List<SharedDataSerializablePair>();
			foreach (KeyValuePair<string, string> sharedDatum in sharedData)
			{
				if (sharedDatum.Key == null)
				{
					Debug.LogErrorFormat("Null key found in shared data");
					continue;
				}
				if (sharedDatum.Value == null)
				{
					Debug.LogErrorFormat("Null value for key '{0}' found in shared data");
					continue;
				}
				list.Add(new SharedDataSerializablePair
				{
					Key = sharedDatum.Key,
					Value = sharedDatum.Value
				});
			}
			return new SharedDataSerializableBlob
			{
				pairs = list.ToArray()
			};
		}

		public void ToSharedData(Dictionary<string, string> sharedData)
		{
			sharedData.Clear();
			int num = 0;
			while (pairs != null && num < pairs.Length)
			{
				SharedDataSerializablePair sharedDataSerializablePair = pairs[num];
				if (sharedDataSerializablePair.Key == null)
				{
					Debug.LogErrorFormat("Null key found in shared data");
				}
				else if (sharedDataSerializablePair.Value == null)
				{
					Debug.LogErrorFormat("Null value for key '{0}' found in shared data", sharedDataSerializablePair.Key);
				}
				else if (sharedData.ContainsKey(sharedDataSerializablePair.Key))
				{
					Debug.LogErrorFormat("Duplicate key '{0}' found in shared data", sharedDataSerializablePair.Key);
				}
				else
				{
					sharedData.Add(sharedDataSerializablePair.Key, sharedDataSerializablePair.Value);
				}
				num++;
			}
		}
	}

	[Serializable]
	private struct SharedDataSerializablePair
	{
		public string Key;

		public string Value;
	}

	public interface IResponder
	{
		void OnModified(CustomSharedData sharedData);
	}

	private Dictionary<string, string> sharedData;

	private IResponder responder;

	public Dictionary<string, string> SharedData => sharedData;

	public IResponder Responder
	{
		get
		{
			return responder;
		}
		set
		{
			responder = value;
		}
	}

	protected CustomSharedData()
	{
		sharedData = new Dictionary<string, string>();
	}

	public void LoadFromJSON(string str)
	{
		JsonUtility.FromJson<SharedDataSerializableBlob>(str).ToSharedData(sharedData);
	}

	public string SaveToJSON()
	{
		return JsonUtility.ToJson(SharedDataSerializableBlob.FromSharedData(sharedData));
	}

	protected static byte[] JsonToBytes(string jsonData, bool useEncryption)
	{
		byte[] result;
		if (useEncryption)
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

	protected static string BytesToJson(byte[] byteData, bool useEncryption)
	{
		if (useEncryption)
		{
			BinaryFormatter binaryFormatter = new BinaryFormatter();
			MemoryStream serializationStream = new MemoryStream(byteData);
			return Encryption.Decrypt((string)binaryFormatter.Deserialize(serializationStream));
		}
		return Encoding.UTF8.GetString(byteData);
	}

	public bool HasKey(string key)
	{
		return sharedData.ContainsKey(key);
	}

	public void DeleteKey(string key)
	{
		if (sharedData.Remove(key))
		{
			OnModified();
		}
	}

	public void DeleteAll()
	{
		if (sharedData.Count > 0)
		{
			sharedData.Clear();
			OnModified();
		}
	}

	public virtual void ImportData(Platform.ISharedData otherData)
	{
		if (otherData == null || !(otherData is CustomSharedData customSharedData))
		{
			return;
		}
		foreach (KeyValuePair<string, string> sharedDatum in customSharedData.sharedData)
		{
			if (!sharedData.ContainsKey(sharedDatum.Key))
			{
				sharedData.Add(sharedDatum.Key, sharedDatum.Value);
			}
		}
	}

	public bool GetBool(string key, bool def)
	{
		return GetInt(key, def ? 1 : 0) > 0;
	}

	public void SetBool(string key, bool val)
	{
		SetInt(key, val ? 1 : 0);
	}

	public int GetInt(string key, int def)
	{
		if (!sharedData.TryGetValue(key, out var value))
		{
			return def;
		}
		if (!int.TryParse(value, out var result))
		{
			return def;
		}
		return result;
	}

	public void SetInt(string key, int val)
	{
		string text = val.ToString();
		if (!sharedData.ContainsKey(key) || sharedData[key] != text)
		{
			sharedData[key] = text;
			OnModified();
		}
	}

	public float GetFloat(string key, float def)
	{
		if (!sharedData.TryGetValue(key, out var value))
		{
			return def;
		}
		if (!float.TryParse(value, NumberStyles.Float, CultureInfo.InvariantCulture, out var result))
		{
			return def;
		}
		return result;
	}

	public void SetFloat(string key, float val)
	{
		string text = val.ToString(CultureInfo.InvariantCulture);
		if (!sharedData.ContainsKey(key) || sharedData[key] != text)
		{
			sharedData[key] = text;
			OnModified();
		}
	}

	public string GetString(string key, string def)
	{
		if (!sharedData.TryGetValue(key, out var value))
		{
			return def;
		}
		return value;
	}

	public void SetString(string key, string val)
	{
		if (!sharedData.ContainsKey(key) || sharedData[key] != val)
		{
			sharedData[key] = val;
			OnModified();
		}
	}

	public abstract void Save();

	protected virtual void OnModified()
	{
		if (responder != null)
		{
			responder.OnModified(this);
		}
	}
}

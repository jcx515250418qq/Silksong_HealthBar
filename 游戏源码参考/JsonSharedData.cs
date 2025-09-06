using System;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;
using TeamCherry.SharedUtils;
using UnityEngine;

public class JsonSharedData : CustomSharedData
{
	private readonly bool useEncryption;

	private readonly string saveDir;

	private readonly string dataPath;

	public JsonSharedData(string saveDir, string fileName, bool useEncryption)
	{
		this.saveDir = saveDir;
		dataPath = Path.Combine(saveDir, fileName);
		this.useEncryption = useEncryption;
		Load();
	}

	public override void Save()
	{
		SaveDataUtility.AddTaskToAsyncQueue(delegate
		{
			try
			{
				string text = SaveToJSON();
				byte[] bytes;
				if (useEncryption)
				{
					string graph = Encryption.Encrypt(text);
					BinaryFormatter binaryFormatter = new BinaryFormatter();
					MemoryStream memoryStream = new MemoryStream();
					binaryFormatter.Serialize(memoryStream, graph);
					bytes = memoryStream.ToArray();
					memoryStream.Close();
				}
				else
				{
					bytes = Encoding.UTF8.GetBytes(text);
				}
				if (!Directory.Exists(saveDir))
				{
					Directory.CreateDirectory(saveDir);
				}
				File.WriteAllBytes(dataPath, bytes);
			}
			catch (Exception exception)
			{
				Debug.LogException(exception);
			}
		});
	}

	public void Load()
	{
		string path = dataPath;
		if (!File.Exists(path))
		{
			return;
		}
		try
		{
			byte[] array = File.ReadAllBytes(path);
			string str;
			if (useEncryption)
			{
				BinaryFormatter binaryFormatter = new BinaryFormatter();
				MemoryStream serializationStream = new MemoryStream(array);
				str = Encryption.Decrypt((string)binaryFormatter.Deserialize(serializationStream));
			}
			else
			{
				str = Encoding.UTF8.GetString(array);
			}
			LoadFromJSON(str);
		}
		catch (Exception message)
		{
			Debug.LogError(message);
		}
	}
}

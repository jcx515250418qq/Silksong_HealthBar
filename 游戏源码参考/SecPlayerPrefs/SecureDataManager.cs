using System.IO;
using System.Text;
using System.Xml;
using System.Xml.Serialization;

namespace SecPlayerPrefs
{
	public class SecureDataManager<T> where T : new()
	{
		private T stats;

		private string key;

		public SecureDataManager(string filename)
		{
			key = filename;
			stats = Load();
		}

		public T Get()
		{
			return stats;
		}

		private T Load()
		{
			if (!SecurePlayerPrefs.HasKey(key))
			{
				return new T();
			}
			string @string = SecurePlayerPrefs.GetString(key);
			return DeserializeObject(@string);
		}

		public void Save(T stats)
		{
			string value = SerializeObject(stats);
			SecurePlayerPrefs.SetString(key, value);
			SecurePlayerPrefs.Save();
		}

		private string SerializeObject(T pObject)
		{
			MemoryStream w = new MemoryStream();
			XmlSerializer xmlSerializer = new XmlSerializer(typeof(T));
			XmlTextWriter xmlTextWriter = new XmlTextWriter(w, Encoding.UTF8);
			xmlSerializer.Serialize(xmlTextWriter, pObject);
			return UTF8ByteArrayToString(((MemoryStream)xmlTextWriter.BaseStream).ToArray());
		}

		private T DeserializeObject(string pXmlizedString)
		{
			XmlSerializer xmlSerializer = new XmlSerializer(typeof(T));
			MemoryStream memoryStream = new MemoryStream(StringToUTF8ByteArray(pXmlizedString));
			new XmlTextWriter(memoryStream, Encoding.UTF8);
			return (T)xmlSerializer.Deserialize(memoryStream);
		}

		private static string UTF8ByteArrayToString(byte[] characters)
		{
			return new UTF8Encoding().GetString(characters);
		}

		private static byte[] StringToUTF8ByteArray(string pXmlString)
		{
			return new UTF8Encoding().GetBytes(pXmlString);
		}
	}
}

using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Text;
using UnityEngine;

public class INIParser
{
	public int error;

	private object m_Lock = new object();

	private string m_FileName;

	private string m_iniString;

	private bool m_AutoFlush;

	private Dictionary<string, Dictionary<string, string>> m_Sections = new Dictionary<string, Dictionary<string, string>>();

	private Dictionary<string, Dictionary<string, string>> m_Modified = new Dictionary<string, Dictionary<string, string>>();

	private bool m_CacheModified;

	public string FileName => m_FileName;

	public string iniString => m_iniString;

	public void Open(string path)
	{
		m_FileName = path;
		if (File.Exists(m_FileName))
		{
			m_iniString = File.ReadAllText(m_FileName);
		}
		else
		{
			File.Create(m_FileName).Close();
			m_iniString = "";
		}
		Initialize(m_iniString, AutoFlush: false);
	}

	public void Open(TextAsset name)
	{
		if (name == null)
		{
			error = 1;
			m_iniString = "";
			m_FileName = null;
			Initialize(m_iniString, AutoFlush: false);
			return;
		}
		m_FileName = Application.persistentDataPath + name.name;
		if (File.Exists(m_FileName))
		{
			m_iniString = File.ReadAllText(m_FileName);
		}
		else
		{
			m_iniString = name.text;
		}
		Initialize(m_iniString, AutoFlush: false);
	}

	public void OpenFromString(string str)
	{
		m_FileName = null;
		Initialize(str, AutoFlush: false);
	}

	public override string ToString()
	{
		return m_iniString;
	}

	private void Initialize(string iniString, bool AutoFlush)
	{
		m_iniString = iniString;
		m_AutoFlush = AutoFlush;
		Refresh();
	}

	public void Close()
	{
		lock (m_Lock)
		{
			PerformFlush();
			m_FileName = null;
			m_iniString = null;
		}
	}

	private string ParseSectionName(string Line)
	{
		if (!Line.StartsWith("["))
		{
			return null;
		}
		if (!Line.EndsWith("]"))
		{
			return null;
		}
		if (Line.Length < 3)
		{
			return null;
		}
		return Line.Substring(1, Line.Length - 2);
	}

	private bool ParseKeyValuePair(string Line, ref string Key, ref string Value)
	{
		int num;
		if ((num = Line.IndexOf('=')) <= 0)
		{
			return false;
		}
		int num2 = Line.Length - num - 1;
		Key = Line.Substring(0, num).Trim();
		if (Key.Length <= 0)
		{
			return false;
		}
		Value = ((num2 > 0) ? Line.Substring(num + 1, num2).Trim() : "");
		return true;
	}

	private bool isComment(string Line)
	{
		string Key = null;
		string Value = null;
		if (ParseSectionName(Line) != null)
		{
			return false;
		}
		if (ParseKeyValuePair(Line, ref Key, ref Value))
		{
			return false;
		}
		return true;
	}

	private void Refresh()
	{
		lock (m_Lock)
		{
			StringReader stringReader = null;
			try
			{
				m_Sections.Clear();
				m_Modified.Clear();
				stringReader = new StringReader(m_iniString);
				Dictionary<string, string> dictionary = null;
				string Key = null;
				string Value = null;
				string text;
				while ((text = stringReader.ReadLine()) != null)
				{
					text = text.Trim();
					string text2 = ParseSectionName(text);
					if (text2 != null)
					{
						if (m_Sections.ContainsKey(text2))
						{
							dictionary = null;
							continue;
						}
						dictionary = new Dictionary<string, string>();
						m_Sections.Add(text2, dictionary);
					}
					else if (dictionary != null && ParseKeyValuePair(text, ref Key, ref Value) && !dictionary.ContainsKey(Key))
					{
						dictionary.Add(Key, Value);
					}
				}
			}
			finally
			{
				stringReader?.Close();
				stringReader = null;
			}
		}
	}

	private void PerformFlush()
	{
		if (!m_CacheModified)
		{
			return;
		}
		m_CacheModified = false;
		StringWriter stringWriter = new StringWriter();
		try
		{
			Dictionary<string, string> value = null;
			Dictionary<string, string> value2 = null;
			StringReader stringReader = null;
			try
			{
				stringReader = new StringReader(m_iniString);
				string Key = null;
				string Value = null;
				bool flag = true;
				bool flag2 = false;
				string Key2 = null;
				string Value2 = null;
				while (flag)
				{
					string text = stringReader.ReadLine();
					flag = text != null;
					bool flag3;
					string text2;
					if (flag)
					{
						flag3 = true;
						text = text.Trim();
						text2 = ParseSectionName(text);
					}
					else
					{
						flag3 = false;
						text2 = null;
					}
					if (text2 != null || !flag)
					{
						if (value != null && value.Count > 0)
						{
							StringBuilder stringBuilder = stringWriter.GetStringBuilder();
							while (stringBuilder[stringBuilder.Length - 1] == '\n' || stringBuilder[stringBuilder.Length - 1] == '\r')
							{
								stringBuilder.Length--;
							}
							stringWriter.WriteLine();
							foreach (string key in value.Keys)
							{
								if (value.TryGetValue(key, out Value))
								{
									stringWriter.Write(key);
									stringWriter.Write('=');
									stringWriter.WriteLine(Value);
								}
							}
							stringWriter.WriteLine();
							stringWriter.WriteLine();
							value.Clear();
						}
						if (flag && !m_Modified.TryGetValue(text2, out value))
						{
							value = null;
						}
					}
					else if (value != null && ParseKeyValuePair(text, ref Key, ref Value) && value.TryGetValue(Key, out Value))
					{
						flag3 = false;
						value.Remove(Key);
						stringWriter.Write(Key);
						stringWriter.Write('=');
						stringWriter.WriteLine(Value);
					}
					if (flag3)
					{
						if (text2 != null)
						{
							if (!m_Sections.ContainsKey(text2))
							{
								flag2 = true;
								value2 = null;
							}
							else
							{
								flag2 = false;
								m_Sections.TryGetValue(text2, out value2);
							}
						}
						else if (value2 != null && ParseKeyValuePair(text, ref Key2, ref Value2))
						{
							flag2 = !value2.ContainsKey(Key2);
						}
					}
					if (flag3 && !isComment(text) && !flag2)
					{
						stringWriter.WriteLine(text);
					}
				}
				stringReader.Close();
				stringReader = null;
			}
			finally
			{
				stringReader?.Close();
				stringReader = null;
			}
			foreach (KeyValuePair<string, Dictionary<string, string>> item in m_Modified)
			{
				value = item.Value;
				if (value.Count <= 0)
				{
					continue;
				}
				stringWriter.WriteLine();
				stringWriter.Write('[');
				stringWriter.Write(item.Key);
				stringWriter.WriteLine(']');
				foreach (KeyValuePair<string, string> item2 in value)
				{
					stringWriter.Write(item2.Key);
					stringWriter.Write('=');
					stringWriter.WriteLine(item2.Value);
				}
				value.Clear();
			}
			m_Modified.Clear();
			m_iniString = stringWriter.ToString();
			stringWriter.Close();
			stringWriter = null;
			if (m_FileName != null)
			{
				File.WriteAllText(m_FileName, m_iniString);
			}
		}
		finally
		{
			stringWriter?.Close();
			stringWriter = null;
		}
	}

	public bool IsSectionExists(string SectionName)
	{
		return m_Sections.ContainsKey(SectionName);
	}

	public bool IsKeyExists(string SectionName, string Key)
	{
		if (m_Sections.ContainsKey(SectionName))
		{
			m_Sections.TryGetValue(SectionName, out var value);
			return value.ContainsKey(Key);
		}
		return false;
	}

	public void SectionDelete(string SectionName)
	{
		if (!IsSectionExists(SectionName))
		{
			return;
		}
		lock (m_Lock)
		{
			m_CacheModified = true;
			m_Sections.Remove(SectionName);
			m_Modified.Remove(SectionName);
			if (m_AutoFlush)
			{
				PerformFlush();
			}
		}
	}

	public void KeyDelete(string SectionName, string Key)
	{
		if (!IsKeyExists(SectionName, Key))
		{
			return;
		}
		lock (m_Lock)
		{
			m_CacheModified = true;
			m_Sections.TryGetValue(SectionName, out var value);
			value.Remove(Key);
			if (m_Modified.TryGetValue(SectionName, out value))
			{
				value.Remove(SectionName);
			}
			if (m_AutoFlush)
			{
				PerformFlush();
			}
		}
	}

	public string ReadValue(string SectionName, string Key, string DefaultValue)
	{
		lock (m_Lock)
		{
			if (!m_Sections.TryGetValue(SectionName, out var value))
			{
				return DefaultValue;
			}
			if (!value.TryGetValue(Key, out var value2))
			{
				return DefaultValue;
			}
			return value2;
		}
	}

	public void WriteValue(string SectionName, string Key, string Value)
	{
		lock (m_Lock)
		{
			m_CacheModified = true;
			if (!m_Sections.TryGetValue(SectionName, out var value))
			{
				value = new Dictionary<string, string>();
				m_Sections.Add(SectionName, value);
			}
			if (value.ContainsKey(Key))
			{
				value.Remove(Key);
			}
			value.Add(Key, Value);
			if (!m_Modified.TryGetValue(SectionName, out value))
			{
				value = new Dictionary<string, string>();
				m_Modified.Add(SectionName, value);
			}
			if (value.ContainsKey(Key))
			{
				value.Remove(Key);
			}
			value.Add(Key, Value);
			if (m_AutoFlush)
			{
				PerformFlush();
			}
		}
	}

	private string EncodeByteArray(byte[] Value)
	{
		if (Value == null)
		{
			return null;
		}
		StringBuilder stringBuilder = new StringBuilder();
		for (int i = 0; i < Value.Length; i++)
		{
			string text = Convert.ToString(Value[i], 16);
			int length = text.Length;
			if (length > 2)
			{
				stringBuilder.Append(text.Substring(length - 2, 2));
				continue;
			}
			if (length < 2)
			{
				stringBuilder.Append("0");
			}
			stringBuilder.Append(text);
		}
		return stringBuilder.ToString();
	}

	private byte[] DecodeByteArray(string Value)
	{
		if (Value == null)
		{
			return null;
		}
		int length = Value.Length;
		if (length < 2)
		{
			return new byte[0];
		}
		length /= 2;
		byte[] array = new byte[length];
		for (int i = 0; i < length; i++)
		{
			array[i] = Convert.ToByte(Value.Substring(i * 2, 2), 16);
		}
		return array;
	}

	public bool ReadValue(string SectionName, string Key, bool DefaultValue)
	{
		if (int.TryParse(ReadValue(SectionName, Key, DefaultValue.ToString(CultureInfo.InvariantCulture)), out var result))
		{
			return result != 0;
		}
		return DefaultValue;
	}

	public int ReadValue(string SectionName, string Key, int DefaultValue)
	{
		if (int.TryParse(ReadValue(SectionName, Key, DefaultValue.ToString(CultureInfo.InvariantCulture)), NumberStyles.Any, CultureInfo.InvariantCulture, out var result))
		{
			return result;
		}
		return DefaultValue;
	}

	public long ReadValue(string SectionName, string Key, long DefaultValue)
	{
		if (long.TryParse(ReadValue(SectionName, Key, DefaultValue.ToString(CultureInfo.InvariantCulture)), NumberStyles.Any, CultureInfo.InvariantCulture, out var result))
		{
			return result;
		}
		return DefaultValue;
	}

	public double ReadValue(string SectionName, string Key, double DefaultValue)
	{
		if (double.TryParse(ReadValue(SectionName, Key, DefaultValue.ToString(CultureInfo.InvariantCulture)), NumberStyles.Any, CultureInfo.InvariantCulture, out var result))
		{
			return result;
		}
		return DefaultValue;
	}

	public float ReadValue(string SectionName, string Key, float DefaultValue)
	{
		if (float.TryParse(ReadValue(SectionName, Key, DefaultValue.ToString(CultureInfo.InvariantCulture)), NumberStyles.Any, CultureInfo.InvariantCulture, out var result))
		{
			return result;
		}
		return DefaultValue;
	}

	public byte[] ReadValue(string SectionName, string Key, byte[] DefaultValue)
	{
		string value = ReadValue(SectionName, Key, EncodeByteArray(DefaultValue));
		try
		{
			return DecodeByteArray(value);
		}
		catch (FormatException)
		{
			return DefaultValue;
		}
	}

	public DateTime ReadValue(string SectionName, string Key, DateTime DefaultValue)
	{
		if (DateTime.TryParse(ReadValue(SectionName, Key, DefaultValue.ToString(CultureInfo.InvariantCulture)), CultureInfo.InvariantCulture, DateTimeStyles.AllowWhiteSpaces | DateTimeStyles.NoCurrentDateDefault | DateTimeStyles.AssumeLocal, out var result))
		{
			return result;
		}
		return DefaultValue;
	}

	public void WriteValue(string SectionName, string Key, bool Value)
	{
		WriteValue(SectionName, Key, Value ? "1" : "0");
	}

	public void WriteValue(string SectionName, string Key, int Value)
	{
		WriteValue(SectionName, Key, Value.ToString(CultureInfo.InvariantCulture));
	}

	public void WriteValue(string SectionName, string Key, long Value)
	{
		WriteValue(SectionName, Key, Value.ToString(CultureInfo.InvariantCulture));
	}

	public void WriteValue(string SectionName, string Key, double Value)
	{
		WriteValue(SectionName, Key, Value.ToString(CultureInfo.InvariantCulture));
	}

	public void WriteValue(string SectionName, string Key, float Value)
	{
		WriteValue(SectionName, Key, Value.ToString(CultureInfo.InvariantCulture));
	}

	public void WriteValue(string SectionName, string Key, byte[] Value)
	{
		WriteValue(SectionName, Key, EncodeByteArray(Value));
	}

	public void WriteValue(string SectionName, string Key, DateTime Value)
	{
		WriteValue(SectionName, Key, Value.ToString(CultureInfo.InvariantCulture));
	}
}

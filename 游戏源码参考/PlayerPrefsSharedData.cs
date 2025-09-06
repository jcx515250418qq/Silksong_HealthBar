using System.Globalization;
using TeamCherry.SharedUtils;
using UnityEngine;

public class PlayerPrefsSharedData : Platform.ISharedData
{
	public bool IsEncrypted { get; private set; }

	public PlayerPrefsSharedData(bool isEncrypted)
	{
		IsEncrypted = isEncrypted;
	}

	private string ReadEncrypted(string key)
	{
		string @string = PlayerPrefs.GetString(Encryption.Encrypt(key), string.Empty);
		if (string.IsNullOrEmpty(@string))
		{
			return null;
		}
		return Encryption.Decrypt(@string);
	}

	private void WriteEncrypted(string key, string val)
	{
		string key2 = Encryption.Encrypt(key);
		string value = Encryption.Encrypt(val);
		PlayerPrefs.SetString(key2, value);
	}

	public bool HasKey(string key)
	{
		return PlayerPrefs.HasKey(key);
	}

	public void DeleteKey(string key)
	{
		PlayerPrefs.DeleteKey(key);
	}

	public void DeleteAll()
	{
		PlayerPrefs.DeleteAll();
	}

	public void ImportData(Platform.ISharedData otherData)
	{
		Debug.LogError("PlayerPrefsSharedData does not support ImportData");
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
		if (IsEncrypted)
		{
			string text = ReadEncrypted(key);
			if (text == null)
			{
				return def;
			}
			if (!int.TryParse(text, out var result))
			{
				return def;
			}
			return result;
		}
		return PlayerPrefs.GetInt(key, def);
	}

	public void SetInt(string key, int val)
	{
		if (IsEncrypted)
		{
			WriteEncrypted(key, val.ToString());
		}
		else
		{
			PlayerPrefs.SetInt(key, val);
		}
	}

	public float GetFloat(string key, float def)
	{
		if (IsEncrypted)
		{
			string text = ReadEncrypted(key);
			if (text == null)
			{
				return def;
			}
			if (!float.TryParse(text, NumberStyles.Float, CultureInfo.InvariantCulture, out var result))
			{
				return def;
			}
			return result;
		}
		return PlayerPrefs.GetFloat(key, def);
	}

	public void SetFloat(string key, float val)
	{
		if (IsEncrypted)
		{
			WriteEncrypted(key, val.ToString(CultureInfo.InvariantCulture));
		}
		else
		{
			PlayerPrefs.SetFloat(key, val);
		}
	}

	public string GetString(string key, string def)
	{
		if (IsEncrypted)
		{
			string text = ReadEncrypted(key);
			if (text == null)
			{
				return def;
			}
			return text;
		}
		return PlayerPrefs.GetString(key, def);
	}

	public void SetString(string key, string val)
	{
		if (IsEncrypted)
		{
			WriteEncrypted(key, val);
		}
		else
		{
			PlayerPrefs.SetString(key, val);
		}
	}

	public virtual void Save()
	{
		PlayerPrefs.Save();
	}
}

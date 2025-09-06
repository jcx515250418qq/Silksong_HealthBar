public class DummyPlayerPrefsSharedData : Platform.ISharedData
{
	public bool IsEncrypted { get; private set; }

	public DummyPlayerPrefsSharedData(bool isEncrypted)
	{
		IsEncrypted = isEncrypted;
	}

	private string ReadEncrypted(string key)
	{
		return null;
	}

	private void WriteEncrypted(string key, string val)
	{
	}

	public bool HasKey(string key)
	{
		return false;
	}

	public void DeleteKey(string key)
	{
	}

	public void DeleteAll()
	{
	}

	public void ImportData(Platform.ISharedData otherData)
	{
	}

	public bool GetBool(string key, bool def)
	{
		return false;
	}

	public void SetBool(string key, bool val)
	{
	}

	public int GetInt(string key, int def)
	{
		return 0;
	}

	public void SetInt(string key, int val)
	{
	}

	public float GetFloat(string key, float def)
	{
		return 0f;
	}

	public void SetFloat(string key, float val)
	{
	}

	public string GetString(string key, string def)
	{
		return string.Empty;
	}

	public void SetString(string key, string val)
	{
	}

	public void Save()
	{
	}
}

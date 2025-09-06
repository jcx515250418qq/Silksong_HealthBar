using System;

[Serializable]
public sealed class RestorePointFileWrapper
{
	public byte[] data;

	public string date;

	public string version;

	public int number;

	public string identifier;

	public RestorePointFileWrapper(byte[] data)
	{
		this.data = data;
	}

	public RestorePointFileWrapper(byte[] data, int number)
	{
		this.data = data;
		this.number = number;
	}

	public RestorePointFileWrapper(byte[] data, int number, string identifier)
	{
		this.data = data;
		this.number = number;
		this.identifier = identifier;
	}

	public RestorePointFileWrapper()
	{
	}

	public void SetDateString()
	{
		date = GetDateString();
	}

	public void SetVersion()
	{
		version = "1.0.28324";
	}

	private static string GetDateString()
	{
		return DateTime.Now.ToString("yyyy/MM/dd");
	}
}

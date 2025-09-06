using System;

[Serializable]
public class SecureplayerPrefsDemoClass
{
	public string playID { get; set; }

	public int type { get; set; }

	public bool incremental { get; set; }

	public SecureplayerPrefsDemoClass()
	{
		playID = "";
		type = 0;
		incremental = false;
	}
}

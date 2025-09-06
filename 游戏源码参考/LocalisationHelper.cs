using System.Collections.Generic;

public static class LocalisationHelper
{
	public enum FontSource
	{
		Trajan = 0,
		Perpetua = 1
	}

	private static Dictionary<FontSource, Dictionary<string, string>> substitutions = new Dictionary<FontSource, Dictionary<string, string>> { 
	{
		FontSource.Trajan,
		new Dictionary<string, string> { { "ß", "ss" } }
	} };

	public static string GetProcessed(this string text, FontSource fontSource)
	{
		if (substitutions.ContainsKey(fontSource))
		{
			string text2 = text;
			foreach (KeyValuePair<string, string> item in substitutions[fontSource])
			{
				text2 = text2.Replace(item.Key, item.Value);
			}
			if (text2 != text)
			{
				text = text2;
			}
		}
		return text;
	}
}

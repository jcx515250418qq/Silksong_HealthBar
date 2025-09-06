using System.IO;
using GlobalSettings;
using UnityEngine;

public static class DemoHelper
{
	private static bool _checkedExhibitionMode;

	private static bool _isExhibitionMode;

	private const string EXHIBITION_MODE_MARKER = "IsExhibitionMode";

	public static bool IsDemoMode => false;

	public static bool IsExhibitionMode
	{
		get
		{
			if (!IsDemoMode)
			{
				return false;
			}
			if (_checkedExhibitionMode)
			{
				return _isExhibitionMode;
			}
			_checkedExhibitionMode = true;
			_isExhibitionMode = File.Exists(Path.Combine(Application.dataPath, "IsExhibitionMode"));
			return _isExhibitionMode;
		}
	}

	public static bool TryGetSaveData(int index, out string jsonData)
	{
		TextAsset saveFile = Demo.GetSaveFileOverride(index).SaveFile;
		if (saveFile != null)
		{
			jsonData = saveFile.text;
			return true;
		}
		jsonData = null;
		return false;
	}

	public static bool HasSaveFile(int saveSlot)
	{
		if (saveSlot == 0)
		{
			return true;
		}
		string jsonData = null;
		return TryGetSaveData(saveSlot - 1, out jsonData);
	}

	public static bool IsDummySaveFile(int saveSlot)
	{
		return Demo.GetSaveFileOverride(saveSlot - 1).IsDummySave;
	}
}

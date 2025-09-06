using UnityEngine;

public class QuickCreateAssetAttribute : PropertyAttribute
{
	public string FolderPath { get; private set; }

	public string SourceField { get; private set; }

	public string TargetField { get; private set; }

	public QuickCreateAssetAttribute(string folderPath, string sourceField, string targetField)
	{
		FolderPath = folderPath;
		SourceField = sourceField;
		TargetField = targetField;
	}
}

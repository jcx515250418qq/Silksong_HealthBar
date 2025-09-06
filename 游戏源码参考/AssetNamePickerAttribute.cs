using UnityEngine;

public class AssetNamePickerAttribute : PropertyAttribute
{
	private readonly string searchFilter;

	public string SearchFilter => searchFilter;

	public AssetNamePickerAttribute(string searchFilter)
	{
		this.searchFilter = searchFilter;
	}
}

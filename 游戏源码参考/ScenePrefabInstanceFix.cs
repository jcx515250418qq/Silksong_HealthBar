using UnityEngine;

public static class ScenePrefabInstanceFix
{
	public interface ICheckFields
	{
		void OnPrefabInstanceFix();
	}

	public static void CheckField<T>(ref T obj) where T : Object
	{
	}
}

using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Hornet/Scene Object Pool")]
public class SceneObjectPool : ScriptableObject
{
	[SerializeField]
	private StartupPool[] gameObjectPool;

	[Space]
	[SerializeField]
	private Object[] holdReferences;

	public void SpawnPool(GameObject owner)
	{
		if (gameObjectPool != null && gameObjectPool.Length != 0)
		{
			owner.AddComponent<PersonalObjectPool>().startupPool = new List<StartupPool>(gameObjectPool);
		}
	}
}

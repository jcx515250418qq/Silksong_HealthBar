using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Profiles/Spike Slash Reaction")]
public sealed class SpikeSlashReactionProfile : ScriptableObject
{
	[SerializeField]
	private List<GameObject> spawnedEffects = new List<GameObject>();

	public void SpawnEffect(Vector3 position, Quaternion rotation)
	{
		spawnedEffects.RemoveAll((GameObject o) => o == null);
		foreach (GameObject spawnedEffect in spawnedEffects)
		{
			spawnedEffect.Spawn(position, rotation);
		}
	}
}

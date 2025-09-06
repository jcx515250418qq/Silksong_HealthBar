using UnityEngine;

public sealed class ExtinguishEffectSpawner : MonoBehaviour
{
	[SerializeField]
	private GameObject effectPrefab;

	public void PlayEffect()
	{
		PlayEffect(base.transform.position);
	}

	public void PlayEffect(Vector3 position)
	{
		if (!(effectPrefab == null))
		{
			effectPrefab.Spawn(position);
		}
	}
}

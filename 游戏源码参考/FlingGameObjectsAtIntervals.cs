using System.Collections;
using UnityEngine;

public class FlingGameObjectsAtIntervals : MonoBehaviour
{
	[SerializeField]
	private Vector2 spawnOffset;

	[SerializeField]
	private float minSpawnDelay = 1f;

	[SerializeField]
	private float maxSpawnDelay = 3f;

	[SerializeField]
	private FlingGameObject[] spawnObjects;

	private Coroutine spawnRoutine;

	private void OnEnable()
	{
		spawnRoutine = StartCoroutine(SpawnRepeating());
	}

	private void OnDisable()
	{
		if (spawnRoutine != null)
		{
			StopCoroutine(spawnRoutine);
		}
	}

	private void OnDrawGizmosSelected()
	{
		Gizmos.color = Color.red;
		Gizmos.matrix = base.transform.localToWorldMatrix;
		Gizmos.DrawWireSphere(spawnOffset, 0.2f);
	}

	private IEnumerator SpawnRepeating()
	{
		while (true)
		{
			yield return new WaitForSeconds(Random.Range(minSpawnDelay, maxSpawnDelay));
			if (spawnObjects.Length != 0)
			{
				spawnObjects[Random.Range(0, spawnObjects.Length)].Fling(base.transform.TransformPoint(spawnOffset), 1f);
			}
		}
	}
}

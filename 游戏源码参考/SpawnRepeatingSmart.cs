using System.Collections.Generic;
using GlobalEnums;
using UnityEngine;

public class SpawnRepeatingSmart : MonoBehaviour
{
	[SerializeField]
	private GameObject prefab;

	[SerializeField]
	private float minDistance;

	[SerializeField]
	private int maxInDistance;

	[SerializeField]
	private float spawnDelay;

	[SerializeField]
	private float spawnDistance;

	private readonly List<GameObject> spawnedObjects = new List<GameObject>();

	private float timer;

	private Vector2 lastSpawnPosition;

	private HeroController hc;

	private GameManager gm;

	private void OnEnable()
	{
		timer = 0f;
		hc = GetComponentInParent<HeroController>();
		gm = GameManager.instance;
	}

	private void OnDisable()
	{
		hc = null;
		gm = null;
	}

	private void Update()
	{
		if ((bool)gm && gm.GameState == GameState.LOADING)
		{
			for (int num = spawnedObjects.Count - 1; num >= 0; num--)
			{
				spawnedObjects[num].Recycle();
			}
		}
		else
		{
			if ((bool)hc && !hc.isHeroInPosition)
			{
				return;
			}
			if (spawnDelay > 0f)
			{
				timer += Time.deltaTime;
				if (timer >= spawnDelay)
				{
					timer = 0f;
					TrySpawn();
				}
			}
			if (spawnDistance > 0f && Vector2.Distance(lastSpawnPosition, base.transform.position) >= spawnDistance)
			{
				TrySpawn();
			}
		}
	}

	private void TrySpawn()
	{
		if (maxInDistance <= 0 || minDistance <= 0f)
		{
			DoSpawn();
			return;
		}
		int num = 0;
		Vector3 position = base.transform.position;
		foreach (GameObject spawnedObject in spawnedObjects)
		{
			if (!(spawnedObject == null) && Vector2.Distance(position, spawnedObject.transform.position) <= minDistance)
			{
				num++;
			}
		}
		if (num < maxInDistance)
		{
			DoSpawn();
		}
	}

	private void DoSpawn()
	{
		Vector3 position = base.transform.position;
		lastSpawnPosition = position;
		GameObject gameObject = prefab.Spawn(position);
		spawnedObjects.Add(gameObject);
		RecycleResetHandler.Add(gameObject, delegate(GameObject o)
		{
			spawnedObjects.Remove(o);
		});
	}
}

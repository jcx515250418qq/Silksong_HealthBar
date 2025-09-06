using System.Collections.Generic;
using UnityEngine;

public class PersistentFolderReset : MonoBehaviour
{
	[SerializeField]
	private GameObject sourceFolder;

	[SerializeField]
	private bool spawnFromPersonalPool;

	[SerializeField]
	private int initialPoolSize = 3;

	[SerializeField]
	private bool skipOnPlayerDeath;

	private GameManager gm;

	private GameObject current;

	private Transform spawnedFolder;

	private PersonalObjectPool personalObjectPool;

	private List<GameObject> sourceList = new List<GameObject>();

	private List<GameObject> spawnedList = new List<GameObject>();

	private void Awake()
	{
		sourceFolder.SetActive(value: false);
		gm = GameManager.instance;
		gm.ResetSemiPersistentObjects += ResetSemiPersistentObjects;
		if (spawnFromPersonalPool)
		{
			spawnFromPersonalPool = base.gameObject.AddComponentIfNotPresent<PersonalObjectPool>();
			spawnedFolder = new GameObject($"{this} Spawn Folder").transform;
			spawnedFolder.SetParent(sourceFolder.transform.parent, worldPositionStays: false);
			spawnedFolder.localPosition = sourceFolder.transform.localPosition;
			foreach (Transform item in sourceFolder.transform)
			{
				GameObject gameObject = item.gameObject;
				if (gameObject.activeSelf)
				{
					sourceList.Add(gameObject);
					PersonalObjectPool.EnsurePooledInScene(base.gameObject, gameObject, 3, finished: false, initialiseSpawned: true);
				}
			}
			PersonalObjectPool.EnsurePooledInSceneFinished(base.gameObject);
		}
		SpawnNew();
	}

	private void OnDestroy()
	{
		if ((bool)gm)
		{
			gm.ResetSemiPersistentObjects -= ResetSemiPersistentObjects;
		}
	}

	private void ResetSemiPersistentObjects()
	{
		if (skipOnPlayerDeath)
		{
			HeroController instance = HeroController.instance;
			if ((bool)instance && instance.cState.dead)
			{
				return;
			}
		}
		SpawnNew();
	}

	private void SpawnNew()
	{
		if (spawnFromPersonalPool)
		{
			foreach (GameObject spawned in spawnedList)
			{
				if (spawned != null && spawned.activeSelf)
				{
					spawned.Recycle();
				}
			}
			foreach (Transform item in spawnedFolder)
			{
				item.gameObject.Recycle();
			}
			spawnedList.Clear();
			{
				foreach (GameObject source in sourceList)
				{
					Transform transform = source.transform;
					spawnedList.Add(source.Spawn(spawnedFolder, transform.localPosition, transform.localRotation));
				}
				return;
			}
		}
		if ((bool)current)
		{
			Object.Destroy(current);
		}
		current = Object.Instantiate(sourceFolder, sourceFolder.transform.parent);
		current.SetActive(value: true);
	}
}

using UnityEngine;

[CreateAssetMenu(menuName = "Hornet/Collectable Items/Collectable Item (Prefab)")]
public class PrefabCollectable : FakeCollectable, ISavedItemPreSpawn, IPreSpawn
{
	[Space]
	[SerializeField]
	private GameObject prefab;

	[SerializeField]
	private bool spawnPrefab;

	[SerializeField]
	private bool canPreSpawn = true;

	public override bool CanGetMultipleAtOnce => false;

	public override bool GetTakesHeroControl()
	{
		return !prefab.GetComponent<SilkRationObject>();
	}

	public override void Get(bool showPopup = true)
	{
		base.Get(showPopup);
		SilkRationObject component = prefab.GetComponent<SilkRationObject>();
		if ((bool)component)
		{
			component.AddSilk();
			if (showPopup)
			{
				component.CollectPopup();
			}
		}
		else
		{
			Spawn();
		}
	}

	public GameObject Spawn()
	{
		if (!prefab)
		{
			return null;
		}
		if (spawnPrefab)
		{
			return prefab.Spawn();
		}
		return Object.Instantiate(prefab);
	}

	public void PreSpawnGet(bool showPopup = true)
	{
		base.Get(showPopup);
	}

	public bool TryGetPrespawnedItem(out PreSpawnedItem item)
	{
		if (spawnPrefab || prefab == null || !canPreSpawn)
		{
			item = null;
			return false;
		}
		if ((bool)prefab.GetComponent<SilkRationObject>())
		{
			item = null;
			return false;
		}
		GameObject gameObject = Object.Instantiate(prefab);
		item = new PreSpawnedItem(gameObject, recycle: false);
		gameObject.gameObject.SetActive(value: false);
		return true;
	}
}

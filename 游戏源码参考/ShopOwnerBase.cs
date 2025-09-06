using TeamCherry.Localization;
using UnityEngine;

public abstract class ShopOwnerBase : MonoBehaviour
{
	[SerializeField]
	private ShopMenuStock shopPrefab;

	[SerializeField]
	private LocalisedString shopTitle;

	private static ShopMenuStock _spawnedShop;

	public abstract ShopItem[] Stock { get; }

	public GameObject ShopObject
	{
		get
		{
			SpawnUpdateShop();
			return _spawnedShop.gameObject;
		}
	}

	private void OnEnable()
	{
		SpawnUpdateShop();
	}

	private void OnDestroy()
	{
		if (_spawnedShop != null)
		{
			Object.Destroy(_spawnedShop.gameObject);
		}
		_spawnedShop = null;
	}

	private void SpawnUpdateShop()
	{
		if (!_spawnedShop && (bool)shopPrefab)
		{
			_spawnedShop = Object.Instantiate(shopPrefab);
			ListenToShopDestroyed(_spawnedShop);
		}
		if ((bool)_spawnedShop)
		{
			_spawnedShop.SetStock(Stock);
			_spawnedShop.Title = shopTitle;
		}
	}

	private void ListenToShopDestroyed(ShopMenuStock shop)
	{
		shop.gameObject.AddComponent<OnDestroyEventAnnouncer>().OnDestroyEvent += OnSpawnedShopDestroyed;
	}

	private void OnSpawnedShopDestroyed(OnDestroyEventAnnouncer announcer)
	{
		announcer.OnDestroyEvent -= OnSpawnedShopDestroyed;
		_spawnedShop = null;
	}
}

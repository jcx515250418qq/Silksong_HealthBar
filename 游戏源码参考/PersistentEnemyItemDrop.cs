using UnityEngine;

public class PersistentEnemyItemDrop : MonoBehaviour
{
	[SerializeField]
	private CollectableItemPickup dropPrefab;

	[SerializeField]
	private SavedItem item;

	[SerializeField]
	private bool onlyOnStartedDead;

	[SerializeField]
	private Transform startedDeadSpawnPoint;

	private HealthManager healthManager;

	private PersistentItemData<bool> enemyItemData;

	private string droppedItemID;

	private void Awake()
	{
		if (!dropPrefab || !item || !item.CanGetMore())
		{
			return;
		}
		healthManager = GetComponent<HealthManager>();
		if (!healthManager)
		{
			return;
		}
		enemyItemData = new PersistentItemData<bool>
		{
			ID = base.gameObject.name,
			SceneName = base.gameObject.scene.name
		};
		droppedItemID = $"{enemyItemData.ID}_item_{item.name}";
		healthManager.StartedDead += delegate
		{
			if (!SceneData.instance.PersistentBools.GetValueOrDefault(enemyItemData.SceneName, droppedItemID))
			{
				DropItem(fling: false);
			}
		};
		if (!onlyOnStartedDead)
		{
			healthManager.OnDeath += delegate
			{
				DropItem(fling: true);
			};
		}
	}

	private void DropItem(bool fling)
	{
		CollectableItemPickup collectableItemPickup = Object.Instantiate(dropPrefab);
		if (!fling && (bool)startedDeadSpawnPoint)
		{
			collectableItemPickup.transform.SetPosition2D(startedDeadSpawnPoint.position);
		}
		else
		{
			collectableItemPickup.transform.SetPosition2D(base.transform.TransformPoint(healthManager.EffectOrigin));
		}
		collectableItemPickup.SetItem(item);
		collectableItemPickup.OnPickup.AddListener(delegate
		{
			SceneData.instance.PersistentBools.SetValue(new PersistentItemData<bool>
			{
				ID = droppedItemID,
				SceneName = enemyItemData.SceneName,
				IsSemiPersistent = enemyItemData.IsSemiPersistent,
				Value = true
			});
		});
		if (fling)
		{
			float angleMin = (healthManager.MegaFlingGeo ? 65 : 80);
			float angleMax = (healthManager.MegaFlingGeo ? 115 : 100);
			float speedMin = (healthManager.MegaFlingGeo ? 30 : 15);
			float speedMax = (healthManager.MegaFlingGeo ? 45 : 30);
			FlingUtils.SelfConfig config = default(FlingUtils.SelfConfig);
			config.Object = collectableItemPickup.gameObject;
			config.SpeedMin = speedMin;
			config.SpeedMax = speedMax;
			config.AngleMin = angleMin;
			config.AngleMax = angleMax;
			FlingUtils.FlingObject(config, base.transform, healthManager.EffectOrigin);
		}
	}
}

using TeamCherry.SharedUtils;
using UnityEngine;

public class MemoryOrbGroup : MonoBehaviour
{
	[SerializeField]
	[PlayerDataField(typeof(ulong), false)]
	private string pdBitmask;

	[Space]
	[SerializeField]
	private PersistentBoolItem readActivated;

	[SerializeField]
	[PlayerDataField(typeof(bool), false)]
	private string readPdBool;

	[Space]
	[SerializeField]
	private PlayMakerFSM eventFsm;

	[SerializeField]
	private Transform orbsParent;

	[SerializeField]
	private Transform orbReturnTarget;

	[SerializeField]
	private GameObject activeWhileUncollected;

	[Space]
	[SerializeField]
	private GameObject finishSingleScreenEdgePrefab;

	[SerializeField]
	private GameObject finishTargetScreenEdgePrefab;

	[SerializeField]
	private GameObject finishAllScreenEdgePrefab;

	[SerializeField]
	private float screenEdgePadding;

	public string PdBitmask => pdBitmask;

	public bool IsAllCollected
	{
		get
		{
			if (string.IsNullOrWhiteSpace(pdBitmask))
			{
				return true;
			}
			ulong variable = PlayerData.instance.GetVariable<ulong>(pdBitmask);
			for (int i = 0; i < orbsParent.childCount; i++)
			{
				if (!variable.IsBitSet(i))
				{
					return false;
				}
			}
			return true;
		}
	}

	private void Awake()
	{
		if ((bool)readActivated)
		{
			readActivated.OnSetSaveState += OnReadActivated;
		}
		if ((bool)finishSingleScreenEdgePrefab)
		{
			PersonalObjectPool.EnsurePooledInScene(base.gameObject, finishSingleScreenEdgePrefab, 5, finished: false, initialiseSpawned: false, shared: true);
		}
		if ((bool)finishTargetScreenEdgePrefab)
		{
			PersonalObjectPool.EnsurePooledInScene(base.gameObject, finishTargetScreenEdgePrefab, 1, finished: false, initialiseSpawned: false, shared: true);
		}
		if ((bool)finishAllScreenEdgePrefab)
		{
			PersonalObjectPool.EnsurePooledInScene(base.gameObject, finishAllScreenEdgePrefab, 1, finished: false, initialiseSpawned: false, shared: true);
		}
		PersonalObjectPool.EnsurePooledInSceneFinished(base.gameObject);
		bool instantOrb = !string.IsNullOrWhiteSpace(pdBitmask);
		for (int i = 0; i < orbsParent.childCount; i++)
		{
			Transform child = orbsParent.GetChild(i);
			MemoryOrb component = child.gameObject.GetComponent<MemoryOrb>();
			component.Setup(this, i);
			component.InstantOrb = instantOrb;
			child.gameObject.SetActive(value: false);
		}
		if ((bool)activeWhileUncollected)
		{
			activeWhileUncollected.SetActive(value: false);
		}
	}

	private void Start()
	{
		if (!string.IsNullOrEmpty(readPdBool))
		{
			OnReadActivated(PlayerData.instance.GetBool(readPdBool));
		}
	}

	private void OnDestroy()
	{
		if ((bool)readActivated)
		{
			readActivated.OnSetSaveState -= OnReadActivated;
		}
	}

	private void OnReadActivated(bool value)
	{
		if (value)
		{
			Reappear();
		}
	}

	public void Appear()
	{
		if (string.IsNullOrWhiteSpace(pdBitmask))
		{
			return;
		}
		ulong variable = PlayerData.instance.GetVariable<ulong>(pdBitmask);
		bool active = false;
		for (int i = 0; i < orbsParent.childCount; i++)
		{
			if (!variable.IsBitSet(i))
			{
				orbsParent.GetChild(i).gameObject.SetActive(value: true);
				active = true;
			}
		}
		if ((bool)activeWhileUncollected)
		{
			activeWhileUncollected.SetActive(active);
		}
	}

	public void Reappear()
	{
		Appear();
	}

	public void CollectedOrb(int index)
	{
		if (string.IsNullOrWhiteSpace(pdBitmask))
		{
			return;
		}
		PlayerData instance = PlayerData.instance;
		ulong variable = instance.GetVariable<ulong>(pdBitmask);
		variable = variable.SetBitAtIndex(index);
		instance.SetVariable(pdBitmask, variable);
		bool active = false;
		for (int i = 0; i < orbsParent.childCount; i++)
		{
			if (!variable.IsBitSet(i))
			{
				active = true;
			}
		}
		if ((bool)activeWhileUncollected)
		{
			activeWhileUncollected.SetActive(active);
		}
	}

	public void LargeOrbReturned()
	{
		SpawnScreenEdgeEffect(finishSingleScreenEdgePrefab);
	}

	public void OrbReturned(int total)
	{
		switch (total)
		{
		case 12:
			eventFsm.SendEvent("LAST ORB COLLECTED");
			SpawnScreenEdgeEffect(finishTargetScreenEdgePrefab);
			break;
		case 17:
			SpawnScreenEdgeEffect(finishAllScreenEdgePrefab);
			break;
		}
	}

	private void SpawnScreenEdgeEffect(GameObject prefab)
	{
		if ((bool)prefab)
		{
			Transform transform = GameCameras.instance.mainCamera.transform;
			Vector2 vector = transform.position;
			Vector2 vector2 = orbReturnTarget.position;
			Vector2 vector3 = new Vector2(8.3f * ForceCameraAspect.CurrentViewportAspect, 8.3f);
			Vector2 vector4 = vector - vector3;
			Vector2 vector5 = vector + vector3;
			Vector2 normalized = (vector2 - vector).normalized;
			Vector2 original = vector + normalized * vector3.x;
			if (original.x < vector4.x)
			{
				original.x = vector4.x;
			}
			else if (original.x > vector5.x)
			{
				original.x = vector5.x;
			}
			if (original.y < vector4.y)
			{
				original.y = vector4.y;
			}
			else if (original.y > vector5.y)
			{
				original.y = vector5.y;
			}
			original += normalized * screenEdgePadding;
			prefab.Spawn(original.ToVector3(prefab.transform.localScale.z), Quaternion.Euler(0f, 0f, normalized.DirectionToAngle())).transform.SetParent(transform);
		}
	}

	public void TestSingle()
	{
		SpawnScreenEdgeEffect(finishSingleScreenEdgePrefab);
	}

	public void TestTarget()
	{
		SpawnScreenEdgeEffect(finishTargetScreenEdgePrefab);
	}

	public void TestAll()
	{
		SpawnScreenEdgeEffect(finishAllScreenEdgePrefab);
	}
}

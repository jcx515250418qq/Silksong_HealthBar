using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

public class TrackTriggerObjects : DebugDrawColliderRuntimeAdder
{
	[SerializeField]
	private LayerMask ignoreLayers;

	[SerializeField]
	[TagSelector]
	[FormerlySerializedAs("tagFilter")]
	private List<string> tagIncludeList;

	[SerializeField]
	[TagSelector]
	private List<string> tagExcludeList;

	private List<GameObject> insideGameObjects = new List<GameObject>();

	private int layerMask = -1;

	private bool gottenOverlappedColliders;

	private static readonly Collider2D[] _tempResults = new Collider2D[10];

	private static readonly List<GameObject> _refreshTemp = new List<GameObject>();

	public int InsideCount
	{
		get
		{
			int num = 0;
			foreach (GameObject insideGameObject in insideGameObjects)
			{
				if ((bool)insideGameObject && IsCounted(insideGameObject))
				{
					num++;
				}
			}
			return num;
		}
	}

	public bool IsInside => InsideCount > 0;

	public IEnumerable<GameObject> InsideGameObjects
	{
		get
		{
			if (!gottenOverlappedColliders)
			{
				GetOverlappedColliders(isRefresh: false);
			}
			return insideGameObjects;
		}
	}

	public List<GameObject> insideObjectsList => insideGameObjects;

	protected virtual bool RequireEnabled => false;

	public event Action<bool> InsideStateChanged;

	protected virtual void OnEnable()
	{
		if (layerMask < 0)
		{
			layerMask = Helper.GetCollidingLayerMaskForLayer(base.gameObject.layer);
		}
		HeroController silentInstance = HeroController.SilentInstance;
		if ((bool)silentInstance)
		{
			silentInstance.heroInPosition += OnHeroInPosition;
			if (silentInstance.isHeroInPosition)
			{
				GetOverlappedColliders(isRefresh: false);
			}
		}
	}

	protected virtual void OnDisable()
	{
		HeroController silentInstance = HeroController.SilentInstance;
		if ((bool)silentInstance)
		{
			silentInstance.heroInPosition -= OnHeroInPosition;
		}
		foreach (GameObject insideGameObject in insideGameObjects)
		{
			ExitNotify(insideGameObject);
		}
		insideGameObjects.Clear();
		CallInsideStateChanged(isInside: false);
		gottenOverlappedColliders = false;
	}

	private void OnHeroInPosition(bool forceDirect)
	{
		if (!this)
		{
			Debug.LogError("TrackTriggerObjects native Object was destroyed! This should not happen...", this);
		}
		else
		{
			GetOverlappedColliders(isRefresh: true);
		}
	}

	protected void Refresh()
	{
		GetOverlappedColliders(isRefresh: true);
	}

	private void GetOverlappedColliders(bool isRefresh)
	{
		if (!base.enabled || !base.gameObject.activeInHierarchy || (gottenOverlappedColliders && !isRefresh))
		{
			return;
		}
		gottenOverlappedColliders = true;
		Collider2D[] components = GetComponents<Collider2D>();
		for (int i = 0; i < components.Length; i++)
		{
			int num = components[i].Overlap(new ContactFilter2D
			{
				useTriggers = true,
				useLayerMask = true,
				layerMask = layerMask
			}, _tempResults);
			if (num > 0)
			{
				for (int j = 0; j < Mathf.Min(num, _tempResults.Length); j++)
				{
					OnTriggerEnter2D(_tempResults[j]);
				}
			}
		}
		if (isRefresh)
		{
			_refreshTemp.AddRange(insideGameObjects);
			foreach (GameObject item in _refreshTemp)
			{
				bool flag = false;
				components = _tempResults;
				foreach (Collider2D collider2D in components)
				{
					if ((bool)collider2D && collider2D.gameObject == item)
					{
						flag = true;
						break;
					}
				}
				if (!flag)
				{
					OnExit(item);
				}
			}
			_refreshTemp.Clear();
		}
		for (int k = 0; k < _tempResults.Length; k++)
		{
			_tempResults[k] = null;
		}
	}

	private void OnTriggerEnter2D(Collider2D collision)
	{
		if (!gottenOverlappedColliders || (RequireEnabled && !base.isActiveAndEnabled))
		{
			return;
		}
		GameObject gameObject = collision.gameObject;
		if (IsIgnored(gameObject))
		{
			return;
		}
		List<string> list = tagIncludeList;
		if (list != null && list.Count > 0)
		{
			bool flag = false;
			foreach (string tagInclude in tagIncludeList)
			{
				if (gameObject.CompareTag(tagInclude))
				{
					flag = true;
					break;
				}
			}
			if (!flag)
			{
				return;
			}
		}
		list = tagExcludeList;
		if (list != null && list.Count > 0)
		{
			foreach (string tagExclude in tagExcludeList)
			{
				if (gameObject.CompareTag(tagExclude))
				{
					return;
				}
			}
		}
		HeroController component = collision.GetComponent<HeroController>();
		if ((!component || !component.cState.isTriggerEventsPaused) && !insideGameObjects.Contains(gameObject))
		{
			insideGameObjects.Add(gameObject);
			gameObject.GetComponent<ITrackTriggerObject>()?.OnTrackTriggerEntered(this);
			if (insideGameObjects.Count == 1)
			{
				CallInsideStateChanged(isInside: true);
			}
		}
	}

	private void OnTriggerExit2D(Collider2D collision)
	{
		HeroController component = collision.GetComponent<HeroController>();
		if (!component || !component.cState.isTriggerEventsPaused)
		{
			GameObject obj = collision.gameObject;
			OnExit(obj);
		}
	}

	private void OnExit(GameObject obj)
	{
		if (insideGameObjects.Remove(obj))
		{
			ExitNotify(obj);
			if (insideGameObjects.Count == 0)
			{
				CallInsideStateChanged(isInside: false);
			}
		}
	}

	private void ExitNotify(GameObject obj)
	{
		if ((bool)obj)
		{
			obj.GetComponent<ITrackTriggerObject>()?.OnTrackTriggerExited(this);
		}
	}

	private void CallInsideStateChanged(bool isInside)
	{
		this.InsideStateChanged?.Invoke(isInside);
		OnInsideStateChanged(isInside);
	}

	private bool IsIgnored(GameObject obj)
	{
		int layer = obj.layer;
		int num = 1 << layer;
		return (ignoreLayers.value & num) == num;
	}

	protected virtual void OnInsideStateChanged(bool isInside)
	{
	}

	public GameObject GetClosestInside(Vector2 toPos, List<GameObject> excludeObjects)
	{
		float num = float.MaxValue;
		GameObject result = null;
		foreach (GameObject insideGameObject in InsideGameObjects)
		{
			if (!excludeObjects.Contains(insideGameObject))
			{
				float sqrMagnitude = ((Vector2)insideGameObject.transform.position - toPos).sqrMagnitude;
				if (!(sqrMagnitude > num))
				{
					num = sqrMagnitude;
					result = insideGameObject;
				}
			}
		}
		return result;
	}

	public GameObject GetClosestInsideLineOfSight(Vector2 originPos, HashSet<GameObject> excludeObjects)
	{
		return GetClosestInsideLineOfSight(originPos, excludeObjects, PhysicsConstants.ALL_TERRAIN_LAYER);
	}

	public GameObject GetClosestInsideLineOfSight(Vector2 originPos, HashSet<GameObject> excludeObjects, int obstacleLayerMask)
	{
		float num = float.MaxValue;
		GameObject result = null;
		foreach (GameObject insideGameObject in InsideGameObjects)
		{
			if (excludeObjects.Contains(insideGameObject))
			{
				continue;
			}
			Vector2 vector = (Vector2)insideGameObject.transform.position - originPos;
			float sqrMagnitude = vector.sqrMagnitude;
			if (!(sqrMagnitude >= num))
			{
				float distance = Mathf.Sqrt(sqrMagnitude);
				RaycastHit2D raycastHit2D = Physics2D.Raycast(originPos, vector.normalized, distance, obstacleLayerMask);
				if (!(raycastHit2D.collider != null) || !(raycastHit2D.collider.gameObject != insideGameObject))
				{
					num = sqrMagnitude;
					result = insideGameObject;
				}
			}
		}
		return result;
	}

	public override void AddDebugDrawComponent()
	{
		DebugDrawColliderRuntime.AddOrUpdate(base.gameObject, DebugDrawColliderRuntime.ColorType.Region);
	}

	protected virtual bool IsCounted(GameObject obj)
	{
		return true;
	}
}

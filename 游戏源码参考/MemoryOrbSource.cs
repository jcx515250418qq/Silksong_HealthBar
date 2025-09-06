using System.Collections.Generic;
using UnityEngine;

public class MemoryOrbSource : MonoBehaviour
{
	[SerializeField]
	private GameObject needolinScreenEdgePrefab;

	[SerializeField]
	private GameObject needolinOnScreenPrefab;

	[SerializeField]
	private float screenEdgePadding;

	private GameObject spawnedEffect;

	private List<ParticleSystem> temp;

	protected virtual bool IsActive => true;

	private void Awake()
	{
		if ((bool)needolinScreenEdgePrefab)
		{
			PersonalObjectPool.EnsurePooledInScene(base.gameObject, needolinScreenEdgePrefab, 2, finished: false);
		}
		if ((bool)needolinOnScreenPrefab)
		{
			PersonalObjectPool.EnsurePooledInScene(base.gameObject, needolinOnScreenPrefab, 2, finished: false);
		}
		PersonalObjectPool.EnsurePooledInSceneFinished(base.gameObject);
	}

	private void OnEnable()
	{
		HeroPerformanceRegion.StartedPerforming += OnNeedolinStart;
		HeroPerformanceRegion.StoppedPerforming += OnNeedolinStop;
	}

	private void OnDisable()
	{
		OnNeedolinStop();
		HeroPerformanceRegion.StartedPerforming -= OnNeedolinStart;
		HeroPerformanceRegion.StoppedPerforming -= OnNeedolinStop;
	}

	private void OnNeedolinStart()
	{
		if (!spawnedEffect && !NeedolinMsgBox.IsBlocked && IsActive)
		{
			spawnedEffect = SpawnScreenEdgeEffect(needolinScreenEdgePrefab, needolinOnScreenPrefab, base.transform.position, screenEdgePadding);
		}
	}

	private void OnNeedolinStop()
	{
		if (!spawnedEffect)
		{
			return;
		}
		if (temp == null)
		{
			temp = new List<ParticleSystem>();
		}
		spawnedEffect.GetComponentsInChildren(temp);
		foreach (ParticleSystem item in temp)
		{
			item.Stop(withChildren: true);
		}
		if (temp.Count == 0)
		{
			spawnedEffect.Recycle();
		}
		temp.Clear();
		spawnedEffect = null;
	}

	public static GameObject SpawnScreenEdgeEffect(GameObject edgePrefab, GameObject onScreenPrefab, Vector2 orbPos, float screenEdgePadding)
	{
		Transform transform = GameCameras.instance.mainCamera.transform;
		Vector2 vector = transform.position;
		Vector2 vector2 = new Vector2(8.3f * ForceCameraAspect.CurrentViewportAspect, 8.3f);
		Vector2 vector3 = vector - vector2;
		Vector2 vector4 = vector + vector2;
		Vector2 normalized = (orbPos - vector).normalized;
		if (orbPos.x >= vector3.x && orbPos.x <= vector4.x && orbPos.y >= vector3.y && orbPos.y <= vector4.y)
		{
			if (!onScreenPrefab)
			{
				return null;
			}
			return onScreenPrefab.Spawn(orbPos.ToVector3(onScreenPrefab.transform.localScale.z), Quaternion.Euler(0f, 0f, normalized.DirectionToAngle()));
		}
		Vector2 original = vector + normalized * vector2.x;
		if (original.x < vector3.x)
		{
			original.x = vector3.x;
		}
		else if (original.x > vector4.x)
		{
			original.x = vector4.x;
		}
		if (original.y < vector3.y)
		{
			original.y = vector3.y;
		}
		else if (original.y > vector4.y)
		{
			original.y = vector4.y;
		}
		original += normalized * screenEdgePadding;
		if (!edgePrefab)
		{
			return null;
		}
		GameObject obj = edgePrefab.Spawn(original.ToVector3(edgePrefab.transform.localScale.z), Quaternion.Euler(0f, 0f, normalized.DirectionToAngle()));
		obj.transform.SetParent(transform);
		return obj;
	}
}

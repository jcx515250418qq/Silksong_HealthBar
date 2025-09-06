using System.Collections.Generic;
using UnityEngine;

public static class HitTaker
{
	private const int DefaultRecursionDepth = 3;

	private static readonly List<IHitResponder> _tempHitResponders = new List<IHitResponder>();

	public static void Hit(GameObject targetGameObject, HitInstance damageInstance, int recursionDepth = 3)
	{
		Hit(targetGameObject, damageInstance, recursionDepth, null);
	}

	public static void Hit(GameObject targetGameObject, HitInstance damageInstance, HashSet<IHitResponder> blackList)
	{
		Hit(targetGameObject, damageInstance, 3, blackList);
	}

	public static void Hit(GameObject targetGameObject, HitInstance damageInstance, int recursionDepth, HashSet<IHitResponder> blackList)
	{
		foreach (IHitResponder hitResponder in GetHitResponders(targetGameObject, recursionDepth, blackList))
		{
			hitResponder.Hit(damageInstance);
		}
	}

	public static List<IHitResponder> GetHitResponders(GameObject targetGameObject, HashSet<IHitResponder> blackList)
	{
		return GetHitResponders(targetGameObject, 3, blackList);
	}

	public static List<IHitResponder> GetHitResponders(GameObject targetGameObject, int recursionDepth, HashSet<IHitResponder> blackList)
	{
		List<IHitResponder> list = new List<IHitResponder>();
		GetHitResponders(list, targetGameObject, recursionDepth, blackList);
		return list;
	}

	public static void GetHitResponders(List<IHitResponder> storeList, GameObject targetGameObject, HashSet<IHitResponder> blackList)
	{
		GetHitResponders(storeList, targetGameObject, 3, blackList);
	}

	public static void GetHitResponders(List<IHitResponder> storeList, GameObject targetGameObject, int recursionDepth, HashSet<IHitResponder> blackList)
	{
		if (targetGameObject == null)
		{
			return;
		}
		Transform transform = targetGameObject.transform;
		bool flag = blackList != null;
		try
		{
			for (int i = 0; i < recursionDepth; i++)
			{
				_tempHitResponders.Clear();
				transform.GetComponents(_tempHitResponders);
				bool flag2 = false;
				foreach (IHitResponder tempHitResponder in _tempHitResponders)
				{
					if (!flag)
					{
						storeList.Add(tempHitResponder);
					}
					else if (!blackList.Contains(tempHitResponder))
					{
						storeList.Add(tempHitResponder);
					}
					if (!tempHitResponder.HitRecurseUpwards)
					{
						flag2 = true;
					}
				}
				if (flag2 || (bool)transform.GetComponent<Rigidbody2D>())
				{
					break;
				}
				transform = transform.parent;
				if (transform == null)
				{
					break;
				}
			}
		}
		finally
		{
			_tempHitResponders.Clear();
		}
	}

	public static bool TryGetHealthManager(GameObject targetGameObject, out HealthManager healthManager)
	{
		return TryGetHealthManager(targetGameObject, 3, out healthManager);
	}

	public static bool TryGetHealthManager(GameObject targetGameObject, int recursionDepth, out HealthManager healthManager)
	{
		if (targetGameObject == null)
		{
			healthManager = null;
			return false;
		}
		Transform transform = targetGameObject.transform;
		try
		{
			for (int i = 0; i < recursionDepth; i++)
			{
				_tempHitResponders.Clear();
				transform.GetComponents(_tempHitResponders);
				bool flag = false;
				foreach (IHitResponder tempHitResponder in _tempHitResponders)
				{
					if (tempHitResponder is HealthManager healthManager2)
					{
						healthManager = healthManager2;
						return true;
					}
					if (!tempHitResponder.HitRecurseUpwards)
					{
						flag = true;
					}
				}
				if (flag || (bool)transform.GetComponent<Rigidbody2D>())
				{
					break;
				}
				transform = transform.parent;
				if (transform == null)
				{
					break;
				}
			}
		}
		finally
		{
			_tempHitResponders.Clear();
		}
		healthManager = null;
		return false;
	}
}

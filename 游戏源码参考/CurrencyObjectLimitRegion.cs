using System.Collections.Generic;
using GlobalSettings;
using UnityEngine;

public class CurrencyObjectLimitRegion : MonoBehaviour, ICurrencyLimitRegion
{
	[SerializeField]
	private Vector2 min;

	[SerializeField]
	private Vector2 max;

	[SerializeField]
	private CurrencyType currencyType;

	[SerializeField]
	private int limit;

	private static readonly HashSet<ICurrencyLimitRegion> _activeRegions = new HashSet<ICurrencyLimitRegion>();

	public CurrencyType CurrencyType => currencyType;

	public int Limit => limit;

	private void OnDrawGizmosSelected()
	{
		Gizmos.matrix = base.transform.localToWorldMatrix;
		Bounds bounds = default(Bounds);
		bounds.min = min;
		bounds.max = max;
		Bounds bounds2 = bounds;
		Gizmos.DrawWireCube(bounds2.center, bounds2.size);
	}

	private void OnEnable()
	{
		_activeRegions.Add(this);
	}

	private void OnDisable()
	{
		_activeRegions.Remove(this);
	}

	public static void AddRegion(ICurrencyLimitRegion limitRegion)
	{
		_activeRegions.Add(limitRegion);
	}

	public static void RemoveRegion(ICurrencyLimitRegion limitRegion)
	{
		_activeRegions.Remove(limitRegion);
	}

	public static int GetLimit(Vector2 pos, CurrencyType limitedType)
	{
		int maxCurrencyObjects = Gameplay.GetMaxCurrencyObjects(limitedType);
		foreach (ICurrencyLimitRegion activeRegion in _activeRegions)
		{
			if (activeRegion.CurrencyType == limitedType && activeRegion.Limit <= maxCurrencyObjects && activeRegion.IsInsideLimitRegion(pos))
			{
				maxCurrencyObjects = activeRegion.Limit;
			}
		}
		return maxCurrencyObjects;
	}

	public bool IsInsideLimitRegion(Vector2 point)
	{
		Transform obj = base.transform;
		Vector3 vector = obj.TransformPoint(min);
		Vector3 vector2 = obj.TransformPoint(max);
		if (point.x < vector.x || point.y < vector.y || point.x > vector2.x || point.y > vector2.y)
		{
			return false;
		}
		return true;
	}
}

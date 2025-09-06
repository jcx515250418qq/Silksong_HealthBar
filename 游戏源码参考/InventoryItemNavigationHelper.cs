using System;
using System.Collections.Generic;
using UnityEngine;

public static class InventoryItemNavigationHelper
{
	public static T GetClosestOnAxis<T>(InventoryItemManager.SelectionDirection direction, InventoryItemSelectable currentSelected, IEnumerable<T> items) where T : InventoryItemSelectable
	{
		T val = ((direction != InventoryItemManager.SelectionDirection.Left && direction != InventoryItemManager.SelectionDirection.Right) ? GetClosest(GetYAxis, GetXAxis, direction == InventoryItemManager.SelectionDirection.Down, currentSelected, items) : GetClosest(GetXAxis, GetYAxis, direction == InventoryItemManager.SelectionDirection.Left, currentSelected, items));
		if ((bool)val)
		{
			return (T)val.Get(direction);
		}
		return null;
	}

	private static T GetClosest<T>(Func<Vector2, float> getMainAxis, Func<Vector2, float> getSecondaryAxis, bool selectPositive, InventoryItemSelectable currentSelected, IEnumerable<T> items) where T : InventoryItemSelectable
	{
		Vector2 a = Vector2.zero;
		float? num = null;
		float y = getSecondaryAxis(currentSelected.transform.TransformPoint(currentSelected.NavigationOffset));
		foreach (T item in items)
		{
			Vector3 vector = item.transform.TransformPoint(item.NavigationOffset);
			float num2 = getMainAxis(vector);
			if (!num.HasValue || (selectPositive && num2 > num) || (!selectPositive && num2 < num))
			{
				num = num2;
				a = vector;
			}
		}
		if (num.HasValue)
		{
			Vector2 arg = new Vector2(num.Value, y);
			Vector2 a2 = new Vector2(getMainAxis(arg), getSecondaryAxis(arg));
			float num3 = float.MaxValue;
			float num4 = float.MaxValue;
			T result = null;
			{
				foreach (T item2 in items)
				{
					Vector3 vector2 = item2.transform.TransformPoint(item2.NavigationOffset);
					float num5 = Vector2.Distance(a2, vector2);
					if (num5 >= num3)
					{
						if (!(Math.Abs(num5 - num3) < 0.2f))
						{
							continue;
						}
						float num6 = Vector2.Distance(a, vector2);
						if (num6 >= num4)
						{
							continue;
						}
						num4 = num6;
					}
					num3 = num5;
					result = item2;
				}
				return result;
			}
		}
		return null;
	}

	private static float GetXAxis(Vector2 vector)
	{
		return vector.x;
	}

	private static float GetYAxis(Vector2 vector)
	{
		return vector.y;
	}
}

using System;
using System.Collections.Generic;
using UnityEngine;

public class InventoryAutoNavGroup : MonoBehaviour
{
	private readonly List<InventoryItemSelectable> selectables = new List<InventoryItemSelectable>();

	public void Register(InventoryItemSelectable selectable)
	{
		selectables.AddIfNotPresent(selectable);
	}

	public void Deregister(InventoryItemSelectable selectable)
	{
		selectables.Remove(selectable);
	}

	public InventoryItemSelectable GetNextSelectable(InventoryItemSelectable currentSelected, InventoryItemManager.SelectionDirection direction, Func<InventoryItemSelectable, bool> predicate = null)
	{
		return this.GetNextSelectable<InventoryItemSelectable>(currentSelected, direction, predicate);
	}

	public InventoryItemSelectable GetNextSelectable<T>(InventoryItemSelectable fromSelectable, InventoryItemManager.SelectionDirection direction, Func<T, bool> predicate = null) where T : InventoryItemSelectable
	{
		Vector2 vector = fromSelectable.transform.TransformPoint(fromSelectable.NavigationOffset);
		float directionAngle = GetDirectionAngle(direction);
		Vector2 vector2 = direction switch
		{
			InventoryItemManager.SelectionDirection.Down => new Vector2(1.15f, 1f), 
			InventoryItemManager.SelectionDirection.Up => new Vector2(1.15f, 1f), 
			InventoryItemManager.SelectionDirection.Left => new Vector2(1f, 1.15f), 
			InventoryItemManager.SelectionDirection.Right => new Vector2(1f, 1.15f), 
			_ => throw new ArgumentOutOfRangeException("direction", direction, null), 
		};
		InventoryItemSelectable result = null;
		float num = float.MaxValue;
		foreach (InventoryItemSelectable selectable in selectables)
		{
			if (selectable == fromSelectable)
			{
				continue;
			}
			T val = selectable as T;
			if (!val || (predicate != null && !predicate(val)))
			{
				continue;
			}
			Vector2 vector3 = (Vector2)selectable.transform.TransformPoint(selectable.NavigationOffset) - vector;
			if (Vector2.SignedAngle(Vector2.right, vector3.normalized).IsAngleWithinTolerance(67.5f, directionAngle))
			{
				float magnitude = (vector3 * vector2).magnitude;
				if (!(magnitude >= num))
				{
					num = magnitude;
					result = selectable;
				}
			}
		}
		return result;
	}

	private float GetDirectionAngle(InventoryItemManager.SelectionDirection direction)
	{
		return direction switch
		{
			InventoryItemManager.SelectionDirection.Up => 90f, 
			InventoryItemManager.SelectionDirection.Down => 270f, 
			InventoryItemManager.SelectionDirection.Left => 180f, 
			InventoryItemManager.SelectionDirection.Right => 0f, 
			_ => throw new NotImplementedException(), 
		};
	}
}

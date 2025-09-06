using System;
using UnityEngine;

public sealed class InventoryCollectableItemSelectionHelper : MonoBehaviour
{
	[Serializable]
	public enum SelectionType
	{
		None = 0,
		Needle = 1,
		MaskShard = 2,
		SpoolPiece = 3,
		Silk = 4,
		Needolin = 5,
		Sprint = 6,
		HarpoonDash = 7,
		EvaHeal = 8,
		SuperJump = 9,
		WallJump = 10
	}

	[ArrayForEnum(typeof(SelectionType))]
	[SerializeField]
	private InventoryItemSelectable[] selectables = new InventoryItemSelectable[0];

	private static SelectionType lastSelectionUpdate;

	public static SelectionType LastSelectionUpdate
	{
		get
		{
			return lastSelectionUpdate;
		}
		set
		{
			if (value != 0)
			{
				CollectableItemManager.CollectedItem = null;
				InventoryPaneList.SetNextOpen("Inv");
			}
			lastSelectionUpdate = value;
		}
	}

	private void OnValidate()
	{
		ArrayForEnumAttribute.EnsureArraySize(ref selectables, typeof(SelectionType));
	}

	private void OnDestroy()
	{
		LastSelectionUpdate = SelectionType.None;
	}

	public bool TryGetSelectable(out InventoryItemSelectable selectable)
	{
		if (lastSelectionUpdate == SelectionType.None)
		{
			selectable = null;
			return false;
		}
		try
		{
			selectable = selectables[(int)lastSelectionUpdate];
		}
		catch (Exception)
		{
			selectable = null;
		}
		lastSelectionUpdate = SelectionType.None;
		return true;
	}
}

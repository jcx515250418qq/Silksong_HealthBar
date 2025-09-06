using UnityEngine;
using UnityEngine.UI;

namespace TeamCherry.DebugMenu
{
	public class DebugMenuGridLayoutHelper : MonoBehaviour
	{
		[SerializeField]
		private GridLayoutGroup gridLayoutGroup;

		[Header("Settings")]
		[SerializeField]
		private bool fitInRect;

		[Space]
		[SerializeField]
		private float width;

		[SerializeField]
		private float height;

		[Space]
		private Vector2 cellSize;

		[Space]
		[SerializeField]
		private Vector2 spacing;

		[Space]
		[SerializeField]
		private int targetRows = -1;

		[SerializeField]
		private int targetColumns = -1;

		private void OnValidate()
		{
			if (!gridLayoutGroup)
			{
				gridLayoutGroup = GetComponent<GridLayoutGroup>();
			}
		}

		[ContextMenu("Apply Layout")]
		private void CalculateLayout()
		{
			if ((bool)gridLayoutGroup)
			{
				base.gameObject.RecordUndoChanges("Apply Layout");
				if (fitInRect)
				{
					Rect rect = (gridLayoutGroup.transform as RectTransform).rect;
					width = rect.width - (float)gridLayoutGroup.padding.left - (float)gridLayoutGroup.padding.right;
					height = rect.height - (float)gridLayoutGroup.padding.top - (float)gridLayoutGroup.padding.bottom;
				}
				if (targetColumns > 0)
				{
					float num = width - spacing.x * (float)(targetColumns - 1);
					cellSize.x = num / (float)targetColumns;
				}
				if (targetRows > 0)
				{
					float num2 = height - spacing.y * (float)(targetRows - 1);
					cellSize.y = num2 / (float)targetRows;
				}
				gridLayoutGroup.spacing = spacing;
				gridLayoutGroup.cellSize = cellSize;
				base.gameObject.ApplyPrefabInstanceModifications();
			}
		}
	}
}

using UnityEngine;

[ExecuteInEditMode]
public class TransformLayoutGrid : TransformLayout
{
	[SerializeField]
	private Vector2 gridOffset;

	[SerializeField]
	private Vector2 itemOffset;

	[SerializeField]
	private bool startAtZero;

	[ContextMenu("Refresh")]
	public override void UpdatePositions()
	{
		int childCount = base.transform.childCount;
		int num = (startAtZero ? (-1) : 0);
		for (int i = 0; i < childCount; i++)
		{
			Transform child = base.transform.GetChild(i);
			if (child.gameObject.activeSelf)
			{
				num++;
				Vector2 position = gridOffset + itemOffset * num;
				child.SetLocalPosition2D(position);
			}
		}
	}
}

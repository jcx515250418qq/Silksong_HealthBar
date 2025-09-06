using UnityEngine;

public class InventoryCursorTarget : MonoBehaviour, InventoryCursor.ICursorTarget
{
	[SerializeField]
	private bool showCursor = true;

	[SerializeField]
	private bool overrideCursorColor;

	[SerializeField]
	[ModifiableProperty]
	[Conditional("overrideCursorColor", true, false, false)]
	private Color cursorColor;

	public bool ShowCursor => showCursor;

	public Color? CursorColor
	{
		get
		{
			if (!overrideCursorColor)
			{
				return null;
			}
			return cursorColor;
		}
	}
}

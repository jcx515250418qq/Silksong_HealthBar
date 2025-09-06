using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

[ExecuteInEditMode]
public sealed class GraphicLayoutDirtyEvent : MonoBehaviour
{
	[SerializeField]
	private Graphic graphic;

	public UnityEvent onLayoutDirty;

	private void OnEnable()
	{
		if (graphic != null)
		{
			graphic.RegisterDirtyLayoutCallback(UpdateLayout);
			UpdateLayout();
		}
	}

	private void OnDisable()
	{
		if (graphic != null)
		{
			graphic.UnregisterDirtyLayoutCallback(UpdateLayout);
		}
	}

	private void UpdateLayout()
	{
		onLayoutDirty?.Invoke();
	}
}

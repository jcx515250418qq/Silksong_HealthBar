using UnityEngine;
using UnityEngine.Events;

public class TriggerEvent : MonoBehaviour
{
	public UnityEvent TriggerEntered;

	public UnityEvent TriggerExited;

	[SerializeField]
	[Tooltip("If not 0, will only work if z position is within +- this value.")]
	private float activeZWidth = 0.5f;

	[SerializeField]
	[Tooltip("If set, will only work when assigned renderers are all visible.")]
	private Renderer[] activeRenderers;

	public bool IsActive
	{
		get
		{
			Renderer[] array = activeRenderers;
			for (int i = 0; i < array.Length; i++)
			{
				if (!array[i].isVisible)
				{
					return false;
				}
			}
			if (!(activeZWidth < Mathf.Epsilon))
			{
				return Mathf.Abs(base.transform.position.z) <= activeZWidth;
			}
			return true;
		}
	}

	private void OnTriggerEnter2D(Collider2D collision)
	{
		if (IsActive)
		{
			TriggerEntered.Invoke();
		}
	}

	private void OnTriggerExit2D(Collider2D collision)
	{
		if (IsActive)
		{
			TriggerExited.Invoke();
		}
	}
}

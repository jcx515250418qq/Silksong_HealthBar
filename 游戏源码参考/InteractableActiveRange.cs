using UnityEngine;

public class InteractableActiveRange : MonoBehaviour, IInteractableBlocker
{
	[SerializeField]
	private InteractableBase interactable;

	[SerializeField]
	private Vector2 min;

	[SerializeField]
	private Vector2 max;

	public bool IsBlocking
	{
		get
		{
			Vector2 vector = base.transform.position;
			return !(vector.x > min.x) || !(vector.x < max.x) || !(vector.y > min.y) || !(vector.y < max.y);
		}
	}

	private void OnDrawGizmosSelected()
	{
		Vector2 vector = max - min;
		Gizmos.DrawWireCube(min + vector / 2f, vector);
	}

	private void OnValidate()
	{
		if (min.x > max.x)
		{
			min.x = max.x;
		}
		if (max.x < min.x)
		{
			max.x = min.x;
		}
		if (min.y > max.y)
		{
			min.y = max.y;
		}
		if (max.y < min.y)
		{
			max.y = min.y;
		}
	}

	private void OnEnable()
	{
		if ((bool)interactable)
		{
			interactable.AddBlocker(this);
		}
	}

	private void OnDisable()
	{
		if ((bool)interactable)
		{
			interactable.RemoveBlocker(this);
		}
	}
}

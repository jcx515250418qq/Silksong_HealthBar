using TeamCherry.SharedUtils;
using UnityEngine;

public class BreakableBreaker : MonoBehaviour
{
	public enum BreakableTypes
	{
		Basic = 0,
		Grass = 1
	}

	[SerializeField]
	[EnumPickerBitmask(typeof(BreakableTypes))]
	private int breakTypeMask = -1;

	[SerializeField]
	private bool breakInstantly = true;

	private void OnTriggerEnter2D(Collider2D collision)
	{
		HandleCollider(collision);
	}

	private void OnCollisionEnter2D(Collision2D collision)
	{
		HandleCollider(collision.collider);
	}

	private void HandleCollider(Collider2D collision)
	{
		IBreakerBreakable component = collision.GetComponent<IBreakerBreakable>();
		if (component != null && breakTypeMask.IsBitSet((int)component.BreakableType))
		{
			if (breakInstantly)
			{
				component.BreakFromBreaker(this);
			}
			else
			{
				component.HitFromBreaker(this);
			}
		}
	}
}

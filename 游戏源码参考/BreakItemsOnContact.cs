using UnityEngine;

public class BreakItemsOnContact : MonoBehaviour
{
	private void OnTriggerEnter2D(Collider2D collision)
	{
		if (base.gameObject.layer == 17)
		{
			collision.GetComponent<IBreakOnContact>()?.Break();
		}
	}
}

using UnityEngine;

public class CorpseDropRegion : MonoBehaviour
{
	[SerializeField]
	private bool waitToDrop;

	private void OnTriggerEnter2D(Collider2D collision)
	{
		Corpse component = collision.gameObject.GetComponent<Corpse>();
		if ((bool)component)
		{
			component.DropThroughFloor(waitToDrop);
		}
	}
}

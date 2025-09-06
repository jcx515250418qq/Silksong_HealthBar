using UnityEngine;

public class CorpseCatcher : MonoBehaviour
{
	private const int CORPSE_LAYER = 26;

	private const int PARTICLE_LAYER = 18;

	private void OnTriggerEnter2D(Collider2D collision)
	{
		GameObject gameObject = collision.gameObject;
		if (gameObject.layer != 26 && gameObject.layer != 18)
		{
			return;
		}
		Transform parent = collision.transform.parent;
		while (parent != null)
		{
			if (parent.gameObject.layer == 26)
			{
				return;
			}
			parent = parent.parent;
		}
		gameObject.transform.SetParent(base.transform);
		if (ActiveCorpse.TryGetCorpse(gameObject, out var corpse))
		{
			corpse.SetInert(setIsInert: true);
		}
	}

	private void OnTriggerExit2D(Collider2D collision)
	{
		GameObject gameObject = collision.gameObject;
		if ((gameObject.layer == 26 || gameObject.layer == 18) && gameObject.transform.parent == base.transform)
		{
			gameObject.transform.SetParent(null);
			if (ObjectPool.ObjectWasSpawned(gameObject))
			{
				Object.DontDestroyOnLoad(gameObject);
			}
		}
		if (ActiveCorpse.TryGetCorpse(gameObject, out var corpse))
		{
			corpse.SetInert(setIsInert: false);
		}
	}
}

using System.Collections;
using UnityEngine;

public class GrassWind : MonoBehaviour
{
	private Collider2D col;

	private bool dirty;

	private bool hasGrassBehaviour;

	private GrassBehaviour grassBehaviour;

	private void Awake()
	{
		col = GetComponent<Collider2D>();
		CacheGrassBehaviour();
	}

	private void OnTriggerEnter2D(Collider2D collision)
	{
		if (collision.CompareTag("Nail Attack"))
		{
			if (dirty)
			{
				CacheGrassBehaviour();
			}
			if (hasGrassBehaviour)
			{
				StartCoroutine(DelayReact(grassBehaviour, collision));
			}
		}
	}

	private IEnumerator DelayReact(GrassBehaviour behaviour, Collider2D collision)
	{
		yield return null;
		behaviour.WindReact(collision);
	}

	private void OnTransformParentChanged()
	{
		dirty = true;
	}

	private void CacheGrassBehaviour()
	{
		grassBehaviour = GetComponentInParent<GrassBehaviour>();
		hasGrassBehaviour = grassBehaviour;
	}
}

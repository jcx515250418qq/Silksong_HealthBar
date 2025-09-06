using System;
using System.Collections.Generic;
using UnityEngine;

public class CogCorpseReaction : MonoBehaviour
{
	[SerializeField]
	private float jitterFrameTime = 0.025f;

	[SerializeField]
	private float jitterX = 0.2f;

	[SerializeField]
	private float jitterY = 0.2f;

	[SerializeField]
	private GameObject grindParticlePrefab;

	private readonly List<Transform> corpses = new List<Transform>();

	private float timer;

	private void Update()
	{
		if (corpses.Count <= 0)
		{
			return;
		}
		timer += Time.deltaTime;
		if (!(timer < jitterFrameTime))
		{
			for (int i = 0; i < corpses.Count; i++)
			{
				Transform obj = corpses[i];
				Vector3 position = obj.position;
				obj.position = new Vector3(position.x + UnityEngine.Random.Range(0f - jitterX, jitterX), position.y + UnityEngine.Random.Range(0f - jitterY, jitterY), position.z);
			}
			timer -= jitterFrameTime;
		}
	}

	private void OnTriggerEnter2D(Collider2D collision)
	{
		GameObject gameObject = collision.gameObject;
		if (!ShouldGrab(gameObject, out var item))
		{
			return;
		}
		corpses.Add(gameObject.transform);
		Rigidbody2D component = gameObject.GetComponent<Rigidbody2D>();
		if ((bool)component)
		{
			component.linearVelocity = new Vector2(0f, 0f);
			component.angularVelocity = UnityEngine.Random.Range(-1000f, 1000f);
			GameObject grindParticle = grindParticlePrefab.Spawn(gameObject.transform, Vector3.zero, Quaternion.identity);
			RecycleResetHandler.Add(gameObject, (Action)delegate
			{
				grindParticle.Recycle();
			});
		}
		if ((bool)item)
		{
			item.EnteredCogs();
			item.AddTrackedRegion(this);
			return;
		}
		Collider2D component2 = gameObject.GetComponent<Collider2D>();
		if ((bool)component2)
		{
			component2.isTrigger = true;
		}
	}

	private void OnTriggerExit2D(Collider2D collision)
	{
		if (ShouldGrab(collision.gameObject, out var item))
		{
			corpses.Remove(collision.gameObject.transform);
			if ((bool)item)
			{
				item.ExitedCogs();
				item.RemoveTrackedRegion(this);
			}
		}
	}

	public void RemoveCorpse(Transform transform)
	{
		corpses.Remove(transform);
	}

	private bool ShouldGrab(GameObject obj, out CogCorpseItem item)
	{
		item = obj.GetComponent<CogCorpseItem>();
		if (obj.layer != 26)
		{
			if ((bool)item)
			{
				return !item.IsBroken;
			}
			return false;
		}
		return true;
	}
}

using System.Collections.Generic;
using UnityEngine;

public class BoneSegment : MonoBehaviour
{
	[SerializeField]
	private GameObject sprite;

	[SerializeField]
	private ParticleSystem particle;

	[SerializeField]
	private AudioEventRandom depressSound;

	[SerializeField]
	private Transform[] depressOthers;

	private readonly List<GameObject> touchingObjects = new List<GameObject>();

	private bool depressed;

	private void OnTriggerEnter2D(Collider2D collision)
	{
		int layer = collision.gameObject.layer;
		bool flag = layer == 9;
		if (flag || layer == 11 || layer == 26)
		{
			touchingObjects.AddIfNotPresent(collision.gameObject);
			if (!depressed)
			{
				Depress(flag);
			}
		}
	}

	private void OnTriggerExit2D(Collider2D collision)
	{
		touchingObjects.Remove(collision.gameObject);
		if (touchingObjects.Count <= 0)
		{
			Release();
		}
	}

	private void Depress(bool vibrate)
	{
		if (depressed)
		{
			return;
		}
		depressed = true;
		if (particle != null)
		{
			particle.Play();
		}
		sprite.transform.SetLocalPosition2D(new Vector2(0f, -0.05f));
		Vector3 vector = new Vector3(0f, -0.05f, 0f);
		Transform[] array = depressOthers;
		foreach (Transform transform in array)
		{
			if ((bool)transform)
			{
				transform.localPosition += vector;
			}
		}
		depressSound.SpawnAndPlayOneShot(base.transform.position, vibrate);
	}

	private void Release()
	{
		if (!depressed)
		{
			return;
		}
		depressed = false;
		sprite.transform.SetLocalPosition2D(new Vector3(0f, 0f));
		Vector3 vector = new Vector3(0f, 0.05f, 0f);
		Transform[] array = depressOthers;
		foreach (Transform transform in array)
		{
			if ((bool)transform)
			{
				transform.localPosition += vector;
			}
		}
	}
}

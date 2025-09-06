using System.Collections.Generic;
using UnityEngine;

public class AirZone : MonoBehaviour
{
	private const float forceMinX = -10f;

	private const float forceMaxX = 10f;

	private const float forceMinY = 60f;

	private const float forceMaxY = 125f;

	private const int CORPSE_LAYER = 26;

	private const int PARTICLE_LAYER = 18;

	public List<Rigidbody2D> blowParticles;

	private void OnTriggerEnter2D(Collider2D collision)
	{
		GameObject gameObject = collision.gameObject;
		if (collision.gameObject.layer == 26 && ActiveCorpse.TryGetCorpse(collision.gameObject, out var corpse))
		{
			corpse.SetInAirZone(isInAirZone: true);
		}
		if (collision.gameObject.layer == 18)
		{
			Rigidbody2D component = gameObject.GetComponent<Rigidbody2D>();
			if ((bool)component && component.gravityScale <= 2f)
			{
				blowParticles.Add(gameObject.GetComponent<Rigidbody2D>());
			}
		}
	}

	private void OnTriggerExit2D(Collider2D collision)
	{
		GameObject gameObject = collision.gameObject;
		if (gameObject.layer == 26 && ActiveCorpse.TryGetCorpse(collision.gameObject, out var corpse))
		{
			corpse.SetInAirZone(isInAirZone: false);
		}
		if (collision.gameObject.layer == 18 && (bool)gameObject.GetComponent<Rigidbody2D>())
		{
			blowParticles.Remove(gameObject.GetComponent<Rigidbody2D>());
		}
	}

	private void FixedUpdate()
	{
		foreach (Rigidbody2D blowParticle in blowParticles)
		{
			Vector2 force = new Vector2(Random.Range(-10f, 10f), Random.Range(60f, 125f));
			blowParticle.AddForce(force);
		}
	}
}

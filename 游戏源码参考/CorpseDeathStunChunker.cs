using System.Collections;
using UnityEngine;

public class CorpseDeathStunChunker : CorpseChunker
{
	[Header("Death Stun Variables")]
	[SerializeField]
	protected GameObject deathWaveInfectedPrefab;

	public ParticleSystem stunSteam;

	protected override void Start()
	{
		StartCoroutine(DeathStun());
	}

	private IEnumerator DeathStun()
	{
		if ((bool)stunSteam)
		{
			stunSteam.Play();
		}
		SpriteFlash spriteFlash = GetComponent<SpriteFlash>();
		if ((bool)spriteFlash)
		{
			spriteFlash.flashInfectedLoop();
		}
		Vector2 velocity = Vector2.zero;
		if ((bool)body)
		{
			velocity = body.linearVelocity;
			body.isKinematic = true;
			body.linearVelocity = Vector2.zero;
		}
		yield return StartCoroutine(Jitter(0.75f));
		if ((bool)spriteFlash)
		{
			spriteFlash.CancelFlash();
		}
		Object.Instantiate(deathWaveInfectedPrefab, base.transform.position, Quaternion.identity).transform.localScale = new Vector3(2f, 2f, 2f);
		if ((bool)body)
		{
			body.isKinematic = false;
			body.linearVelocity = velocity;
		}
		if ((bool)stunSteam)
		{
			stunSteam.Stop();
		}
		base.Start();
	}

	private IEnumerator Jitter(float duration)
	{
		Transform sprite = spriteAnimator.transform;
		Vector3 initialPos = sprite.position;
		for (float elapsed = 0f; elapsed < duration; elapsed += Time.deltaTime)
		{
			sprite.position = initialPos + new Vector3(Random.Range(-0.15f, 0.15f), 0f, 0f);
			yield return null;
		}
		sprite.position = initialPos;
	}
}

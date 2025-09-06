using System.Collections;
using UnityEngine;

public class CorpseGoopExplode : Corpse
{
	[Header("Goop Explode Variables")]
	public GameObject gasExplosion;

	public AudioEvent gushSound;

	protected override void LandEffects()
	{
		base.LandEffects();
		StartCoroutine(DoLandEffects());
	}

	private IEnumerator DoLandEffects()
	{
		body.isKinematic = true;
		body.linearVelocity = Vector3.zero;
		yield return new WaitForSeconds(1f);
		body.linearVelocity = Vector2.zero;
		gushSound.SpawnAndPlayOneShot(audioPlayerPrefab, base.transform.position);
		yield return StartCoroutine(Jitter(0.7f));
		GameCameras gameCameras = Object.FindObjectOfType<GameCameras>();
		if ((bool)gameCameras)
		{
			gameCameras.cameraShakeFSM.SendEvent("EnemyKillShake");
		}
		if ((bool)gasExplosion)
		{
			Object.Instantiate(gasExplosion, base.transform.position, Quaternion.identity);
		}
		meshRenderer.enabled = false;
	}

	private IEnumerator Jitter(float duration)
	{
		Transform sprite = spriteAnimator.transform;
		Vector3 initialPos = sprite.position;
		for (float elapsed = 0f; elapsed < duration; elapsed += Time.deltaTime)
		{
			sprite.position = initialPos + new Vector3(Random.Range(-0.1f, 0.1f), 0f, 0f);
			yield return null;
		}
		sprite.position = initialPos;
	}
}

using System.Collections;
using UnityEngine;

public class CorpseFungusExplode : Corpse
{
	[Header("Fungus Explode Variables")]
	public ParticleSystem anticSteam;

	public ParticleSystem gasAttack;

	public AudioEvent gushSound;

	public AudioEvent explodeSound;

	public GameObject gasHitBox;

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
		if ((bool)anticSteam)
		{
			anticSteam.Play();
		}
		body.linearVelocity = Vector2.zero;
		gushSound.SpawnAndPlayOneShot(audioPlayerPrefab, base.transform.position);
		yield return StartCoroutine(Jitter(0.9f));
		explodeSound.SpawnAndPlayOneShot(audioPlayerPrefab, base.transform.position);
		if ((bool)anticSteam)
		{
			anticSteam.Stop();
		}
		GameCameras gameCameras = Object.FindObjectOfType<GameCameras>();
		if ((bool)gameCameras)
		{
			gameCameras.cameraShakeFSM.SendEvent("EnemyKillShake");
		}
		if ((bool)gasAttack)
		{
			gasAttack.Play();
		}
		if ((bool)gasHitBox)
		{
			gasHitBox.SetActive(value: true);
			Vector3 localScale = gasHitBox.transform.localScale;
			gasHitBox.transform.localScale = new Vector3(0.2f, 0.2f, 0.2f);
			iTween.ScaleTo(gasHitBox, iTween.Hash("scale", localScale, "time", 0.4f, "easetype", iTween.EaseType.easeOutCirc, "islocal", true));
			yield return new WaitForSeconds(0.4f);
		}
		meshRenderer.enabled = false;
		yield return new WaitForSeconds(0.4f);
		if ((bool)gasHitBox)
		{
			gasHitBox.SetActive(value: false);
		}
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

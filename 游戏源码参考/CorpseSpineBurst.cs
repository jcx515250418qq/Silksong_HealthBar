using System.Collections;
using UnityEngine;

public class CorpseSpineBurst : Corpse
{
	[Header("Spine Burst Variables")]
	public AudioEvent shakerExplode;

	public AudioEvent zombiePrep;

	public AudioEvent zombieShoot;

	[Space]
	public GameObject spineHit;

	public GameObject lines;

	protected override void LandEffects()
	{
		base.LandEffects();
		StartCoroutine(DoLandEffects());
	}

	private IEnumerator DoLandEffects(bool burst = true)
	{
		body.isKinematic = true;
		body.linearVelocity = Vector3.zero;
		if (burst)
		{
			yield return new WaitForSeconds(1f);
			spriteAnimator.Play("Burst Antic");
			shakerExplode.SpawnAndPlayOneShot(audioPlayerPrefab, base.transform.position);
			yield return new WaitForSeconds(0.9f);
			spriteAnimator.Play("Burst");
			zombiePrep.SpawnAndPlayOneShot(audioPlayerPrefab, base.transform.position);
			zombieShoot.SpawnAndPlayOneShot(audioPlayerPrefab, base.transform.position);
			if ((bool)spineHit)
			{
				spineHit.SetActive(value: true);
			}
			if ((bool)lines)
			{
				lines.SetActive(value: true);
			}
			if (Vector2.Distance(HeroController.instance.transform.position, base.transform.position) <= 44f)
			{
				GameCameras gameCameras = Object.FindObjectOfType<GameCameras>();
				if ((bool)gameCameras)
				{
					gameCameras.cameraShakeFSM.SendEvent("EnemyKillShake");
				}
			}
		}
		HealthManager component = GetComponent<HealthManager>();
		if ((bool)component)
		{
			component.IsInvincible = false;
		}
	}
}

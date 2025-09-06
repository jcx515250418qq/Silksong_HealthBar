using UnityEngine;

public class BossStatueLever : MonoBehaviour, IBossStatueToggle
{
	public Transform hitOrigin;

	public AudioSource audioPlayerPrefab;

	public AudioEvent switchSound;

	public GameObject strikeNailPrefab;

	private bool canToggle = true;

	public Animator leverAnimator;

	private BossStatue bossStatue;

	private void Enable()
	{
		base.gameObject.SetActive(value: true);
	}

	private void OnTriggerEnter2D(Collider2D collision)
	{
		if (base.gameObject.activeInHierarchy && canToggle && collision.tag == "Nail Attack")
		{
			bossStatue.SetDreamVersion(!bossStatue.UsingDreamVersion, useAltStatue: true);
			canToggle = false;
			switchSound.SpawnAndPlayOneShot(audioPlayerPrefab, base.transform.position);
			GameManager.instance.FreezeMoment(1);
			GameCameras.instance.cameraShakeFSM.SendEvent("EnemyKillShake");
			if ((bool)strikeNailPrefab && (bool)hitOrigin)
			{
				strikeNailPrefab.Spawn(hitOrigin.transform.position);
			}
			if ((bool)leverAnimator)
			{
				leverAnimator.Play("Hit");
			}
		}
	}

	public void SetOwner(BossStatue statue)
	{
		bossStatue = statue;
		if (bossStatue.UsingDreamVersion)
		{
			bossStatue.SetDreamVersion(value: true, useAltStatue: true, doAnim: false);
		}
		bossStatue.OnStatueSwapFinished += delegate
		{
			canToggle = true;
			if ((bool)leverAnimator)
			{
				leverAnimator.Play("Shine");
			}
		};
	}

	public void SetState(bool value)
	{
		canToggle = value;
		if (!value)
		{
			base.gameObject.SetActive(value: false);
		}
	}
}

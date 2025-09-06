using System.Collections;
using UnityEngine;

public class MinesFlipPlatform : MonoBehaviour
{
	public tk2dSpriteAnimator spriteAnimator;

	[Space]
	public AudioClip flipSound;

	public AudioClip flipBackSound;

	public AudioClip hitSound;

	[Space]
	public GameObject strikeEffect;

	public GameObject nailStrikePrefab;

	[Space]
	public ParticleSystem crystalParticles;

	public ParticleSystem crystalHitParticles;

	[Space]
	public GameObject topSpikes;

	public GameObject bottomSpikes;

	private Coroutine idleRoutine;

	private Coroutine flipRoutine;

	private bool hitCancel;

	private TriggerEnterEvent triggerEnter;

	private AudioSource audioSource;

	private void Awake()
	{
		audioSource = GetComponent<AudioSource>();
	}

	private void Start()
	{
		idleRoutine = StartCoroutine(Idle());
		if ((bool)topSpikes)
		{
			triggerEnter = topSpikes.GetComponent<TriggerEnterEvent>();
		}
		if (!triggerEnter)
		{
			return;
		}
		triggerEnter.OnTriggerEntered += delegate(Collider2D collider, GameObject sender)
		{
			if (collider.tag == "Nail Attack")
			{
				hitCancel = true;
			}
		};
	}

	private void OnCollisionEnter2D(Collision2D collision)
	{
		if (collision.gameObject.tag == "Player")
		{
			if (idleRoutine != null)
			{
				StopCoroutine(idleRoutine);
			}
			if (flipRoutine == null)
			{
				flipRoutine = StartCoroutine(Flip());
			}
		}
	}

	private void PlaySound(AudioClip clip)
	{
		if ((bool)audioSource && (bool)clip)
		{
			audioSource.PlayOneShot(clip);
		}
	}

	private IEnumerator Idle()
	{
		while (true)
		{
			spriteAnimator.Play("Idle Up");
			yield return new WaitForSeconds(Random.Range(2f, 5f));
			tk2dSpriteAnimationClip clipByName = spriteAnimator.GetClipByName("Glimmer Up");
			spriteAnimator.Play(clipByName);
			yield return new WaitForSeconds(clipByName.Duration);
		}
	}

	private IEnumerator Flip()
	{
		PlaySound(flipSound);
		spriteAnimator.Play("Shake Up");
		if ((bool)strikeEffect)
		{
			strikeEffect.SetActive(value: true);
		}
		StartCoroutine(Jitter(0.5f));
		yield return new WaitForSeconds(0.5f);
		if ((bool)crystalParticles)
		{
			crystalParticles.Play();
		}
		yield return StartCoroutine(spriteAnimator.PlayAnimWait("Flip Down 1"));
		if ((bool)crystalParticles)
		{
			crystalParticles.Stop(withChildren: true, ParticleSystemStopBehavior.StopEmitting);
		}
		yield return StartCoroutine(spriteAnimator.PlayAnimWait("Flip Down 2"));
		if ((bool)topSpikes)
		{
			topSpikes.SetActive(value: true);
		}
		if ((bool)bottomSpikes)
		{
			bottomSpikes.SetActive(value: false);
		}
		spriteAnimator.Play("Idle Down");
		hitCancel = false;
		bool skipped = false;
		for (float elapsed = 0f; elapsed < 4f; elapsed += Time.deltaTime)
		{
			if (hitCancel)
			{
				skipped = true;
				if ((bool)nailStrikePrefab)
				{
					nailStrikePrefab.Spawn(base.transform.position);
				}
				if ((bool)crystalHitParticles)
				{
					crystalHitParticles.Play();
				}
				PlaySound(hitSound);
				GameCameras gameCameras = Object.FindObjectOfType<GameCameras>();
				if ((bool)gameCameras)
				{
					gameCameras.cameraShakeFSM.SendEvent("EnemyKillShake");
				}
				break;
			}
			yield return null;
		}
		PlaySound(flipBackSound);
		yield return StartCoroutine(spriteAnimator.PlayAnimWait(skipped ? "Flip Up 1N" : "Flip Up 1"));
		yield return StartCoroutine(spriteAnimator.PlayAnimWait("Flip Up 2"));
		if ((bool)topSpikes)
		{
			topSpikes.SetActive(value: false);
		}
		if ((bool)bottomSpikes)
		{
			bottomSpikes.SetActive(value: true);
		}
		flipRoutine = null;
		idleRoutine = StartCoroutine(Idle());
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

using System.Collections;
using UnityEngine;

public class DropPlatform : MonoBehaviour
{
	public tk2dSpriteAnimator spriteAnimator;

	[Space]
	public string idleAnim = "Idle Small";

	public string dropAnim = "Drop Small";

	public string raiseAnim = "Raise Small";

	[Space]
	public AudioClip landSound;

	public AudioClip dropSound;

	public AudioClip flipUpSound;

	[Space]
	public GameObject strikeEffect;

	[Space]
	public Collider2D collider;

	private Coroutine flipRoutine;

	private AudioSource audioSource;

	private void Awake()
	{
		audioSource = GetComponent<AudioSource>();
	}

	private void Start()
	{
		Idle();
	}

	private void OnCollisionEnter2D(Collision2D collision)
	{
		if (flipRoutine == null && collision.gameObject.layer == 9 && collision.collider.bounds.min.y > collider.bounds.max.y)
		{
			flipRoutine = StartCoroutine(Flip());
		}
	}

	private void PlaySound(AudioClip clip)
	{
		if ((bool)audioSource && (bool)clip)
		{
			audioSource.PlayOneShot(clip);
		}
	}

	private void Idle()
	{
		base.transform.SetPositionZ(0.003f);
		spriteAnimator.Play(idleAnim);
		if ((bool)collider)
		{
			collider.enabled = true;
		}
	}

	private IEnumerator Flip()
	{
		PlaySound(landSound);
		if ((bool)strikeEffect)
		{
			strikeEffect.SetActive(value: true);
		}
		yield return new WaitForSeconds(0.7f);
		if ((bool)collider)
		{
			collider.enabled = false;
		}
		PlaySound(dropSound);
		yield return StartCoroutine(spriteAnimator.PlayAnimWait(dropAnim));
		base.transform.SetPositionZ(0.007f);
		yield return new WaitForSeconds(1.5f);
		PlaySound(flipUpSound);
		yield return StartCoroutine(spriteAnimator.PlayAnimWait(raiseAnim));
		flipRoutine = null;
		Idle();
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

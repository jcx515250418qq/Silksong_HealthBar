using UnityEngine;

public class SceneryTriggerCircle : MonoBehaviour
{
	private Animator animator;

	private CircleCollider2D[] col2ds;

	private int enterCount;

	public AudioSource audioSource;

	public AudioClip activateSound;

	public AudioClip deactivateSound;

	public bool active { get; private set; }

	private void Awake()
	{
		col2ds = GetComponentsInChildren<CircleCollider2D>();
		animator = GetComponentInChildren<Animator>();
		if (col2ds.Length > 2 || col2ds.Length < 2)
		{
			Debug.LogError("Scenery Trigger requires exactly two Collider components attached to work correctly.");
			base.gameObject.SetActive(value: false);
		}
	}

	private void OnTriggerEnter2D(Collider2D other)
	{
		if (other.gameObject.layer != 9)
		{
			return;
		}
		if (enterCount == 0)
		{
			enterCount = 1;
		}
		else if (enterCount == 1)
		{
			active = true;
			animator.Play("Show");
			if (activateSound != null && audioSource != null)
			{
				RandomizePitch(audioSource, 0.85f, 1.15f);
				audioSource.PlayOneShot(activateSound);
			}
			enterCount = 2;
		}
	}

	private void OnTriggerStay2D(Collider2D other)
	{
		if (other.gameObject.layer == 9 && enterCount == 0)
		{
			enterCount = 1;
		}
	}

	private void OnTriggerExit2D(Collider2D other)
	{
		if (other.gameObject.layer != 9)
		{
			return;
		}
		if (enterCount == 1)
		{
			active = false;
			animator.Play("Hide");
			if (deactivateSound != null && audioSource != null)
			{
				RandomizePitch(audioSource, 0.85f, 1.15f);
				audioSource.PlayOneShot(deactivateSound);
			}
			enterCount = 0;
		}
		else if (enterCount == 2)
		{
			enterCount = 1;
		}
	}

	private void RandomizePitch(AudioSource src, float minPitch, float maxPitch)
	{
		float pitch = Random.Range(minPitch, maxPitch);
		src.pitch = pitch;
	}

	private void ResetPitch(AudioSource src)
	{
		src.pitch = 1f;
	}
}

using UnityEngine;

public class InfectedBurstLarge : MonoBehaviour
{
	public AudioSource audioSource;

	public GameObject effects;

	public SpriteRenderer spriteRenderer;

	public Animator animator;

	public CircleCollider2D circleCollider;

	private VibrationPlayer vibration;

	private void Awake()
	{
		vibration = GetComponent<VibrationPlayer>();
	}

	private void Start()
	{
		audioSource.pitch = Random.Range(0.8f, 1.2f);
	}

	private void OnTriggerEnter2D(Collider2D otherCollider)
	{
		if (otherCollider.gameObject.tag == "Nail Attack" || otherCollider.gameObject.tag == "Hero Spell" || (otherCollider.tag == "HeroBox" && HeroController.instance.cState.superDashing))
		{
			audioSource.Play();
			effects.SetActive(value: true);
			BloodSpawner.SpawnBlood(base.transform.position, 15, 15, 10f, 20f, 40f, 140f, null);
			spriteRenderer.enabled = false;
			animator.enabled = false;
			circleCollider.enabled = false;
			if ((bool)vibration)
			{
				vibration.Play();
			}
		}
	}
}

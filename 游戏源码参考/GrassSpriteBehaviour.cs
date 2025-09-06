using UnityEngine;

public class GrassSpriteBehaviour : MonoBehaviour
{
	[Header("Variables")]
	public bool isWindy;

	public bool noPushAnimation;

	[Space]
	public GameObject deathParticles;

	public GameObject deathParticlesWindy;

	public GameObject cutEffectPrefab;

	[Space]
	public AudioClip[] pushSounds;

	public AudioClip[] cutSounds;

	[Header("Animation State Names")]
	public string idleAnimation = "Idle";

	public string pushAnimation = "Push";

	public string cutAnimation = "Dead";

	[Space]
	public string idleWindyAnimation = "WindyIdle";

	public string pushWindyAnimation = "WindyPush";

	private bool isCut;

	private bool interaction = true;

	private bool visible;

	private Animator animator;

	private AudioSource source;

	private void Awake()
	{
		animator = GetComponent<Animator>();
		source = GetComponent<AudioSource>();
	}

	private void Start()
	{
		if (Mathf.Abs(base.transform.position.z - 0.004f) > 1.8f)
		{
			interaction = false;
		}
		Init();
	}

	private void OnBecameVisible()
	{
		visible = true;
	}

	private void OnBecameInvisible()
	{
		visible = false;
	}

	private void OnTriggerEnter2D(Collider2D collision)
	{
		if (isCut || !interaction || !visible)
		{
			return;
		}
		if (GrassCut.ShouldCut(collision))
		{
			animator.Play(cutAnimation);
			isCut = true;
			if (isWindy && (bool)deathParticlesWindy)
			{
				deathParticlesWindy.SetActive(value: true);
			}
			else if ((bool)deathParticles)
			{
				deathParticles.SetActive(value: true);
			}
			if ((bool)source && cutSounds.Length != 0)
			{
				source.PlayOneShot(cutSounds[Random.Range(0, cutSounds.Length)]);
			}
			if ((bool)cutEffectPrefab)
			{
				int num = (int)Mathf.Sign(collision.transform.position.x - base.transform.position.x);
				Vector3 position = (collision.transform.position + base.transform.position) / 2f;
				GameObject obj = cutEffectPrefab.Spawn(position);
				Vector3 localScale = obj.transform.localScale;
				localScale.x = Mathf.Abs(localScale.x) * (float)(-num);
				obj.transform.localScale = localScale;
			}
		}
		else
		{
			if (!noPushAnimation)
			{
				animator.Play(isWindy ? pushWindyAnimation : pushAnimation);
			}
			if ((bool)source && pushSounds.Length != 0)
			{
				source.PlayOneShot(pushSounds[Random.Range(0, pushSounds.Length)]);
			}
		}
	}

	private void Init()
	{
		animator.Play(isWindy ? idleWindyAnimation : idleAnimation);
	}

	public void SetWindy()
	{
		if (!isCut)
		{
			isWindy = true;
			noPushAnimation = true;
			Init();
		}
	}

	public void SetNotWindy()
	{
		if (!isCut)
		{
			isWindy = false;
			noPushAnimation = false;
			Init();
		}
	}
}

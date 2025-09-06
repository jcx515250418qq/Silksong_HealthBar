using UnityEngine;

public class BigCentipede : MonoBehaviour
{
	private Rigidbody2D body;

	private MeshRenderer meshRenderer;

	private AudioSource audioSource;

	private BigCentipedeSection[] sections;

	[SerializeField]
	private ParticleSystem entryDust;

	[SerializeField]
	private ParticleSystem exitDust;

	private Vector2 entryPoint;

	private Vector2 exitPoint;

	[SerializeField]
	private float burrowTime;

	[SerializeField]
	private float moveSpeed;

	private Vector2 direction;

	private bool fadingAudio;

	private bool isBurrowing;

	private float burrowTimer;

	[SerializeField]
	private Transform entry;

	[SerializeField]
	private Transform exit;

	public Vector2 EntryPoint => entryPoint;

	public Vector2 ExitPoint => exitPoint;

	public Vector2 Direction => direction;

	protected void Awake()
	{
		body = GetComponent<Rigidbody2D>();
		meshRenderer = GetComponent<MeshRenderer>();
		audioSource = GetComponent<AudioSource>();
		sections = GetComponentsInChildren<BigCentipedeSection>();
		if ((bool)audioSource)
		{
			audioSource.pitch = Random.Range(0.8f, 1.15f);
		}
	}

	protected void Start()
	{
		direction = ((Vector2)base.transform.right).normalized;
		entryDust.transform.parent = null;
		if (entry != null)
		{
			entryPoint = entry.transform.position;
			entryDust.transform.SetPosition2D(entry.transform.position);
		}
		else
		{
			entryPoint = (Vector2)base.transform.position - direction * 12f;
		}
		exitDust.transform.parent = null;
		if (exit != null)
		{
			exitPoint = exit.transform.position;
			exitDust.transform.SetPosition2D(exit.transform.position);
		}
		else
		{
			exitPoint = (Vector2)base.transform.position + direction * 6f;
		}
		UnBurrow(changePosition: false);
	}

	private void UnBurrow(bool changePosition)
	{
		entryDust.Play();
		isBurrowing = false;
		if (changePosition)
		{
			base.transform.SetPosition2D(entryPoint - direction * 2.6f);
		}
		exitDust.Stop();
		meshRenderer.enabled = true;
		audioSource.volume = 0f;
		fadingAudio = true;
	}

	private void Burrow()
	{
		exitDust.Play();
		isBurrowing = true;
		burrowTimer = 0f;
		meshRenderer.enabled = false;
	}

	protected void FixedUpdate()
	{
		body.MovePosition(body.position + direction * moveSpeed * Time.fixedDeltaTime);
	}

	protected void Update()
	{
		Vector2 lhs = base.transform.position;
		if (!isBurrowing)
		{
			if (Vector2.Dot(lhs, direction) > Vector2.Dot(exitPoint, direction))
			{
				Burrow();
			}
		}
		else
		{
			burrowTimer += Time.deltaTime;
			if (burrowTimer > burrowTime)
			{
				UnBurrow(changePosition: true);
			}
		}
		if (fadingAudio)
		{
			audioSource.volume += Time.deltaTime * 1.5f;
			if (audioSource.volume > 1f)
			{
				audioSource.volume = 1f;
				fadingAudio = false;
			}
		}
	}
}

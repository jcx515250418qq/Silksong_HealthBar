using UnityEngine;

public class BombBounce : MonoBehaviour
{
	public delegate void BounceEvent();

	public float bounceFactor;

	public float speedThreshold = 1f;

	public bool playSound;

	public float minBounceSpeed;

	public float maxBounceSpeed;

	public AudioClip[] clips;

	public int chanceToPlay = 100;

	public float pitchMin = 1f;

	public float pitchMax = 1f;

	public bool playAnimationOnBounce;

	public string animationName;

	public float animPause = 0.5f;

	private float speed;

	private float animTimer;

	private tk2dSpriteAnimator animator;

	private Vector2 velocity;

	private Vector2 lastPos;

	private Rigidbody2D rb;

	private AudioSource audio;

	private int chooser;

	private float xSpeed;

	private float prevXSpeed;

	private float ySpeed;

	private float prevYSpeed;

	private bool bouncing = true;

	public event BounceEvent OnBounce;

	private void Start()
	{
		rb = GetComponent<Rigidbody2D>();
		audio = GetComponent<AudioSource>();
		animator = GetComponent<tk2dSpriteAnimator>();
	}

	private void FixedUpdate()
	{
		if (bouncing)
		{
			Vector2 vector = new Vector2(base.transform.position.x, base.transform.position.y);
			velocity = vector - lastPos;
			lastPos = vector;
			speed = (rb ? rb.linearVelocity.magnitude : 0f);
		}
	}

	private void LateUpdate()
	{
		if (animTimer > 0f)
		{
			animTimer -= Time.deltaTime;
		}
		prevXSpeed = xSpeed;
		xSpeed = rb.linearVelocity.x;
		if (rb.linearVelocity.x < 0.01f && rb.linearVelocity.x > -0.01f)
		{
			if (prevXSpeed > 0f)
			{
				rb.linearVelocity = new Vector2(-3f, rb.linearVelocity.y);
			}
			else
			{
				rb.linearVelocity = new Vector2(3f, rb.linearVelocity.y);
			}
		}
		prevYSpeed = ySpeed;
		ySpeed = rb.linearVelocity.y;
		if (rb.linearVelocity.y < 0.01f && rb.linearVelocity.y > -0.01f)
		{
			if (prevYSpeed > 0f)
			{
				rb.linearVelocity.Set(rb.linearVelocity.x, 0f);
			}
			else
			{
				rb.linearVelocity = new Vector2(rb.linearVelocity.x, 10f);
			}
		}
	}

	private void OnCollisionEnter2D(Collision2D col)
	{
		if (!rb || rb.isKinematic || !bouncing || !(speed > speedThreshold))
		{
			return;
		}
		Vector3 inNormal = col.GetSafeContact().Normal;
		Vector3 normalized = Vector3.Reflect(velocity.normalized, inNormal).normalized;
		Vector2 linearVelocity = new Vector2(normalized.x, normalized.y) * (speed * bounceFactor);
		rb.linearVelocity = linearVelocity;
		if (playSound)
		{
			chooser = Random.Range(1, 100);
			int num = Random.Range(0, clips.Length - 1);
			AudioClip clip = clips[num];
			if (chooser <= chanceToPlay)
			{
				float pitch = Random.Range(pitchMin, pitchMax);
				audio.pitch = pitch;
				audio.PlayOneShot(clip);
			}
		}
		if (playAnimationOnBounce && animTimer <= 0f)
		{
			animator.Play(animationName);
			animator.PlayFromFrame(0);
			animTimer = animPause;
		}
		if (this.OnBounce != null)
		{
			this.OnBounce();
		}
	}

	private void OnCollisionStay2D(Collision2D col)
	{
	}

	public void StopBounce()
	{
		bouncing = false;
	}

	public void StartBounce()
	{
		bouncing = true;
	}

	public void SetBounceFactor(float value)
	{
		bounceFactor = value;
	}

	public void SetBounceAnimation(bool set)
	{
		playAnimationOnBounce = set;
	}
}

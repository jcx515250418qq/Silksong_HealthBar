using System;
using UnityEngine;

public class WispFireballLight : MonoBehaviour
{
	[SerializeField]
	private GameObject haze;

	[SerializeField]
	private ParticleSystem ptFire;

	[SerializeField]
	private AudioEvent shootAudio;

	[Space]
	[SerializeField]
	private GameObject explosion;

	[SerializeField]
	private CameraShakeTarget explodeCamShake;

	[SerializeField]
	private AudioEvent explodeAudio;

	[Space]
	[SerializeField]
	private EventRegister wispsEndEventRegister;

	private bool isActive;

	private Vector2 curveForce;

	private float explodeTimer;

	private float recycleTimer;

	private MeshRenderer meshRenderer;

	private Collider2D collider;

	private tk2dSpriteAnimator animator;

	private Rigidbody2D body;

	private DamageHero damager;

	private void Awake()
	{
		meshRenderer = GetComponent<MeshRenderer>();
		collider = GetComponent<Collider2D>();
		animator = GetComponent<tk2dSpriteAnimator>();
		body = GetComponent<Rigidbody2D>();
		damager = GetComponent<DamageHero>();
		damager.HeroDamaged += Explode;
		wispsEndEventRegister.ReceivedEvent += delegate
		{
			End();
			meshRenderer.enabled = false;
			recycleTimer = 3f;
		};
	}

	private void OnEnable()
	{
		meshRenderer.enabled = false;
		collider.enabled = false;
		isActive = false;
		explodeTimer = 0f;
		recycleTimer = 0f;
		explosion.SetActive(value: false);
		haze.SetActive(value: true);
	}

	private void FixedUpdate()
	{
		if (isActive)
		{
			body.AddForce(curveForce, ForceMode2D.Force);
			Vector2 linearVelocity = body.linearVelocity;
			float rotation = Mathf.Atan2(linearVelocity.y, linearVelocity.x) * (180f / MathF.PI);
			body.rotation = rotation;
		}
	}

	private void Update()
	{
		if (recycleTimer > 0f)
		{
			recycleTimer -= Time.deltaTime;
			if (recycleTimer <= 0f)
			{
				base.gameObject.Recycle();
			}
		}
		else if (explodeTimer > 0f)
		{
			explodeTimer -= Time.deltaTime;
			if (explodeTimer <= 0f)
			{
				Explode();
			}
		}
	}

	private void OnCollisionEnter2D()
	{
		Explode();
	}

	public void Fire(Vector2 initialVelocity, Vector2 newCurveForce)
	{
		body.linearVelocity = initialVelocity;
		curveForce = newCurveForce;
		shootAudio.SpawnAndPlayOneShot(base.transform.position);
		collider.enabled = true;
		meshRenderer.enabled = true;
		ptFire.Play(withChildren: true);
		animator.Play("Fire Instant");
		animator.AnimationCompleted = null;
		isActive = true;
		explodeTimer = 2f;
	}

	private void Explode()
	{
		End();
		explodeCamShake.DoShake(this);
		explodeAudio.SpawnAndPlayOneShot(base.transform.position);
		explosion.SetActive(value: true);
		animator.Play("Impact");
		animator.AnimationCompleted = OnAnimationCompleted;
	}

	private void End()
	{
		isActive = false;
		body.linearVelocity = Vector2.zero;
		collider.enabled = false;
		haze.SetActive(value: false);
		ptFire.Stop(withChildren: true);
	}

	private void OnAnimationCompleted(tk2dSpriteAnimator arg1, tk2dSpriteAnimationClip arg2)
	{
		meshRenderer.enabled = false;
		recycleTimer = 3f;
	}
}

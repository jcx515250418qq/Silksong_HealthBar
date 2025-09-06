using System;
using UnityEngine;

public class SpellGetOrb : MonoBehaviour
{
	public SpriteRenderer spriteRenderer;

	public Rigidbody2D rb2d;

	public GameObject trailObject;

	public TrailRenderer trailRenderer;

	public GameObject orbGetObject;

	public ParticleSystem ptIdle;

	public ParticleSystem ptZoom;

	public bool trackToHero;

	private float accel = 0.5f;

	private float accelMultiplier = 12f;

	private float stretchFactor = 2f;

	private float stretchMinX = 0.5f;

	private float stretchMaxY = 2f;

	private float scaleModifier;

	private float timer;

	private float idleTime;

	private float lerpTimer;

	private Vector3 startPosition;

	private Vector3 zoomPosition;

	private GameObject hero;

	private int state;

	private void Start()
	{
		base.transform.position = new Vector3(base.transform.position.x + UnityEngine.Random.Range(-0.1f, 0.1f), base.transform.position.y + UnityEngine.Random.Range(-0.2f, 0.2f), UnityEngine.Random.Range(-6f, 6f));
		idleTime = UnityEngine.Random.Range(2.5f, 4.5f);
		float num = UnityEngine.Random.Range(0.3f, 0.7f);
		base.transform.localScale = new Vector3(num, num);
	}

	private void OnEnable()
	{
		if (trackToHero)
		{
			GameManager instance = GameManager.instance;
			hero = instance.hero_ctrl.gameObject;
			if (hero == null)
			{
				hero = GameObject.FindWithTag("Player");
			}
		}
		startPosition = base.transform.position;
		float num = UnityEngine.Random.Range(3f, 20f);
		float num2 = UnityEngine.Random.Range(0f, 360f);
		float x = num * Mathf.Cos(num2 * (MathF.PI / 180f)) * 1.25f;
		float y = num * Mathf.Sin(num2 * (MathF.PI / 180f));
		rb2d.linearVelocity = new Vector2(x, y);
	}

	private void Update()
	{
		if (state == 0)
		{
			if (timer >= idleTime)
			{
				rb2d.linearVelocity = new Vector3(0f, 0f, 0f);
				trailObject.SetActive(value: true);
				zoomPosition = base.transform.position;
				state = 1;
				ptIdle.Stop();
				ptZoom.Play();
				if (hero != null && trackToHero)
				{
					startPosition = new Vector3(hero.transform.position.x, hero.transform.position.y - 0.5f, hero.transform.position.z);
				}
			}
			else
			{
				timer += Time.deltaTime;
			}
		}
		else if (state == 1)
		{
			trailRenderer.startWidth = base.transform.localScale.x;
			base.transform.position = Vector3.Lerp(zoomPosition, startPosition, lerpTimer);
			lerpTimer += Time.deltaTime * accel;
			accel += Time.deltaTime * accelMultiplier;
			if (lerpTimer >= 1f)
			{
				Collect();
			}
		}
	}

	private void FaceAngle()
	{
		Vector2 linearVelocity = rb2d.linearVelocity;
		float z = Mathf.Atan2(linearVelocity.y, linearVelocity.x) * (180f / MathF.PI);
		base.transform.localEulerAngles = new Vector3(0f, 0f, z);
	}

	private void ProjectileSquash()
	{
		float num = 1f - rb2d.linearVelocity.magnitude * stretchFactor * 0.01f;
		float num2 = 1f + rb2d.linearVelocity.magnitude * stretchFactor * 0.01f;
		if (num2 < stretchMinX)
		{
			num2 = stretchMinX;
		}
		if (num > stretchMaxY)
		{
			num = stretchMaxY;
		}
		num *= scaleModifier;
		num2 *= scaleModifier;
		base.transform.localScale = new Vector3(num2, num, base.transform.localScale.z);
	}

	private void Collect()
	{
		state = 2;
		rb2d.linearVelocity = new Vector3(0f, 0f, 0f);
		spriteRenderer.enabled = false;
		trailObject.SetActive(value: false);
		orbGetObject.SetActive(value: true);
		ptZoom.Stop();
	}
}

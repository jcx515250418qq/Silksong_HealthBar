using System;
using System.Collections;
using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public class EnemyBullet : MonoBehaviour
{
	public float scaleMin = 1.15f;

	public float scaleMax = 1.45f;

	private float scale;

	[Space]
	public float stretchFactor = 1.2f;

	public float stretchMinX = 0.75f;

	public float stretchMaxY = 1.75f;

	[Space]
	public float lingerTime;

	public GameObject idleEffects;

	public GameObject impactEffects;

	public ParticleSystem idleParticle;

	[Space]
	public AudioSource audioSourcePrefab;

	public AudioEvent impactSound;

	public RandomAudioClipTable impactAudioClipTable;

	[Space]
	public bool heroBullet;

	public GameObject damageCollider;

	public bool noImpactRotation;

	public bool dontUseAnimator;

	private bool active;

	private Rigidbody2D body;

	private tk2dSpriteAnimator anim;

	private tk2dSpriteAnimationClip impactAnim;

	private Collider2D col;

	private void Awake()
	{
		body = GetComponent<Rigidbody2D>();
		anim = GetComponent<tk2dSpriteAnimator>();
		col = GetComponent<Collider2D>();
		if ((bool)anim)
		{
			impactAnim = anim.GetClipByName("Impact");
		}
	}

	private void OnEnable()
	{
		active = true;
		scale = UnityEngine.Random.Range(scaleMin, scaleMax);
		col.enabled = true;
		if ((bool)damageCollider)
		{
			damageCollider.SetActive(value: true);
		}
		body.isKinematic = false;
		body.linearVelocity = Vector2.zero;
		body.angularVelocity = 0f;
		if ((bool)idleParticle)
		{
			idleParticle.Play();
		}
		if (!dontUseAnimator && (bool)anim)
		{
			anim.Play("Idle");
		}
		MeshRenderer component = GetComponent<MeshRenderer>();
		if ((bool)component)
		{
			component.enabled = true;
		}
		if ((bool)idleEffects)
		{
			idleEffects.SetActive(value: true);
		}
		if ((bool)impactEffects)
		{
			impactEffects.SetActive(value: false);
		}
	}

	private void Update()
	{
		if (!active)
		{
			return;
		}
		float rotation = Mathf.Atan2(body.linearVelocity.y, body.linearVelocity.x) * (180f / MathF.PI);
		base.transform.SetRotation2D(rotation);
		if (stretchFactor != 1f)
		{
			float num = 1f - body.linearVelocity.magnitude * stretchFactor * 0.01f;
			float num2 = 1f + body.linearVelocity.magnitude * stretchFactor * 0.01f;
			if (num2 < stretchMinX)
			{
				num2 = stretchMinX;
			}
			if (num > stretchMaxY)
			{
				num = stretchMaxY;
			}
			num *= scale;
			num2 *= scale;
			base.transform.localScale = new Vector3(num2, num, base.transform.localScale.z);
		}
	}

	private void OnCollisionEnter2D(Collision2D collision)
	{
		_ = collision.gameObject;
		if (active)
		{
			active = false;
			StartCoroutine(Collision(collision.GetSafeContact().Normal, doRotation: true));
		}
	}

	private void OnTriggerEnter2D(Collider2D collision)
	{
		if (active && !heroBullet && collision.tag == "HeroBox")
		{
			active = false;
			StartCoroutine(Collision(Vector2.zero, doRotation: false));
		}
		if (active)
		{
			GameObject gameObject = collision.gameObject;
			if (gameObject.layer == 25 && gameObject.CompareTag("Water Surface"))
			{
				active = false;
				Break();
			}
		}
	}

	public void OrbitShieldHit(Transform shield)
	{
		if (active)
		{
			active = false;
			Vector2 normal = base.transform.position - shield.position;
			normal.Normalize();
			StartCoroutine(Collision(normal, doRotation: true));
		}
	}

	public void Break()
	{
		StartCoroutine(Collision(Vector2.zero, doRotation: false));
	}

	public void TornadoEffect()
	{
		Break();
	}

	private IEnumerator Collision(Vector2 normal, bool doRotation)
	{
		MeshRenderer meshRenderer = GetComponent<MeshRenderer>();
		base.transform.localScale = new Vector3(scale, scale, base.transform.localScale.z);
		body.isKinematic = true;
		body.linearVelocity = Vector2.zero;
		body.angularVelocity = 0f;
		float animTime = 0f;
		if (impactAnim != null && !dontUseAnimator)
		{
			anim.Play(impactAnim);
			animTime = (float)(impactAnim.frames.Length - 1) / impactAnim.fps;
		}
		else if ((bool)meshRenderer)
		{
			meshRenderer.enabled = false;
		}
		if (!heroBullet && !noImpactRotation)
		{
			if (!doRotation || (normal.y >= 0.75f && Mathf.Abs(normal.x) < 0.5f))
			{
				base.transform.SetRotation2D(0f);
			}
			else if (normal.y <= 0.75f && Mathf.Abs(normal.x) < 0.5f)
			{
				base.transform.SetRotation2D(180f);
			}
			else if (normal.x >= 0.75f && Mathf.Abs(normal.y) < 0.5f)
			{
				base.transform.SetRotation2D(270f);
			}
			else if (normal.x <= 0.75f && Mathf.Abs(normal.y) < 0.5f)
			{
				base.transform.SetRotation2D(90f);
			}
		}
		if ((bool)impactAudioClipTable)
		{
			impactAudioClipTable.SpawnAndPlayOneShot(base.transform.position);
		}
		else
		{
			impactSound.SpawnAndPlayOneShot(audioSourcePrefab, base.transform.position);
		}
		if ((bool)idleParticle)
		{
			idleParticle.Stop();
		}
		if ((bool)idleEffects)
		{
			idleEffects.SetActive(value: false);
		}
		if ((bool)impactEffects)
		{
			impactEffects.SetActive(value: true);
		}
		yield return null;
		col.enabled = false;
		if ((bool)damageCollider)
		{
			damageCollider.SetActive(value: false);
		}
		yield return new WaitForSeconds(animTime);
		if ((bool)meshRenderer)
		{
			meshRenderer.enabled = false;
		}
		yield return new WaitForSeconds(lingerTime);
		base.gameObject.Recycle();
	}
}

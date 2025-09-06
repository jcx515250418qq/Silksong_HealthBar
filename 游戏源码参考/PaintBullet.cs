using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public class PaintBullet : MonoBehaviour
{
	public float scaleMin = 1.15f;

	public float scaleMax = 1.45f;

	private float scale;

	[Space]
	public float stretchFactor = 1.2f;

	public float stretchMinX = 0.75f;

	public float stretchMaxY = 1.75f;

	[Space]
	public AudioSource audioSourcePrefab;

	public List<AudioClip> splatClips;

	[Space]
	public ParticleSystem impactParticle;

	public ParticleSystem trailParticle;

	public GameObject splatEffect;

	public tk2dSprite splatEffectSprite;

	[Space]
	public Color colourBlue;

	public Color colourRed;

	public Color colourPink;

	private bool active;

	public int colour;

	[Space]
	private List<SpriteRenderer> splatList;

	private int chance;

	private bool painting;

	private Rigidbody2D body;

	private Collider2D col;

	private SpriteRenderer sprite;

	private void Awake()
	{
		body = GetComponent<Rigidbody2D>();
		col = GetComponent<Collider2D>();
		sprite = GetComponent<SpriteRenderer>();
	}

	private void OnEnable()
	{
		active = true;
		scale = UnityEngine.Random.Range(scaleMin, scaleMax);
		sprite.enabled = true;
		col.enabled = true;
		trailParticle.Play();
		body.isKinematic = false;
		body.linearVelocity = Vector2.zero;
		body.angularVelocity = 0f;
		splatList = new List<SpriteRenderer>();
		if (colour == 0)
		{
			SetBlue();
		}
		else if (colour == 1)
		{
			SetRed();
		}
		else if (colour == 2)
		{
			SetPink();
		}
	}

	private void Update()
	{
		if (active)
		{
			float rotation = Mathf.Atan2(body.linearVelocity.y, body.linearVelocity.x) * (180f / MathF.PI);
			base.transform.SetRotation2D(rotation);
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

	private void OnTriggerEnter2D(Collider2D collision)
	{
		if (active && collision.tag == "HeroBox")
		{
			active = false;
			StartCoroutine(Collision(Vector2.zero, doRotation: false));
		}
		if (active && collision.tag == "Extra Tag")
		{
			splatList.Add(collision.gameObject.GetComponent<SpriteRenderer>());
		}
	}

	private void OnTriggerExit2D(Collider2D collision)
	{
		if (active && collision.tag == "Extra Tag")
		{
			splatList.Remove(collision.gameObject.GetComponent<SpriteRenderer>());
		}
	}

	private void OnCollisionEnter2D(Collision2D collision)
	{
		if (active)
		{
			active = false;
			StartCoroutine(Collision(collision.GetSafeContact().Normal, doRotation: true));
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

	private IEnumerator Collision(Vector2 normal, bool doRotation)
	{
		base.transform.localScale = new Vector3(scale, scale, base.transform.localScale.z);
		body.isKinematic = true;
		body.linearVelocity = Vector2.zero;
		body.angularVelocity = 0f;
		sprite.enabled = false;
		impactParticle.Play();
		trailParticle.Stop();
		splatEffect.SetActive(value: true);
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
		AudioClip clip = splatClips[UnityEngine.Random.Range(0, splatClips.Count - 1)];
		UnityEngine.Random.Range(0.9f, 1.1f);
		audioSourcePrefab.PlayOneShot(clip);
		chance = 100;
		painting = true;
		foreach (SpriteRenderer splat in splatList)
		{
			if (painting)
			{
				if (UnityEngine.Random.Range(1, 100) <= chance)
				{
					splat.color = sprite.color;
					chance /= 2;
				}
				else
				{
					painting = false;
				}
			}
		}
		yield return null;
		col.enabled = false;
		yield return new WaitForSeconds(1f);
		base.gameObject.Recycle();
	}

	public void SetColourBlue()
	{
		colour = 0;
		SetBlue();
	}

	public void SetColourRed()
	{
		colour = 1;
		SetRed();
	}

	public void SetBlue()
	{
		sprite.color = colourBlue;
		splatEffectSprite.color = colourBlue;
		ParticleSystem.MainModule main = impactParticle.main;
		main.startColor = colourBlue;
		ParticleSystem.MainModule main2 = trailParticle.main;
		main2.startColor = colourBlue;
	}

	public void SetRed()
	{
		sprite.color = colourRed;
		splatEffectSprite.color = colourRed;
		ParticleSystem.MainModule main = impactParticle.main;
		main.startColor = colourRed;
		ParticleSystem.MainModule main2 = trailParticle.main;
		main2.startColor = colourRed;
	}

	public void SetPink()
	{
		sprite.color = colourPink;
		splatEffectSprite.color = colourPink;
		ParticleSystem.MainModule main = impactParticle.main;
		main.startColor = colourPink;
		ParticleSystem.MainModule main2 = trailParticle.main;
		main2.startColor = colourPink;
	}
}

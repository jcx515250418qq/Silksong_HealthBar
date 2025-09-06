using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class CoalBurn : MonoBehaviour
{
	[SerializeField]
	private GameObject burningEffectsParent;

	[SerializeField]
	private GameObject burnUpEffectsParent;

	[SerializeField]
	private float burnDelay = 2f;

	[SerializeField]
	private float burnTime = 1.5f;

	[SerializeField]
	private float unburnTime = 0.5f;

	[SerializeField]
	private Color burnColor = Color.black;

	[Space]
	[SerializeField]
	private UnityEvent onBurnedUp;

	private bool inCoalRegion;

	private bool hasBurnedUp;

	private Coroutine burnRoutine;

	private Color initialSpriteColor;

	private ParticleSystem[] burningParticles;

	private ParticleSystem[] burnUpParticles;

	private tk2dSprite tk2dSprite;

	private SpriteRenderer spriteRenderer;

	private Rigidbody2D body;

	private Collider2D collider;

	private bool playingParticle;

	private HashSet<Collider2D> enteredColliders = new HashSet<Collider2D>();

	private Color SpriteColour
	{
		get
		{
			if ((bool)tk2dSprite)
			{
				return tk2dSprite.color;
			}
			if ((bool)spriteRenderer)
			{
				return spriteRenderer.color;
			}
			return Color.clear;
		}
		set
		{
			if ((bool)tk2dSprite)
			{
				tk2dSprite.color = value;
			}
			else if ((bool)spriteRenderer)
			{
				spriteRenderer.color = value;
			}
		}
	}

	private void Awake()
	{
		tk2dSprite = GetComponent<tk2dSprite>();
		if (!tk2dSprite)
		{
			spriteRenderer = GetComponent<SpriteRenderer>();
		}
		initialSpriteColor = SpriteColour;
		if ((bool)burningEffectsParent)
		{
			burningParticles = burningEffectsParent.GetComponentsInChildren<ParticleSystem>(includeInactive: true);
			ParticleSystem[] array = burningParticles;
			foreach (ParticleSystem obj in array)
			{
				ParticleSystem.MainModule main = obj.main;
				main.playOnAwake = false;
				obj.Stop(withChildren: true, ParticleSystemStopBehavior.StopEmittingAndClear);
			}
			burningEffectsParent.SetActive(value: true);
		}
		else
		{
			burningParticles = Array.Empty<ParticleSystem>();
		}
		if ((bool)burnUpEffectsParent)
		{
			burnUpParticles = burnUpEffectsParent.GetComponentsInChildren<ParticleSystem>(includeInactive: true);
			ParticleSystem[] array = burnUpParticles;
			foreach (ParticleSystem obj2 in array)
			{
				ParticleSystem.MainModule main2 = obj2.main;
				main2.playOnAwake = false;
				obj2.Stop(withChildren: true, ParticleSystemStopBehavior.StopEmittingAndClear);
			}
			burnUpEffectsParent.SetActive(value: true);
		}
		else
		{
			burnUpParticles = Array.Empty<ParticleSystem>();
		}
		body = GetComponent<Rigidbody2D>();
		collider = GetComponent<Collider2D>();
	}

	private void OnDisable()
	{
		if (burnRoutine != null)
		{
			StopCoroutine(burnRoutine);
			burnRoutine = null;
		}
		StopBurning();
		SpriteColour = initialSpriteColor;
		hasBurnedUp = false;
		enteredColliders.Clear();
		inCoalRegion = false;
	}

	private void OnTriggerEnter2D(Collider2D collision)
	{
		if (!hasBurnedUp && IsCoalRegion(collision))
		{
			enteredColliders.Add(collision);
			inCoalRegion = true;
			if (burnRoutine == null)
			{
				burnRoutine = StartCoroutine(BurnMonitor());
			}
		}
	}

	private void OnTriggerExit2D(Collider2D collision)
	{
		if (!hasBurnedUp && inCoalRegion && IsCoalRegion(collision))
		{
			enteredColliders.Remove(collision);
			if (enteredColliders.Count == 0)
			{
				inCoalRegion = false;
			}
		}
	}

	private bool IsCoalRegion(Collider2D collision)
	{
		if (collision.CompareTag("Geo"))
		{
			return false;
		}
		GameObject gameObject = collision.gameObject;
		if ((bool)gameObject.GetComponent<GlowResponseCoal>())
		{
			return true;
		}
		if (gameObject.layer == 17)
		{
			return gameObject.CompareTag("Coal");
		}
		return false;
	}

	private IEnumerator BurnMonitor()
	{
		while (true)
		{
			if (!inCoalRegion)
			{
				yield return null;
				continue;
			}
			float elapsed = 0f;
			while (true)
			{
				if (elapsed < burnDelay)
				{
					if (!inCoalRegion)
					{
						break;
					}
					yield return null;
					elapsed += Time.deltaTime;
					continue;
				}
				elapsed = 0f;
				while (inCoalRegion)
				{
					StartBurning();
					SetBurnTime(elapsed);
					while (elapsed < burnTime)
					{
						if (!inCoalRegion)
						{
							StopBurning();
							break;
						}
						yield return null;
						elapsed += Time.deltaTime;
						SetBurnTime(elapsed);
					}
					if (elapsed >= burnTime)
					{
						BurnUp();
						for (elapsed = 0f; elapsed < 0.5f; elapsed += Time.deltaTime)
						{
							SpriteColour = Color.Lerp(burnColor, Color.clear, elapsed / 0.5f);
							yield return null;
						}
						SpriteColour = Color.clear;
						while (true)
						{
							bool flag = false;
							ParticleSystem[] array = burnUpParticles;
							for (int i = 0; i < array.Length; i++)
							{
								if (array[i].IsAlive())
								{
									flag = true;
									break;
								}
							}
							if (!flag)
							{
								break;
							}
							yield return null;
						}
						burnRoutine = null;
						yield break;
					}
					SetBurnTime(elapsed);
					while (elapsed > 0f && !inCoalRegion)
					{
						yield return null;
						elapsed -= Time.deltaTime;
						SetBurnTime(elapsed);
					}
				}
				StopBurning();
				break;
			}
		}
	}

	private void StartBurning()
	{
		playingParticle = true;
		ParticleSystem[] array = burningParticles;
		for (int i = 0; i < array.Length; i++)
		{
			array[i].Play(withChildren: true);
		}
	}

	private void StopBurning()
	{
		if (playingParticle)
		{
			playingParticle = false;
			ParticleSystem[] array = burningParticles;
			for (int i = 0; i < array.Length; i++)
			{
				array[i].Stop(withChildren: true, ParticleSystemStopBehavior.StopEmitting);
			}
		}
	}

	private void BurnUp()
	{
		StopBurning();
		hasBurnedUp = true;
		ParticleSystem[] array = burnUpParticles;
		for (int i = 0; i < array.Length; i++)
		{
			array[i].Play(withChildren: true);
		}
		collider.enabled = false;
		body.linearVelocity = Vector2.zero;
		body.isKinematic = true;
		onBurnedUp.Invoke();
		base.gameObject.GetComponent<IBurnable>()?.BurnUp();
	}

	private void SetBurnTime(float time)
	{
		if (time < 0f)
		{
			time = 0f;
		}
		else if (time > burnTime)
		{
			time = burnTime;
		}
		float t = time / burnTime;
		SpriteColour = Color.Lerp(initialSpriteColor, burnColor, t);
	}
}

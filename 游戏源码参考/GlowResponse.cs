using System;
using System.Collections;
using System.Collections.Generic;
using GlobalSettings;
using TeamCherry.NestedFadeGroup;
using UnityEngine;

public class GlowResponse : DebugDrawColliderRuntimeAdder
{
	[SerializeField]
	[HideInInspector]
	[Obsolete]
	private SpriteRenderer fadeSprite;

	[SerializeField]
	private List<SpriteRenderer> FadeSprites = new List<SpriteRenderer>();

	[SerializeField]
	private NestedFadeGroupBase fadeGroup;

	[SerializeField]
	private NestedFadeGroupBase fadeGroupStable;

	[SerializeField]
	private float fadeTime = 0.5f;

	[SerializeField]
	private float fadeTo = 1f;

	[SerializeField]
	private bool invert;

	[SerializeField]
	private bool useDefaultPulse = true;

	[SerializeField]
	private ParticleSystem particles;

	private float currentAlpha;

	public AudioSource audioPlayerPrefab;

	public AudioEventRandom soundEffect;

	public RandomAudioClipTable randomAudioClipTable;

	private Coroutine fadeRoutine;

	private readonly HashSet<GameObject> insideObjects = new HashSet<GameObject>();

	private void OnValidate()
	{
		if ((bool)fadeSprite)
		{
			FadeSprites.Add(fadeSprite);
			fadeSprite = null;
		}
	}

	protected override void Awake()
	{
		base.Awake();
		OnValidate();
	}

	private void Start()
	{
		currentAlpha = (invert ? 1 : 0);
		foreach (SpriteRenderer fadeSprite in FadeSprites)
		{
			if ((bool)fadeSprite)
			{
				Color color = fadeSprite.color;
				color.a = currentAlpha;
				fadeSprite.color = color;
			}
		}
		if ((bool)fadeGroup)
		{
			fadeGroup.AlphaSelf = currentAlpha;
		}
		if ((bool)fadeGroupStable)
		{
			fadeGroupStable.AlphaSelf = currentAlpha;
		}
	}

	private void OnDisable()
	{
		insideObjects.Clear();
	}

	private void OnTriggerEnter2D(Collider2D collision)
	{
		if (insideObjects.Add(collision.gameObject) && insideObjects.Count == 1)
		{
			if ((bool)particles)
			{
				particles.Play();
			}
			Vector3 position = base.transform.position;
			position.z = 0f;
			if ((bool)randomAudioClipTable)
			{
				randomAudioClipTable.SpawnAndPlayOneShot(audioPlayerPrefab, position);
			}
			else
			{
				soundEffect.SpawnAndPlayOneShot(audioPlayerPrefab, position);
			}
			FadeTo(invert ? 0f : fadeTo);
		}
	}

	private void OnTriggerExit2D(Collider2D collision)
	{
		if (insideObjects.Remove(collision.gameObject) && insideObjects.Count == 0)
		{
			if ((bool)particles)
			{
				particles.Stop(withChildren: true, ParticleSystemStopBehavior.StopEmitting);
			}
			FadeTo(invert ? fadeTo : 0f);
		}
	}

	private void FadeTo(float alpha)
	{
		if (fadeRoutine != null)
		{
			StopCoroutine(fadeRoutine);
		}
		if (base.gameObject.activeInHierarchy)
		{
			fadeRoutine = StartCoroutine(Fade(alpha));
		}
	}

	private IEnumerator Fade(float toAlpha)
	{
		float initialAlpha = currentAlpha;
		for (float elapsed = 0f; elapsed < fadeTime; elapsed += Time.deltaTime)
		{
			currentAlpha = Mathf.Lerp(initialAlpha, toAlpha, elapsed / fadeTime);
			UpdateGlow();
			yield return null;
		}
		currentAlpha = toAlpha;
		UpdateGlow();
		if (useDefaultPulse && currentAlpha > Mathf.Epsilon)
		{
			fadeRoutine = StartCoroutine(UpdateGlowLoop());
		}
	}

	private IEnumerator UpdateGlowLoop()
	{
		while (true)
		{
			UpdateGlow();
			yield return null;
		}
	}

	public void FadeEnd()
	{
		FadeTo(0f);
		if ((bool)particles)
		{
			particles.Stop(withChildren: true, ParticleSystemStopBehavior.StopEmitting);
		}
		CircleCollider2D component = GetComponent<CircleCollider2D>();
		if (component != null)
		{
			component.enabled = false;
		}
	}

	private void UpdateGlow()
	{
		float num;
		if (useDefaultPulse)
		{
			AnimationCurve glowResponsePulseCurve = Effects.GlowResponsePulseCurve;
			float glowResponsePulseDuration = Effects.GlowResponsePulseDuration;
			float glowResponsePulseFrameRate = Effects.GlowResponsePulseFrameRate;
			float time = Mathf.Floor(Time.time * glowResponsePulseFrameRate + 0.5f) / glowResponsePulseFrameRate % glowResponsePulseDuration / glowResponsePulseDuration;
			num = glowResponsePulseCurve.Evaluate(time);
		}
		else
		{
			num = 1f;
		}
		foreach (SpriteRenderer fadeSprite in FadeSprites)
		{
			Color color = fadeSprite.color;
			color.a = currentAlpha * num;
			fadeSprite.color = color;
		}
		if ((bool)fadeGroup)
		{
			fadeGroup.AlphaSelf = currentAlpha * num;
		}
		if ((bool)fadeGroupStable)
		{
			fadeGroupStable.AlphaSelf = currentAlpha;
		}
	}

	public override void AddDebugDrawComponent()
	{
		DebugDrawColliderRuntime.AddOrUpdate(base.gameObject, DebugDrawColliderRuntime.ColorType.Region);
	}
}

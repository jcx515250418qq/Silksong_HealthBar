using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GlowResponseCoal : DebugDrawColliderRuntimeAdder
{
	[HideInInspector]
	[SerializeField]
	private SpriteRenderer fadeSprite;

	public List<SpriteRenderer> FadeSprites = new List<SpriteRenderer>();

	public float fadeTime = 0.5f;

	public float fadeTo = 1f;

	public ParticleSystem particles;

	public AudioSource audioPlayerPrefab;

	public AudioEventRandom soundEffect;

	private bool glowing;

	private HashSet<GameObject> enteredObjects = new HashSet<GameObject>();

	private Dictionary<SpriteRenderer, Coroutine> fadeRoutines = new Dictionary<SpriteRenderer, Coroutine>();

	private Coroutine fadeRoutine;

	private List<Color> fadeColors = new List<Color>();

	private void OnValidate()
	{
		HandleUpgrade();
	}

	protected override void Awake()
	{
		base.Awake();
		HandleUpgrade();
	}

	private void HandleUpgrade()
	{
		if ((bool)fadeSprite)
		{
			FadeSprites.Add(fadeSprite);
			fadeSprite = null;
		}
	}

	private void Start()
	{
		foreach (SpriteRenderer fadeSprite in FadeSprites)
		{
			if ((bool)fadeSprite)
			{
				Color color = fadeSprite.color;
				color.a = 0f;
				fadeSprite.color = color;
			}
		}
	}

	private void OnTriggerEnter2D(Collider2D collision)
	{
		GameObject item = collision.gameObject;
		if (ShouldCollide(item))
		{
			enteredObjects.Add(item);
			if (!glowing)
			{
				StartGlow();
			}
		}
	}

	private static bool ShouldCollide(GameObject gameObject)
	{
		if (gameObject.layer == 9 || gameObject.layer == 11 || gameObject.layer == 26 || gameObject.layer == 20 || gameObject.CompareTag("Geo"))
		{
			return true;
		}
		return gameObject.GetComponent<CoalBurn>() != null;
	}

	private void OnTriggerExit2D(Collider2D collision)
	{
		GameObject item = collision.gameObject;
		if (enteredObjects.Remove(item) && glowing && enteredObjects.Count <= 0)
		{
			EndGlow();
		}
	}

	private void StartGlow()
	{
		glowing = true;
		if ((bool)particles)
		{
			particles.Play();
		}
		Vector3 position = base.transform.position;
		position.z = 0f;
		soundEffect.SpawnAndPlayOneShot(audioPlayerPrefab, position);
		FadeTo(fadeTo);
	}

	private void EndGlow()
	{
		glowing = false;
		if ((bool)particles)
		{
			particles.Stop(withChildren: true, ParticleSystemStopBehavior.StopEmitting);
		}
		FadeTo(0f);
	}

	private void FadeTo(float alpha)
	{
		FadeSprites.RemoveAll((SpriteRenderer o) => o == null);
		if (FadeSprites.Count > 0)
		{
			if (fadeRoutine != null)
			{
				StopCoroutine(fadeRoutine);
			}
			fadeRoutine = StartCoroutine(Fade(alpha));
		}
	}

	private IEnumerator Fade(float toAlpha, SpriteRenderer sprite)
	{
		float elapsed = 0f;
		Color initialColor = (sprite ? sprite.color : Color.white);
		Color currentColor = initialColor;
		_ = toAlpha;
		for (; elapsed < fadeTime; elapsed += Time.deltaTime)
		{
			if ((bool)sprite)
			{
				currentColor.a = Mathf.Lerp(initialColor.a, toAlpha, elapsed / fadeTime);
				sprite.color = currentColor;
			}
			yield return null;
		}
		if ((bool)sprite)
		{
			currentColor.a = toAlpha;
			sprite.color = currentColor;
		}
	}

	private IEnumerator Fade(float toAlpha)
	{
		float elapsed = 0f;
		fadeColors.Clear();
		fadeColors.Capacity = FadeSprites.Count;
		foreach (SpriteRenderer fadeSprite in FadeSprites)
		{
			fadeColors.Add(fadeSprite.color);
		}
		if (fadeTime > 0f)
		{
			float rate = 1f / fadeTime;
			for (; elapsed < fadeTime; elapsed += Time.deltaTime)
			{
				for (int num = FadeSprites.Count - 1; num >= 0; num--)
				{
					SpriteRenderer spriteRenderer = FadeSprites[num];
					if (spriteRenderer == null)
					{
						FadeSprites.RemoveAt(num);
						fadeColors.RemoveAt(num);
					}
					else
					{
						Color color = fadeColors[num];
						color.a = Mathf.MoveTowards(color.a, toAlpha, rate * Time.deltaTime);
						spriteRenderer.color = color;
						fadeColors[num] = color;
					}
				}
				yield return null;
			}
		}
		for (int num2 = FadeSprites.Count - 1; num2 >= 0; num2--)
		{
			SpriteRenderer spriteRenderer2 = FadeSprites[num2];
			if (spriteRenderer2 == null)
			{
				FadeSprites.RemoveAt(num2);
				fadeColors.RemoveAt(num2);
			}
			else
			{
				Color color2 = fadeColors[num2];
				color2.a = toAlpha;
				spriteRenderer2.color = color2;
			}
		}
		fadeColors.Clear();
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

	public override void AddDebugDrawComponent()
	{
		DebugDrawColliderRuntime.AddOrUpdate(base.gameObject, DebugDrawColliderRuntime.ColorType.Region);
	}
}

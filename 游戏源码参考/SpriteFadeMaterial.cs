using System;
using System.Collections;
using UnityEngine;

public class SpriteFadeMaterial : MonoBehaviour
{
	public Material initialMaterial;

	public float fadeBackDuration = 1f;

	private SpriteRenderer[] sprites;

	private Coroutine fadeRoutine;

	private Action onFadeEnd;

	private void Awake()
	{
		sprites = GetComponentsInChildren<SpriteRenderer>();
	}

	private void Start()
	{
		SpriteRenderer[] array = sprites;
		for (int i = 0; i < array.Length; i++)
		{
			array[i].sharedMaterial = initialMaterial;
		}
	}

	public void FadeBack()
	{
		if (fadeRoutine != null)
		{
			StopCoroutine(fadeRoutine);
		}
		if (onFadeEnd != null)
		{
			onFadeEnd();
			onFadeEnd = null;
		}
		fadeRoutine = StartCoroutine(FadeBackRoutine());
	}

	private IEnumerator FadeBackRoutine()
	{
		SpriteRenderer[] newSprites = new SpriteRenderer[sprites.Length];
		for (int i = 0; i < newSprites.Length; i++)
		{
			newSprites[i] = UnityEngine.Object.Instantiate(sprites[i], sprites[i].transform.parent);
			newSprites[i].transform.Translate(new Vector3(0f, 0f, -0.001f), Space.World);
			newSprites[i].gameObject.name = sprites[i].gameObject.name;
			newSprites[i].sharedMaterial = initialMaterial;
			newSprites[i].color = Color.clear;
		}
		onFadeEnd = delegate
		{
			SpriteRenderer[] array = newSprites;
			for (int j = 0; j < array.Length; j++)
			{
				array[j].color = Color.white;
			}
			for (int k = 0; k < sprites.Length; k++)
			{
				UnityEngine.Object.DestroyImmediate(sprites[k].gameObject);
			}
			Animator component = GetComponent<Animator>();
			if ((bool)component)
			{
				component.Rebind();
			}
			sprites = newSprites;
		};
		for (float elapsed = 0f; elapsed <= fadeBackDuration; elapsed += Time.deltaTime)
		{
			SpriteRenderer[] array2 = newSprites;
			for (int l = 0; l < array2.Length; l++)
			{
				array2[l].color = Color.Lerp(Color.clear, Color.white, elapsed / fadeBackDuration);
			}
			yield return null;
		}
		if (onFadeEnd != null)
		{
			onFadeEnd();
			onFadeEnd = null;
		}
		fadeRoutine = null;
	}
}

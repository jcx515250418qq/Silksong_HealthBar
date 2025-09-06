using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class CharacterLightDust : MonoBehaviour
{
	[SerializeField]
	private SpriteRenderer fgSprite;

	[SerializeField]
	private SpriteRenderer bgSprite;

	[SerializeField]
	private Color defaultColor = new Color(1f, 1f, 1f, 0f);

	private SpriteRenderer[] spriteRenderers;

	private Color[] baseColors;

	private Coroutine fadeRoutine;

	private float fadeDuration;

	private Color currentColor;

	private readonly List<SceneAppearanceRegion> insideRegions = new List<SceneAppearanceRegion>();

	private GameManager gm;

	private Material fgMaterial;

	private Material bgMaterial;

	private void Awake()
	{
		if ((bool)fgSprite && (bool)bgSprite)
		{
			spriteRenderers = new SpriteRenderer[2] { fgSprite, bgSprite };
		}
		else if ((bool)fgSprite)
		{
			spriteRenderers = new SpriteRenderer[1] { fgSprite };
		}
		else if ((bool)bgSprite)
		{
			spriteRenderers = new SpriteRenderer[1] { bgSprite };
		}
		else
		{
			spriteRenderers = Array.Empty<SpriteRenderer>();
		}
		if ((bool)fgSprite)
		{
			fgMaterial = fgSprite.sharedMaterial;
		}
		if ((bool)bgSprite)
		{
			bgMaterial = bgSprite.sharedMaterial;
		}
		baseColors = new Color[spriteRenderers.Length];
		for (int i = 0; i < spriteRenderers.Length; i++)
		{
			SpriteRenderer spriteRenderer = spriteRenderers[i];
			baseColors[i] = spriteRenderer.color;
		}
		UpdateColor(forceImmediate: true);
		gm = GameManager.instance;
		gm.NextSceneWillActivate += RevertMaterials;
	}

	private void OnDestroy()
	{
		if ((bool)gm)
		{
			gm.NextSceneWillActivate -= RevertMaterials;
			gm = null;
		}
	}

	public void AddInside(SceneAppearanceRegion region, bool forceImmediate)
	{
		insideRegions.AddIfNotPresent(region);
		UpdateColor(forceImmediate);
	}

	public void RemoveInside(SceneAppearanceRegion region, bool forceImmediate)
	{
		insideRegions.Remove(region);
		UpdateColor(forceImmediate);
	}

	private void UpdateColor(bool forceImmediate)
	{
		if (fadeRoutine != null)
		{
			StopCoroutine(fadeRoutine);
		}
		Color targetColor;
		bool flag;
		if (insideRegions.Count == 0)
		{
			if (forceImmediate)
			{
				RevertMaterials();
				SetColor(defaultColor);
				fadeDuration = 0f;
				return;
			}
			targetColor = defaultColor;
			flag = true;
		}
		else
		{
			SceneAppearanceRegion sceneAppearanceRegion = insideRegions.Last();
			targetColor = sceneAppearanceRegion.CharacterLightDustColor;
			fadeDuration = sceneAppearanceRegion.FadeDuration;
			SceneAppearanceRegion.DustMaterials characterLightDustMaterials = sceneAppearanceRegion.CharacterLightDustMaterials;
			if ((bool)fgSprite)
			{
				fgSprite.sharedMaterial = characterLightDustMaterials.Foreground;
			}
			if ((bool)bgSprite)
			{
				bgSprite.sharedMaterial = characterLightDustMaterials.Background;
			}
			flag = false;
		}
		Color startColor = currentColor;
		if (fadeDuration > 0f && base.isActiveAndEnabled)
		{
			fadeRoutine = this.StartTimerRoutine(0f, fadeDuration, delegate(float time)
			{
				SetColor(Color.Lerp(startColor, targetColor, time));
			}, null, flag ? new Action(RevertMaterials) : null);
		}
		else
		{
			SetColor(targetColor);
		}
	}

	private void RevertMaterials()
	{
		if ((bool)fgSprite)
		{
			fgSprite.sharedMaterial = fgMaterial;
		}
		if ((bool)bgSprite)
		{
			bgSprite.sharedMaterial = bgMaterial;
		}
	}

	private void SetColor(Color color)
	{
		currentColor = color;
		for (int i = 0; i < spriteRenderers.Length; i++)
		{
			SpriteRenderer spriteRenderer = spriteRenderers[i];
			if ((bool)spriteRenderer)
			{
				spriteRenderer.enabled = (spriteRenderer.color = baseColors[i].MultiplyElements(color)).a > 0.001f;
			}
		}
	}
}

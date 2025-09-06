using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class CharacterTint : MonoBehaviour
{
	private static readonly List<SceneAppearanceRegion> _regions = new List<SceneAppearanceRegion>();

	private readonly Color baseColor = Color.white;

	private float lastFadeTime;

	private Coroutine fadeRoutine;

	private Renderer renderer;

	private MaterialPropertyBlock block;

	private static readonly int _characterTintColorProp = Shader.PropertyToID("_CharacterTintColor");

	private void Awake()
	{
		renderer = GetComponent<Renderer>();
		if ((bool)renderer)
		{
			block = new MaterialPropertyBlock();
			renderer.GetPropertyBlock(block);
			block.SetColor(_characterTintColorProp, baseColor);
			renderer.SetPropertyBlock(block);
		}
		UpdateTint(forceImmediate: true);
	}

	public void AddInside(SceneAppearanceRegion region, bool forceImmediate)
	{
		_regions.AddIfNotPresent(region);
		UpdateTint(forceImmediate);
	}

	public void RemoveInside(SceneAppearanceRegion region, bool forceImmediate)
	{
		_regions.Remove(region);
		UpdateTint(forceImmediate);
	}

	private void UpdateTint(bool forceImmediate)
	{
		if (!renderer)
		{
			return;
		}
		SceneAppearanceRegion sceneAppearanceRegion = _regions.LastOrDefault();
		if (fadeRoutine != null)
		{
			StopCoroutine(fadeRoutine);
		}
		Color color;
		float duration;
		if (sceneAppearanceRegion != null)
		{
			color = sceneAppearanceRegion.CharacterTintColor;
			duration = (lastFadeTime = sceneAppearanceRegion.FadeDuration);
		}
		else
		{
			color = baseColor;
			duration = lastFadeTime;
		}
		renderer.GetPropertyBlock(block);
		if (forceImmediate)
		{
			block.SetColor(_characterTintColorProp, color);
			renderer.SetPropertyBlock(block);
			return;
		}
		Color startColor = block.GetColor(_characterTintColorProp);
		fadeRoutine = this.StartTimerRoutine(0f, duration, delegate(float time)
		{
			Color value = Color.Lerp(startColor, color, time);
			renderer.GetPropertyBlock(block);
			block.SetColor(_characterTintColorProp, value);
			renderer.SetPropertyBlock(block);
		});
	}

	public static bool CanAdd(GameObject gameObject)
	{
		if (!gameObject)
		{
			return false;
		}
		Renderer component = gameObject.GetComponent<Renderer>();
		if (!component)
		{
			return false;
		}
		Material sharedMaterial = component.sharedMaterial;
		if (!sharedMaterial)
		{
			return false;
		}
		return sharedMaterial.IsKeywordEnabled("IS_CHARACTER");
	}
}

using System.Collections;
using UnityEngine;
using UnityStandardAssets.ImageEffects;

[ExecuteInEditMode]
[AddComponentMenu("Image Effects/Custom/Brightness Effect")]
public class BrightnessEffect : ImageEffectBase, IPostprocessModule
{
	[Range(0f, 2f)]
	public float _Brightness = 1f;

	[Range(0f, 2f)]
	public float _Contrast = 1f;

	private float brightnessMultiply;

	private float contrastMultiply;

	private Coroutine extraEffectFadeRoutine;

	private static readonly int _brightnessProp = Shader.PropertyToID("_Brightness");

	private static readonly int _contrastProp = Shader.PropertyToID("_Contrast");

	public string EffectKeyword => "BRIGHTNESS_EFFECT_ENABLED";

	private void Awake()
	{
		brightnessMultiply = 1f;
		contrastMultiply = 1f;
	}

	public void SetBrightness(float value)
	{
		_Brightness = value;
		if ((bool)GameCameraTextureDisplay.Instance)
		{
			GameCameraTextureDisplay.Instance.UpdateBrightness(_Brightness);
		}
	}

	public void SetContrast(float value)
	{
		_Contrast = value;
	}

	public void ExtraEffectFadeTo(float brightness, float contrast, float fadeTime, float delay)
	{
		if (extraEffectFadeRoutine != null)
		{
			StopCoroutine(extraEffectFadeRoutine);
			extraEffectFadeRoutine = null;
		}
		if (fadeTime > 0f || delay > 0f)
		{
			extraEffectFadeRoutine = StartCoroutine(ExtraEffectFadeToRoutine(brightness, contrast, fadeTime, delay));
			return;
		}
		brightnessMultiply = brightness;
		contrastMultiply = contrast;
	}

	private IEnumerator ExtraEffectFadeToRoutine(float brightness, float contrast, float fadeTime, float delay)
	{
		if (delay > 0f)
		{
			yield return new WaitForSeconds(delay);
		}
		if (fadeTime > 0f)
		{
			float initialBrightness = brightnessMultiply;
			float initialContrast = contrastMultiply;
			for (float elapsed = 0f; elapsed < fadeTime; elapsed += Time.deltaTime)
			{
				float t = elapsed / fadeTime;
				brightnessMultiply = Mathf.Lerp(initialBrightness, brightness, t);
				contrastMultiply = Mathf.Lerp(initialContrast, contrast, t);
				yield return null;
			}
		}
		brightnessMultiply = brightness;
		contrastMultiply = contrast;
		extraEffectFadeRoutine = null;
	}

	public void UpdateProperties(Material material)
	{
		material.SetFloat(_brightnessProp, _Brightness * brightnessMultiply);
		material.SetFloat(_contrastProp, _Contrast * contrastMultiply);
	}
}

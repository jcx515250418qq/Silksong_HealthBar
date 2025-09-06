using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class HeroLight : MonoBehaviour
{
	[SerializeField]
	protected SpriteRenderer spriteRenderer;

	[SerializeField]
	private new Transform transform;

	[SerializeField]
	private Transform vignette;

	[SerializeField]
	private float lerpTime = 0.2f;

	[SerializeField]
	private SpriteRenderer heroLightDonut;

	[SerializeField]
	private AnimationCurve heroLightDonutAlphaCurve = AnimationCurve.Linear(0f, 0f, 1f, 1f);

	private bool isDetached;

	private Vector2 initialLocalPosition;

	private Vector2 lerpStartPos;

	private Transform initialParent;

	private Coroutine lerpRoutine;

	private Color baseColor;

	private Color lerpColor;

	private float colorLerpT;

	private Coroutine fadeRoutine;

	private float fadeDuration;

	private readonly List<SceneAppearanceRegion> insideRegions = new List<SceneAppearanceRegion>();

	private static readonly int _color = Shader.PropertyToID("_Color");

	private float alpha;

	public Color BaseColor
	{
		get
		{
			return baseColor;
		}
		set
		{
			baseColor = value;
			SetColor();
		}
	}

	public Color MaterialColor
	{
		get
		{
			if (!spriteRenderer || !spriteRenderer.material)
			{
				return Color.white;
			}
			return spriteRenderer.material.GetColor(_color);
		}
		set
		{
			if ((bool)spriteRenderer && (bool)spriteRenderer.material)
			{
				spriteRenderer.material.SetColor(_color, value);
			}
		}
	}

	public float Alpha
	{
		get
		{
			return alpha;
		}
		set
		{
			alpha = Mathf.Max(value, 0.001f);
			SetColor();
		}
	}

	private void Awake()
	{
		Alpha = 1f;
		if ((bool)spriteRenderer)
		{
			baseColor = spriteRenderer.color;
			if (float.IsInfinity(baseColor.a))
			{
				baseColor.a = 1f;
			}
			else if (float.IsNaN(baseColor.a) || baseColor.a < 0.001f)
			{
				baseColor.a = 0.001f;
			}
			ApplyColor(baseColor);
		}
	}

	public void AddInside(SceneAppearanceRegion region, bool forceImmediate)
	{
		insideRegions.AddIfNotPresent(region);
		UpdateColor(forceImmediate);
	}

	public void RemoveInside(SceneAppearanceRegion region)
	{
		insideRegions.Remove(region);
		UpdateColor(forceImmediate: false);
	}

	public void UpdateColor(bool forceImmediate)
	{
		if (!spriteRenderer)
		{
			return;
		}
		if (fadeRoutine != null)
		{
			StopCoroutine(fadeRoutine);
		}
		float targetT;
		Color targetLerpColor;
		Action action;
		if (insideRegions.Count == 0)
		{
			if (forceImmediate)
			{
				colorLerpT = 0f;
				lerpColor = baseColor;
				SetColor();
				fadeDuration = 0f;
				return;
			}
			targetT = 0f;
			targetLerpColor = lerpColor;
			action = delegate
			{
				lerpColor = baseColor;
			};
		}
		else
		{
			SceneAppearanceRegion sceneAppearanceRegion = insideRegions.Last();
			targetLerpColor = GetTargetColor(sceneAppearanceRegion);
			targetT = 1f;
			fadeDuration = sceneAppearanceRegion.FadeDuration;
			action = null;
		}
		float startT = colorLerpT;
		Color startColor = lerpColor;
		if (fadeDuration > 0f && !forceImmediate && base.isActiveAndEnabled)
		{
			fadeRoutine = this.StartTimerRoutine(0f, fadeDuration, delegate(float time)
			{
				colorLerpT = Mathf.Lerp(startT, targetT, time);
				lerpColor = Color.Lerp(startColor, targetLerpColor, time);
				SetColor();
			}, null, action);
		}
		else
		{
			colorLerpT = 1f;
			lerpColor = targetLerpColor;
			action?.Invoke();
			SetColor();
		}
	}

	private void SetColor()
	{
		Color color = Color.Lerp(baseColor, lerpColor, colorLerpT);
		color.a *= Alpha;
		ApplyColor(color);
	}

	public void Detach()
	{
		if (!isDetached)
		{
			isDetached = true;
			if (lerpRoutine != null)
			{
				StopCoroutine(lerpRoutine);
				lerpRoutine = null;
				transform.SetLocalPosition2D(initialLocalPosition);
				vignette.SetLocalPosition2D(initialLocalPosition);
			}
			initialLocalPosition = transform.localPosition;
			initialParent = transform.parent;
			DeParent(transform);
			DeParent(vignette);
		}
	}

	private static void DeParent(Transform transform)
	{
		transform.SetParent(null, worldPositionStays: true);
		Vector3 localScale = transform.localScale;
		localScale.x = Mathf.Abs(localScale.x);
		transform.localScale = localScale;
	}

	private static void ReParent(Transform transform, Transform parent)
	{
		transform.SetParent(parent);
		Vector3 localScale = transform.localScale;
		localScale.x = Mathf.Abs(localScale.x);
		transform.localScale = localScale;
	}

	public void Reattach()
	{
		if (isDetached)
		{
			isDetached = false;
			ReParent(transform, initialParent);
			ReParent(vignette, initialParent);
			lerpStartPos = transform.localPosition;
			lerpRoutine = this.StartTimerRoutine(0f, lerpTime, delegate(float time)
			{
				Vector2 position = Vector2.Lerp(lerpStartPos, initialLocalPosition, time);
				transform.SetLocalPosition2D(position);
				vignette.SetLocalPosition2D(position);
			}, null, delegate
			{
				lerpRoutine = null;
			});
		}
	}

	private Color GetTargetColor(SceneAppearanceRegion region)
	{
		return region.HeroLightColor;
	}

	private void ApplyColor(Color color)
	{
		spriteRenderer.color = color;
		if ((bool)heroLightDonut)
		{
			Color color2 = heroLightDonut.color;
			color2.a = heroLightDonutAlphaCurve.Evaluate(color.a);
			heroLightDonut.color = color2;
		}
	}
}

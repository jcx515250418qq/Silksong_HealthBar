using System.Collections;
using UnityEngine;

public class LifebloodPustuleSubWither : MonoBehaviour
{
	private const float DESATURATE_DURATION = 1.5f;

	private const float DISTANCE_DELAY = 0.01f;

	[SerializeField]
	private SpriteRenderer spriteRenderer;

	[SerializeField]
	private Sprite witherVariant;

	[SerializeField]
	private GameObject witherStartEffects;

	private bool isWithered;

	private ParticleSystem[] particles;

	private static readonly int _desaturateProp = Shader.PropertyToID("_SaturationLerp");

	private void Reset()
	{
		spriteRenderer = GetComponent<SpriteRenderer>();
	}

	private void Awake()
	{
		if ((bool)witherStartEffects)
		{
			witherStartEffects.SetActive(value: false);
			particles = witherStartEffects.GetComponentsInChildren<ParticleSystem>(includeInactive: true);
		}
	}

	public void BeginWither(Transform fromTrans)
	{
		if (!isWithered)
		{
			isWithered = true;
			Vector2 a = base.transform.position;
			Vector2 b = fromTrans.position;
			float num = Vector2.Distance(a, b);
			StartCoroutine(Wither(num * 0.01f));
		}
	}

	public void StartWithered()
	{
		if (!isWithered)
		{
			isWithered = true;
			if ((bool)witherVariant)
			{
				spriteRenderer.sprite = witherVariant;
			}
			MaterialPropertyBlock materialPropertyBlock = new MaterialPropertyBlock();
			spriteRenderer.GetPropertyBlock(materialPropertyBlock);
			materialPropertyBlock.SetFloat(_desaturateProp, 0f);
			spriteRenderer.SetPropertyBlock(materialPropertyBlock);
		}
	}

	private IEnumerator Wither(float delay)
	{
		yield return new WaitForSeconds(delay);
		if ((bool)witherVariant)
		{
			spriteRenderer.sprite = witherVariant;
		}
		Color color = spriteRenderer.color;
		if ((bool)witherStartEffects)
		{
			ParticleSystem[] array = particles;
			for (int i = 0; i < array.Length; i++)
			{
				ParticleSystem.MainModule main = array[i].main;
				ParticleSystem.MinMaxGradient startColor = main.startColor;
				switch (startColor.mode)
				{
				case ParticleSystemGradientMode.Color:
					startColor.color *= color;
					break;
				case ParticleSystemGradientMode.TwoColors:
					startColor.colorMin *= color;
					startColor.colorMax *= color;
					break;
				}
				main.startColor = startColor;
			}
			witherStartEffects.SetActive(value: true);
		}
		MaterialPropertyBlock block = new MaterialPropertyBlock();
		spriteRenderer.GetPropertyBlock(block);
		for (float elapsed = 0f; elapsed < 1.5f; elapsed += Time.deltaTime)
		{
			block.SetFloat(_desaturateProp, Mathf.Lerp(1f, 0f, elapsed / 1.5f));
			spriteRenderer.SetPropertyBlock(block);
			yield return null;
		}
		block.SetFloat(_desaturateProp, 0f);
		spriteRenderer.SetPropertyBlock(block);
	}
}

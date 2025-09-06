using System;
using TeamCherry.SharedUtils;
using UnityEngine;
using UnityEngine.UI;

public class UiProgressBar : MonoBehaviour
{
	[SerializeField]
	private Image barImage;

	[SerializeField]
	private Image barImageOther;

	[SerializeField]
	private SpriteRenderer barSprite;

	[SerializeField]
	private MinMaxFloat valueRange;

	[SerializeField]
	private float barLerpSpeed;

	[Space]
	[SerializeField]
	private float value;

	private float lastBarFill;

	private float targetImageFill;

	private MaterialPropertyBlock propertyBlock;

	private static readonly int _arc1PropId = Shader.PropertyToID("_Arc1");

	private static readonly int _arc2PropId = Shader.PropertyToID("_Arc2");

	private static readonly int ANGLE = Shader.PropertyToID("_Angle");

	private static readonly int EDGE_FADE = Shader.PropertyToID("_EdgeFade");

	public float Value
	{
		get
		{
			return value;
		}
		set
		{
			this.value = value;
			UpdateImage();
		}
	}

	private void OnValidate()
	{
		if (!Application.isPlaying)
		{
			UpdateImage();
		}
	}

	private void Update()
	{
		UpdateBar(Time.deltaTime);
	}

	public void UpdateBar(float deltaTime)
	{
		if (!(Math.Abs(lastBarFill - targetImageFill) < 0.001f))
		{
			float num = Mathf.Lerp(lastBarFill, targetImageFill, deltaTime * barLerpSpeed);
			if (Math.Abs(num - targetImageFill) < 0.001f)
			{
				num = targetImageFill;
			}
			SetBarFill(num);
		}
	}

	private void UpdateImage()
	{
		float lerpedValue = valueRange.GetLerpedValue(value);
		if (Math.Abs(lerpedValue - lastBarFill) > Mathf.Epsilon)
		{
			targetImageFill = lerpedValue;
			if (barLerpSpeed <= Mathf.Epsilon || !base.gameObject.activeInHierarchy)
			{
				SetBarFill(targetImageFill);
			}
		}
	}

	private void SetBarFill(float fillAmount)
	{
		lastBarFill = fillAmount;
		if ((bool)barImage)
		{
			barImage.fillAmount = fillAmount;
		}
		if ((bool)barImageOther)
		{
			barImageOther.fillAmount = fillAmount;
		}
		if ((bool)barSprite)
		{
			if (propertyBlock == null)
			{
				propertyBlock = new MaterialPropertyBlock();
			}
			barSprite.GetPropertyBlock(propertyBlock);
			propertyBlock.SetFloat(_arc1PropId, fillAmount);
			propertyBlock.SetFloat(_arc2PropId, fillAmount);
			barSprite.SetPropertyBlock(propertyBlock);
		}
	}

	public void SetAngle(float angle)
	{
		if ((bool)barSprite)
		{
			if (propertyBlock == null)
			{
				propertyBlock = new MaterialPropertyBlock();
			}
			barSprite.GetPropertyBlock(propertyBlock);
			propertyBlock.SetFloat(ANGLE, angle);
			barSprite.SetPropertyBlock(propertyBlock);
		}
	}

	public void SetEdgeFade(float edgeFade)
	{
		if ((bool)barSprite)
		{
			if (propertyBlock == null)
			{
				propertyBlock = new MaterialPropertyBlock();
			}
			barSprite.GetPropertyBlock(propertyBlock);
			propertyBlock.SetFloat(EDGE_FADE, edgeFade);
			barSprite.SetPropertyBlock(propertyBlock);
		}
	}

	public void SetValueInstant(float val)
	{
		value = val;
		targetImageFill = valueRange.GetLerpedValue(val);
		SetBarFill(targetImageFill);
	}
}

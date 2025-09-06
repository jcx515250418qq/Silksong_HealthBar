using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public abstract class RadialHudIcon : MonoBehaviour
{
	[SerializeField]
	private SpriteRenderer icon;

	[SerializeField]
	protected Color activeColor;

	[SerializeField]
	protected Color inactiveColor;

	[Space]
	[SerializeField]
	private Image radialImage;

	[SerializeField]
	private Image radialImageBg;

	[SerializeField]
	private float radialLerpDownTime;

	[SerializeField]
	private float radialLerpUpTime;

	[SerializeField]
	private AnimationCurve radialLerpCurve = AnimationCurve.Linear(0f, 0f, 1f, 1f);

	[SerializeField]
	private GameObject templateNotch;

	private readonly List<GameObject> notches = new List<GameObject>();

	private Coroutine radialLerpRoutine;

	private int amountLeft;

	private int storageAmount;

	private float previousFillAmount;

	private bool updated;

	private void Start()
	{
		if (!updated)
		{
			UpdateDisplay();
		}
	}

	private void Update()
	{
		if (radialLerpRoutine == null)
		{
			float targetFillAmount = GetTargetFillAmount();
			if (!(Math.Abs(targetFillAmount - previousFillAmount) <= Mathf.Epsilon))
			{
				SetFillAmount(targetFillAmount);
			}
		}
	}

	private float GetTargetFillAmount()
	{
		if (storageAmount <= 0)
		{
			return 1f;
		}
		float num = (float)amountLeft / (float)storageAmount;
		float midProgress = GetMidProgress();
		if (midProgress <= Mathf.Epsilon)
		{
			return num;
		}
		float b = (float)Mathf.Clamp(amountLeft - 1, 0, storageAmount) / (float)storageAmount;
		return Mathf.Lerp(num, b, midProgress);
	}

	private void SetFillAmount(float value)
	{
		if ((bool)radialImage)
		{
			radialImage.fillAmount = value;
		}
		if ((bool)radialImageBg)
		{
			radialImageBg.fillAmount = 1f - value;
		}
		previousFillAmount = value;
	}

	protected virtual void OnPreUpdateDisplay()
	{
	}

	protected abstract bool GetIsActive();

	protected abstract void GetAmounts(out int amountLeft, out int totalCount);

	protected abstract bool TryGetHudSprite(out Sprite sprite);

	public abstract bool GetIsEmpty();

	protected abstract bool HasTargetChanged();

	protected virtual bool TryGetBarColour(out Color color)
	{
		color = Color.black;
		return false;
	}

	protected virtual float GetMidProgress()
	{
		return 0f;
	}

	protected void UpdateDisplay()
	{
		updated = true;
		OnPreUpdateDisplay();
		if (!GetIsActive())
		{
			base.gameObject.SetActive(value: false);
			return;
		}
		base.gameObject.SetActive(value: true);
		GetAmounts(out amountLeft, out storageAmount);
		bool isEmpty = GetIsEmpty();
		if ((bool)icon)
		{
			if (TryGetHudSprite(out var sprite))
			{
				icon.sprite = sprite;
				icon.transform.localScale = new Vector3(1.4f, 1.4f, 1f);
			}
			else
			{
				icon.sprite = sprite;
				icon.transform.localScale = new Vector3(0.7f, 0.7f, 1f);
			}
			SetIconColour(icon, isEmpty ? inactiveColor : activeColor);
		}
		if ((bool)radialImage)
		{
			if (!TryGetBarColour(out var color))
			{
				color = Color.white;
			}
			float targetFillAmount;
			if (storageAmount > 0)
			{
				targetFillAmount = GetTargetFillAmount();
			}
			else
			{
				targetFillAmount = 1f;
				if (isEmpty)
				{
					color = color.MultiplyElements(inactiveColor);
				}
			}
			radialImage.color = color;
			if (radialLerpRoutine != null)
			{
				StopCoroutine(radialLerpRoutine);
			}
			if (!HasTargetChanged())
			{
				float initialFillAmount = radialImage.fillAmount;
				float duration = ((targetFillAmount > initialFillAmount) ? radialLerpUpTime : radialLerpDownTime);
				radialLerpRoutine = this.StartTimerRoutine(0f, duration, delegate(float time)
				{
					SetFillAmount(Mathf.Lerp(initialFillAmount, targetFillAmount, radialLerpCurve.Evaluate(time)));
				}, null, delegate
				{
					radialLerpRoutine = null;
				});
			}
			else
			{
				SetFillAmount(targetFillAmount);
			}
		}
		if ((bool)templateNotch)
		{
			templateNotch.SetActive(value: false);
			for (int num = storageAmount - notches.Count; num > 0; num--)
			{
				GameObject item = UnityEngine.Object.Instantiate(templateNotch, templateNotch.transform.parent);
				notches.Add(item);
			}
			for (int i = 0; i < notches.Count; i++)
			{
				notches[i].SetActive(i < storageAmount);
			}
		}
	}

	protected virtual void SetIconColour(SpriteRenderer spriteRenderer, Color color)
	{
		spriteRenderer.color = color;
	}
}

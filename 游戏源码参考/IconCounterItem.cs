using System;
using UnityEngine;

public class IconCounterItem : MonoBehaviour
{
	[Serializable]
	private struct DisplayState
	{
		public Color Color;

		public Sprite Sprite;

		public Material Material;
	}

	[SerializeField]
	private SpriteRenderer spriteRenderer;

	[SerializeField]
	private bool setFlashColour;

	[SerializeField]
	private DisplayState activeState;

	[SerializeField]
	private DisplayState inactiveState;

	[SerializeField]
	private bool inactiveDisable;

	[SerializeField]
	private PlayerDataTest customCondition;

	[SerializeField]
	private CollectableItem orItemCondition;

	private Color baseColor = Color.white;

	private Color tintColor = Color.white;

	private Vector3 initialScale;

	private MaterialPropertyBlock propBlock;

	private static readonly int _flashColor = Shader.PropertyToID("_FlashColor");

	public Sprite Sprite
	{
		set
		{
			if ((bool)spriteRenderer)
			{
				spriteRenderer.sprite = value;
			}
		}
	}

	public Vector3 Scale
	{
		set
		{
			if ((bool)spriteRenderer)
			{
				spriteRenderer.transform.localScale = value.MultiplyElements(initialScale);
			}
			else
			{
				base.transform.localScale = value.MultiplyElements(initialScale);
			}
		}
	}

	public Color TintColor
	{
		get
		{
			return tintColor;
		}
		set
		{
			tintColor = value;
			if ((bool)spriteRenderer)
			{
				spriteRenderer.color = baseColor.MultiplyElements(tintColor);
			}
		}
	}

	private void Awake()
	{
		initialScale = (spriteRenderer ? spriteRenderer.transform.localScale : base.transform.localScale);
	}

	private void OnEnable()
	{
		if (customCondition.IsDefined && customCondition.IsFulfilled)
		{
			SetFilled(value: true);
		}
		else if ((bool)orItemCondition)
		{
			SetFilled(orItemCondition.CollectedAmount > 0);
		}
		else if (customCondition.IsDefined)
		{
			SetFilled(value: false);
		}
	}

	public void SetFilled(bool value)
	{
		if (!spriteRenderer)
		{
			return;
		}
		DisplayState displayState;
		if (value)
		{
			displayState = activeState;
			if (!base.gameObject.activeSelf)
			{
				base.gameObject.SetActive(value: true);
			}
		}
		else
		{
			if (inactiveDisable)
			{
				base.gameObject.SetActive(value: false);
				return;
			}
			displayState = inactiveState;
		}
		baseColor = displayState.Color;
		TintColor = TintColor;
		if ((bool)displayState.Sprite)
		{
			spriteRenderer.sprite = displayState.Sprite;
		}
		if ((bool)displayState.Material)
		{
			spriteRenderer.sharedMaterial = displayState.Material;
		}
		if (setFlashColour)
		{
			if (propBlock == null)
			{
				propBlock = new MaterialPropertyBlock();
			}
			propBlock.Clear();
			spriteRenderer.GetPropertyBlock(propBlock);
			propBlock.SetColor(_flashColor, displayState.Color);
			spriteRenderer.SetPropertyBlock(propBlock);
		}
	}
}

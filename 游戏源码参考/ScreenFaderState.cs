using UnityEngine;

public sealed class ScreenFaderState : MonoBehaviour
{
	[SerializeField]
	private SpriteRenderer spriteRenderer;

	private static ScreenFaderState instance;

	private static bool hasInstance;

	public static float Alpha
	{
		get
		{
			if (hasInstance)
			{
				if (!instance.spriteRenderer.enabled)
				{
					return 0f;
				}
				return instance.spriteRenderer.color.a;
			}
			return 0f;
		}
	}

	private void Awake()
	{
		if (instance == null)
		{
			instance = this;
			hasInstance = true;
			if (spriteRenderer == null)
			{
				spriteRenderer = GetComponent<SpriteRenderer>();
			}
		}
	}

	private void OnValidate()
	{
		if (spriteRenderer == null)
		{
			spriteRenderer = GetComponent<SpriteRenderer>();
		}
	}

	private void OnDestroy()
	{
		if (instance == this)
		{
			hasInstance = false;
			instance = null;
		}
	}
}

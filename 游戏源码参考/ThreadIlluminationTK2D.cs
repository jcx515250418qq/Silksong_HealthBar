using UnityEngine;

public class ThreadIlluminationTK2D : MonoBehaviour
{
	private const float RANDOMISATION_RANGE = 8f;

	private const float VISIBLE_RANGE = 1f;

	private const float FALLOFF_RANGE = 10f;

	private tk2dSprite sprite;

	private Transform mainCamera;

	private Color initialColor;

	private float offsetX;

	private float offsetY;

	private bool visible;

	private bool revertAlpha;

	private bool fadeOut;

	public bool active = true;

	private void Awake()
	{
		sprite = GetComponent<tk2dSprite>();
		initialColor = sprite.color;
	}

	private void OnEnable()
	{
		mainCamera = GameCameras.instance.mainCamera.gameObject.transform;
		offsetX = Random.Range(-8f, 8f);
		offsetY = Random.Range(-8f, 8f);
	}

	private void Update()
	{
		if (active)
		{
			Vector2 a = mainCamera.position;
			Vector3 position = base.transform.position;
			Vector2 b = new Vector2(position.x + offsetX, position.y + offsetY);
			float num = Vector2.Distance(a, b);
			if (num > 10f)
			{
				SetAlpha(0f);
				if (visible)
				{
					visible = false;
				}
			}
			else if (num > 1f)
			{
				if (visible)
				{
					visible = false;
				}
				float alpha = 1f - (num - 1f) / 9f;
				SetAlpha(alpha);
			}
			else if (!visible)
			{
				SetAlpha(1f);
				visible = true;
			}
		}
		if (revertAlpha)
		{
			float a2 = sprite.color.a;
			if (a2 < 1f)
			{
				a2 += Time.deltaTime * 4f;
				if (a2 > 1f)
				{
					a2 = 1f;
				}
				sprite.color = new Color(initialColor.r, initialColor.g, initialColor.b, a2);
			}
			else
			{
				revertAlpha = false;
			}
		}
		if (!fadeOut)
		{
			return;
		}
		float a3 = sprite.color.a;
		if (a3 > 0f)
		{
			a3 -= Time.deltaTime * 4f;
			if (a3 < 0f)
			{
				a3 = 0f;
			}
			sprite.color = new Color(initialColor.r, initialColor.g, initialColor.b, a3);
		}
		else
		{
			fadeOut = false;
		}
	}

	private void SetAlpha(float alpha)
	{
		sprite.color = new Color(initialColor.r, initialColor.g, initialColor.b, alpha);
	}

	public void StopIllumination()
	{
		active = false;
	}

	public void ThreadStrum()
	{
		StopIllumination();
		revertAlpha = true;
		tk2dSpriteAnimator component = GetComponent<tk2dSpriteAnimator>();
		if ((bool)component)
		{
			component.PlayFromFrame("Strum", Random.Range(0, 3));
		}
	}

	public void ThreadEnd()
	{
		fadeOut = true;
	}
}

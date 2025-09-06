using TeamCherry.NestedFadeGroup;
using UnityEngine;

public class ThreadIllumination : MonoBehaviour, IUpdateBatchableUpdate
{
	private const float RANDOMISATION_RANGE = 8f;

	private const float VISIBLE_RANGE = 2f;

	private const float FALLOFF_RANGE = 12f;

	private SpriteRenderer spriteRenderer;

	private NestedFadeGroupSpriteRenderer fadeBridge;

	private Transform mainCamera;

	private Color initialColor;

	private float offsetX;

	private float offsetY;

	private bool visible;

	public bool ShouldUpdate
	{
		get
		{
			if ((bool)spriteRenderer)
			{
				return spriteRenderer.isVisible;
			}
			return true;
		}
	}

	private void Awake()
	{
		spriteRenderer = GetComponent<SpriteRenderer>();
		initialColor = spriteRenderer.color;
		fadeBridge = GetComponent<NestedFadeGroupSpriteRenderer>();
		if ((bool)fadeBridge)
		{
			initialColor.a = fadeBridge.AlphaSelf;
		}
	}

	private void OnEnable()
	{
		mainCamera = GameCameras.instance.mainCamera.gameObject.transform;
		offsetX = Random.Range(-8f, 8f);
		offsetY = Random.Range(-8f, 8f);
		GameManager instance = GameManager.instance;
		if ((bool)instance)
		{
			instance.GetComponent<UpdateBatcher>().Add(this);
		}
	}

	private void OnDisable()
	{
		GameManager silentInstance = GameManager.SilentInstance;
		if ((bool)silentInstance)
		{
			silentInstance.GetComponent<UpdateBatcher>().Remove(this);
		}
	}

	public void BatchedUpdate()
	{
		Vector2 a = mainCamera.position;
		Vector3 position = base.transform.position;
		Vector2 b = new Vector2(position.x + offsetX, position.y + offsetY);
		float num = Vector2.Distance(a, b);
		if (num > 12f)
		{
			SetAlpha(0.1f);
			if (visible)
			{
				visible = false;
			}
		}
		else if (num > 2f)
		{
			if (visible)
			{
				visible = false;
			}
			float alpha = 1f - (num - 2f) / 10f * 0.9f;
			SetAlpha(alpha);
		}
		else if (!visible)
		{
			SetAlpha(1f);
			visible = true;
		}
	}

	private void SetAlpha(float alpha)
	{
		if ((bool)fadeBridge)
		{
			fadeBridge.AlphaSelf = alpha;
		}
		else
		{
			spriteRenderer.color = new Color(initialColor.r, initialColor.g, initialColor.b, alpha);
		}
	}
}

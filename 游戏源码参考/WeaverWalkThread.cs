using UnityEngine;

public class WeaverWalkThread : MonoBehaviour
{
	public Transform weaver;

	private float visibleRange = 1f;

	private float falloffRange = 3f;

	private float fullAlpha;

	private SpriteRenderer spriteRenderer;

	private float offsetX;

	private float offsetY;

	private bool visible;

	private void Awake()
	{
		spriteRenderer = GetComponent<SpriteRenderer>();
	}

	private void OnEnable()
	{
		base.transform.localEulerAngles = new Vector3(0f, 0f, Random.Range(-1.5f, 1.5f));
		fullAlpha = Random.Range(0.5f, 0.8f);
	}

	private void Update()
	{
		Vector2 a = new Vector2(weaver.position.x, weaver.position.y);
		Vector2 b = new Vector2(base.transform.position.x + offsetX, base.transform.position.y + offsetY);
		float num = Vector2.Distance(a, b);
		if (num > falloffRange)
		{
			if (spriteRenderer.enabled)
			{
				spriteRenderer.enabled = false;
			}
			if (visible)
			{
				visible = false;
			}
		}
		else if (num > visibleRange)
		{
			if (!spriteRenderer.enabled)
			{
				spriteRenderer.enabled = true;
			}
			if (visible)
			{
				visible = false;
			}
			float a2 = fullAlpha - (num - visibleRange) / (falloffRange - visibleRange);
			spriteRenderer.color = new Color(1f, 1f, 1f, a2);
		}
		else if (!visible)
		{
			spriteRenderer.enabled = true;
			spriteRenderer.color = new Color(1f, 1f, 1f, fullAlpha);
			visible = true;
		}
	}
}

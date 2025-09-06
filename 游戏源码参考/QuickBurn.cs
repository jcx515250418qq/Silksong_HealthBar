using UnityEngine;

public class QuickBurn : MonoBehaviour
{
	private bool inCoal;

	private bool burnt;

	private bool fading;

	private float timer;

	private Rigidbody2D rb;

	private tk2dSprite sprite;

	private SpriteRenderer spriteRenderer;

	private Color startColour;

	private readonly Color burnColour = new Color(0.075f, 0.06f, 0.06f, 1f);

	private readonly Color fadeColour = new Color(0.075f, 0.06f, 0.06f, 0f);

	private void Awake()
	{
		rb = GetComponent<Rigidbody2D>();
		sprite = GetComponent<tk2dSprite>();
		spriteRenderer = GetComponent<SpriteRenderer>();
		if ((bool)sprite)
		{
			startColour = sprite.color;
		}
		if ((bool)spriteRenderer)
		{
			startColour = spriteRenderer.color;
		}
	}

	private void OnTriggerEnter2D(Collider2D collision)
	{
		if (collision.gameObject.CompareTag("Coal"))
		{
			inCoal = true;
		}
	}

	private void OnTriggerExit2D(Collider2D collision)
	{
		if (!burnt && inCoal && collision.gameObject.CompareTag("Coal"))
		{
			inCoal = false;
			timer = 0f;
		}
	}

	private void Update()
	{
		if (inCoal && !burnt)
		{
			timer += Time.deltaTime;
			if (timer >= 2f && !burnt)
			{
				burnt = true;
			}
		}
		if (burnt && !fading)
		{
			timer += Time.deltaTime;
			if ((bool)sprite)
			{
				sprite.color = Color.Lerp(startColour, burnColour, (timer - 2f) * 2f);
			}
			if ((bool)spriteRenderer)
			{
				spriteRenderer.color = Color.Lerp(startColour, burnColour, (timer - 2f) * 2f);
			}
			if (timer >= 2.5f && !fading)
			{
				fading = true;
				rb.isKinematic = true;
				rb.linearVelocity = new Vector2(0f, -1f);
			}
		}
		if (fading)
		{
			timer += Time.deltaTime;
			if ((bool)sprite)
			{
				sprite.color = Color.Lerp(burnColour, fadeColour, (timer - 2.5f) * 2f);
			}
			if ((bool)spriteRenderer)
			{
				spriteRenderer.color = Color.Lerp(burnColour, fadeColour, (timer - 2.5f) * 2f);
			}
			if (timer >= 3f)
			{
				base.transform.position = new Vector3(-50f, -50f, 0f);
			}
			if ((double)timer >= 3.5)
			{
				base.gameObject.SetActive(value: false);
			}
		}
	}
}

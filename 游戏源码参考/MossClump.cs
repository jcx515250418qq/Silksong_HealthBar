using System.Collections.Generic;
using UnityEngine;

public class MossClump : MonoBehaviour, ScenePrefabInstanceFix.ICheckFields
{
	public List<Sprite> sprites;

	public SpriteRenderer spriteRenderer;

	public List<GameObject> touchingObjects;

	private bool depressed;

	private const float frameTime = 0.05f;

	private float timer;

	private int currentFrame;

	private void Awake()
	{
		if (!spriteRenderer)
		{
			base.enabled = false;
		}
	}

	private void OnTriggerEnter2D(Collider2D collision)
	{
		int layer = collision.gameObject.layer;
		if (layer == 9 || layer == 11 || layer == 26)
		{
			touchingObjects.Add(collision.gameObject);
			if (!depressed)
			{
				depressed = true;
			}
		}
	}

	private void OnTriggerExit2D(Collider2D collision)
	{
		if (touchingObjects.Contains(collision.gameObject))
		{
			touchingObjects.Remove(collision.gameObject);
			if (touchingObjects.Count <= 0)
			{
				depressed = false;
			}
		}
	}

	private void Update()
	{
		if (timer > 0f)
		{
			timer -= Time.deltaTime;
			return;
		}
		if (depressed && currentFrame < sprites.Count - 1)
		{
			currentFrame++;
			ChangeFrame();
		}
		if (!depressed && currentFrame > 0)
		{
			currentFrame--;
			ChangeFrame();
		}
	}

	private void ChangeFrame()
	{
		if ((bool)spriteRenderer)
		{
			spriteRenderer.sprite = sprites[currentFrame];
			timer = 0.05f;
		}
	}

	public void OnPrefabInstanceFix()
	{
		if ((bool)spriteRenderer)
		{
			ScenePrefabInstanceFix.CheckField(ref spriteRenderer);
		}
	}
}

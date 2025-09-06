using System.Collections.Generic;
using UnityEngine;

public class RandomSprite : MonoBehaviour
{
	[SerializeField]
	private List<Sprite> sprites;

	[SerializeField]
	private bool flipXScale;

	[SerializeField]
	private bool flipYScale;

	private SpriteRenderer spriteRenderer;

	private void Awake()
	{
		spriteRenderer = GetComponent<SpriteRenderer>();
	}

	private void OnEnable()
	{
		spriteRenderer.sprite = sprites[Random.Range(0, sprites.Count)];
		if (flipXScale && Random.Range(1, 100) > 50)
		{
			base.transform.localScale = new Vector3(0f - base.transform.localScale.x, base.transform.localScale.y, base.transform.localScale.z);
		}
		if (flipYScale && Random.Range(1, 100) > 50)
		{
			base.transform.localScale = new Vector3(base.transform.localScale.x, 0f - base.transform.localScale.y, base.transform.localScale.z);
		}
	}
}

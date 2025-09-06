using UnityEngine;

public class InvRelicBackboard : MonoBehaviour
{
	public Sprite activeSprite;

	public Sprite inactiveSprite;

	private PlayerData playerData;

	private SpriteRenderer spriteRenderer;

	private void Awake()
	{
		spriteRenderer = GetComponent<SpriteRenderer>();
	}

	private void OnEnable()
	{
		if (spriteRenderer != null)
		{
			if (playerData == null)
			{
				playerData = PlayerData.instance;
			}
			spriteRenderer.sprite = activeSprite;
		}
	}
}

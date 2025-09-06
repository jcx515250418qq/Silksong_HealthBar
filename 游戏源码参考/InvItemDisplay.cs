using UnityEngine;

public class InvItemDisplay : MonoBehaviour
{
	public string playerDataBool;

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
			if (playerData.GetBool(playerDataBool))
			{
				spriteRenderer.sprite = activeSprite;
			}
			else
			{
				spriteRenderer.sprite = inactiveSprite;
			}
		}
	}
}

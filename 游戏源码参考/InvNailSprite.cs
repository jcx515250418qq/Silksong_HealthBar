using UnityEngine;

public class InvNailSprite : MonoBehaviour
{
	public Sprite level1;

	public Sprite level2;

	public Sprite level3;

	public Sprite level4;

	public Sprite level5;

	private SpriteRenderer spriteRenderer;

	private PlayerData playerData;

	private void Awake()
	{
		spriteRenderer = GetComponent<SpriteRenderer>();
	}

	private void OnEnable()
	{
		if (playerData == null)
		{
			playerData = PlayerData.instance;
		}
		switch (playerData.GetInt("nailSmithUpgrades"))
		{
		case 0:
			spriteRenderer.sprite = level1;
			break;
		case 1:
			spriteRenderer.sprite = level2;
			break;
		case 2:
			spriteRenderer.sprite = level3;
			break;
		case 3:
			spriteRenderer.sprite = level4;
			break;
		case 4:
			spriteRenderer.sprite = level5;
			break;
		default:
			spriteRenderer.sprite = level1;
			break;
		}
	}
}

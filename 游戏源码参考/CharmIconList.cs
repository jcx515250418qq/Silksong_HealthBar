using UnityEngine;

public class CharmIconList : MonoBehaviour
{
	public static CharmIconList Instance;

	public Sprite[] spriteList;

	public Sprite unbreakableHeart;

	public Sprite unbreakableGreed;

	public Sprite unbreakableStrength;

	public Sprite grimmchildLevel1;

	public Sprite grimmchildLevel2;

	public Sprite grimmchildLevel3;

	public Sprite grimmchildLevel4;

	public Sprite nymmCharm;

	private PlayerData playerData;

	private void Awake()
	{
		Instance = this;
	}

	private void Start()
	{
		playerData = PlayerData.instance;
	}

	public Sprite GetSprite(int id)
	{
		playerData = PlayerData.instance;
		return spriteList[id];
	}
}

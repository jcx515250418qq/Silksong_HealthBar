using UnityEngine;

public class InvCharmBackboard : MonoBehaviour
{
	public GameObject charmObject;

	public GameObject newOrb;

	public int charmNum;

	public string charmNumString;

	public string gotCharmString;

	public string newCharmString;

	public Sprite blankSprite;

	public Sprite activeSprite;

	private bool positionedCharm;

	private PlayerData playerData;

	private GameObject orb;

	private SpriteRenderer spriteRenderer;

	private bool blanked;

	private void Awake()
	{
		spriteRenderer = GetComponent<SpriteRenderer>();
	}

	private void OnEnable()
	{
		if (!positionedCharm)
		{
			charmObject.transform.localPosition = new Vector3(base.transform.localPosition.x, base.transform.localPosition.y, base.transform.localPosition.z - 0.001f);
			positionedCharm = true;
		}
		if (playerData == null)
		{
			playerData = PlayerData.instance;
		}
		if (playerData.GetBool(gotCharmString) && playerData.GetBool(newCharmString))
		{
			newOrb.SetActive(value: true);
		}
		if (playerData.GetBool(gotCharmString) && !blanked)
		{
			spriteRenderer.sprite = blankSprite;
			blanked = true;
		}
		if (!playerData.GetBool(gotCharmString) && blanked)
		{
			spriteRenderer.sprite = activeSprite;
			blanked = false;
		}
	}

	public void SelectCharm()
	{
		if (playerData.GetBool(newCharmString))
		{
			playerData.SetBool(newCharmString, value: false);
			newOrb.GetComponent<SimpleFadeOut>().FadeOut();
		}
	}

	public int GetCharmNum()
	{
		return charmNum;
	}

	public string GetCharmString()
	{
		return gotCharmString;
	}

	public string GetCharmNumString()
	{
		return charmNumString;
	}
}

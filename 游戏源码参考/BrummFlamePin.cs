using UnityEngine;

public class BrummFlamePin : MonoBehaviour
{
	private GameManager gm;

	private PlayerData pd;

	private void Start()
	{
		gm = GameManager.instance;
		pd = gm.playerData;
	}

	private void OnEnable()
	{
		base.gameObject.GetComponent<SpriteRenderer>().enabled = false;
		if (gm == null)
		{
			gm = GameManager.instance;
		}
		if (pd == null)
		{
			pd = gm.playerData;
		}
		base.gameObject.GetComponent<SpriteRenderer>().enabled = true;
	}
}

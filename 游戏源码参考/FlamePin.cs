using UnityEngine;

public class FlamePin : MonoBehaviour
{
	private PlayerData pd;

	public float level;

	private void Start()
	{
		pd = PlayerData.instance;
	}

	private void OnEnable()
	{
		base.gameObject.GetComponent<SpriteRenderer>().enabled = false;
		if (pd == null)
		{
			pd = PlayerData.instance;
		}
		base.gameObject.GetComponent<SpriteRenderer>().enabled = true;
	}
}

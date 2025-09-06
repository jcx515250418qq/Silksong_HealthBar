using UnityEngine;

public class DeactivateIfPlayerdataFalseDelayed : MonoBehaviour
{
	[PlayerDataField(typeof(bool), true)]
	public string boolName;

	public float delay;

	private GameManager gm;

	private PlayerData pd;

	private void Start()
	{
		gm = GameManager.instance;
		pd = gm.playerData;
	}

	private void OnEnable()
	{
		if (delay <= 0f)
		{
			DoCheck();
		}
		else
		{
			Invoke("DoCheck", delay);
		}
	}

	private void DoCheck()
	{
		if (gm == null)
		{
			gm = GameManager.instance;
		}
		if (pd == null)
		{
			pd = gm.playerData;
		}
		if (!pd.GetBool(boolName))
		{
			base.gameObject.SetActive(value: false);
		}
	}
}

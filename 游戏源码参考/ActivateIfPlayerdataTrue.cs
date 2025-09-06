using UnityEngine;

public class ActivateIfPlayerdataTrue : MonoBehaviour
{
	[PlayerDataField(typeof(bool), true)]
	public string boolName;

	public GameObject objectToActivate;

	private GameManager gm;

	private PlayerData pd;

	private void Start()
	{
		gm = GameManager.instance;
		pd = gm.playerData;
		if (pd.GetBool(boolName))
		{
			base.gameObject.SetActive(value: true);
			if ((bool)objectToActivate)
			{
				objectToActivate.SetActive(value: true);
			}
		}
	}
}

using UnityEngine;

public class ChainLinkBreak : MonoBehaviour
{
	public GameObject breakEffects;

	public void Break()
	{
		breakEffects.transform.parent = null;
		breakEffects.SetActive(value: true);
		base.gameObject.SetActive(value: false);
	}
}

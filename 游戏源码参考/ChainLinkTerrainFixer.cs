using UnityEngine;

public class ChainLinkTerrainFixer : MonoBehaviour
{
	[SerializeField]
	private ChainLinkInteraction chainLink;

	private void OnCollisionEnter2D()
	{
		if ((bool)chainLink)
		{
			chainLink.SetActive(value: false);
		}
	}
}

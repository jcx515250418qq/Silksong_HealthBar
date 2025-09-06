using UnityEngine;
using UnityEngine.UI;

public class SaveProfileSilkBar : MonoBehaviour
{
	[SerializeField]
	private LayoutElement sizer;

	[SerializeField]
	private float widthPerSilk;

	[SerializeField]
	private float baseWidth;

	[SerializeField]
	private Image silkChunkTemplate;

	[SerializeField]
	private Sprite[] silkChunkVariants;

	[SerializeField]
	private Transform silkChunkParent;

	[Space]
	[SerializeField]
	private GameObject notBroken;

	[SerializeField]
	private GameObject brokenAlt;

	[SerializeField]
	private GameObject cursedAlt;

	public void ShowSilk(bool isBroken, int maxSilk, bool isCursed)
	{
		cursedAlt.SetActive(isCursed);
		notBroken.SetActive(!isBroken && !isCursed);
		brokenAlt.SetActive(isBroken && !isCursed);
		sizer.preferredWidth = (float)maxSilk * widthPerSilk - baseWidth;
		silkChunkTemplate.gameObject.SetActive(value: false);
		if (silkChunkTemplate.transform.parent == silkChunkParent)
		{
			silkChunkTemplate.transform.SetParent(silkChunkParent.parent);
		}
		for (int num = maxSilk - silkChunkParent.childCount; num > 0; num--)
		{
			Object.Instantiate(silkChunkTemplate, silkChunkParent);
		}
		for (int i = 0; i < silkChunkParent.childCount; i++)
		{
			Transform child = silkChunkParent.GetChild(i);
			if (!isBroken && i < maxSilk)
			{
				child.GetComponent<Image>().sprite = silkChunkVariants[Random.Range(0, silkChunkVariants.Length)];
				child.gameObject.SetActive(value: true);
			}
			else
			{
				child.gameObject.SetActive(value: false);
			}
		}
	}
}

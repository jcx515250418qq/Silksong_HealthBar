using System.Collections;
using UnityEngine;

public class AreaTitleInitialiser : MonoBehaviour
{
	[SerializeField]
	private AreaTitle areaTitle;

	private IEnumerator Start()
	{
		if (areaTitle == null)
		{
			yield break;
		}
		GameObject areaTitleGameObject = areaTitle.gameObject;
		while (!areaTitle.Initialised)
		{
			if (!areaTitleGameObject.activeSelf)
			{
				areaTitleGameObject.SetActive(value: true);
			}
			yield return null;
		}
	}
}

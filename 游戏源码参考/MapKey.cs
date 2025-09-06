using UnityEngine;
using UnityEngine.UI;

public class MapKey : MonoBehaviour
{
	[SerializeField]
	private Transform keysParent;

	[SerializeField]
	private LayoutGroup layoutGroup;

	private void OnEnable()
	{
		foreach (Transform item in keysParent)
		{
			item.gameObject.SetActive(value: true);
		}
		layoutGroup.ForceUpdateLayoutNoCanvas();
	}
}

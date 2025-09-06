using System.Linq;
using TeamCherry.SharedUtils;
using UnityEngine;

public class SavedFleaActivator : MonoBehaviour
{
	[SerializeField]
	private Transform[] fleaParents;

	[SerializeField]
	private string pdBoolTemplate;

	private void Start()
	{
		int remaining = PlayerData.instance.GetVariables<bool>(pdBoolTemplate).Count((bool val) => val);
		Transform[] array = fleaParents;
		for (int i = 0; i < array.Length; i++)
		{
			ActivateFleas(array[i], remaining, out remaining);
		}
	}

	private static void ActivateFleas(Transform fleaParent, int activeCount, out int remaining)
	{
		remaining = activeCount;
		if (activeCount > fleaParent.childCount)
		{
			activeCount = fleaParent.childCount;
		}
		foreach (Transform item in fleaParent)
		{
			item.gameObject.SetActive(value: false);
		}
		int num = 0;
		while (activeCount > 0)
		{
			num++;
			if (num <= 100)
			{
				GameObject gameObject = fleaParent.GetChild(Random.Range(0, fleaParent.childCount)).gameObject;
				if (!gameObject.activeSelf)
				{
					gameObject.SetActive(value: true);
					activeCount--;
					remaining--;
				}
				continue;
			}
			break;
		}
	}
}

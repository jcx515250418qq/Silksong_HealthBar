using UnityEngine;

public class ActivateRandomChild : MonoBehaviour
{
	[SerializeField]
	private Transform[] children;

	[SerializeField]
	private bool getChildren;

	[SerializeField]
	private bool disableOthers;

	private void OnEnable()
	{
		DoActivateRandomChildren();
	}

	public void DoActivateRandomChildren()
	{
		if (getChildren)
		{
			children = new Transform[base.transform.childCount];
			for (int i = 0; i < base.transform.childCount; i++)
			{
				children[i] = base.transform.GetChild(i);
			}
		}
		int num = Random.Range(0, children.Length);
		if (disableOthers)
		{
			for (int j = 0; j < children.Length; j++)
			{
				Transform transform = children[j];
				if ((bool)transform)
				{
					transform.gameObject.SetActive(num == j);
				}
			}
		}
		else
		{
			Transform transform2 = children[num];
			if ((bool)transform2)
			{
				transform2.gameObject.SetActive(value: true);
			}
		}
	}
}

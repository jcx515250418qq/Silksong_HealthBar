using UnityEngine;

public class RemoteDisableChild : MonoBehaviour
{
	public GameObject child;

	public void RemoteDisableObject()
	{
		child.SetActive(value: false);
	}

	public void RemoteEnableObject()
	{
		child.SetActive(value: true);
	}
}

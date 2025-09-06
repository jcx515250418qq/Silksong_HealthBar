using UnityEngine;

public class RemoteDisable : MonoBehaviour
{
	public void RemoteDisableObject()
	{
		base.gameObject.SetActive(value: false);
	}
}

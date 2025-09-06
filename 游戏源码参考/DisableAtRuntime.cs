using UnityEngine;

public class DisableAtRuntime : MonoBehaviour
{
	private void Awake()
	{
		base.gameObject.SetActive(value: false);
	}
}

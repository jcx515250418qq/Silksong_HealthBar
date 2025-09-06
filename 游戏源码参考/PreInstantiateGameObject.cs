using UnityEngine;

public class PreInstantiateGameObject : MonoBehaviour
{
	[SerializeField]
	private GameObject prefab;

	public GameObject InstantiatedGameObject { get; private set; }

	private void Awake()
	{
		if ((bool)prefab)
		{
			InstantiatedGameObject = Object.Instantiate(prefab);
			InstantiatedGameObject.SetActive(value: false);
		}
	}

	public void Activate()
	{
		InstantiatedGameObject.SetActive(value: true);
	}

	public void ActivateWithFsmEvent(string eventName)
	{
		InstantiatedGameObject.SetActive(value: true);
		InstantiatedGameObject.GetComponent<PlayMakerFSM>().SendEvent(eventName);
	}
}

using UnityEngine;

public class SharedDataBoolActivator : MonoBehaviour
{
	[SerializeField]
	private string key;

	[SerializeField]
	private bool targetValue = true;

	[SerializeField]
	private GameObject[] gameObjects;

	private Platform platform;

	private void Awake()
	{
		platform = Platform.Current;
	}

	private void OnEnable()
	{
		bool value = platform.RoamingSharedData.GetBool(key, def: false) == targetValue;
		gameObjects.SetAllActive(value);
	}
}

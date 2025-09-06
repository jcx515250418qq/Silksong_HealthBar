using UnityEngine;

public sealed class InitialiseChildren : MonoBehaviour
{
	[SerializeField]
	private bool forcePoolSpawn = true;

	private IInitialisable[] children;

	private void Awake()
	{
		children = GetComponentsInChildren<IInitialisable>(includeInactive: true);
		IInitialisable[] array = children;
		for (int i = 0; i < array.Length; i++)
		{
			array[i].OnAwake();
		}
	}

	private void Start()
	{
		IInitialisable[] array = children;
		foreach (IInitialisable obj in array)
		{
			obj.OnStart();
			PersonalObjectPool.CreateIfRequired(obj.gameObject, forcePoolSpawn);
		}
		children = null;
	}
}

using UnityEngine;

public interface IInitialisable
{
	GameObject gameObject { get; }

	bool OnAwake();

	bool OnStart();

	static void DoFullInit(GameObject gameObject)
	{
		IInitialisable[] componentsInChildren = gameObject.GetComponentsInChildren<IInitialisable>(includeInactive: true);
		foreach (IInitialisable obj in componentsInChildren)
		{
			obj.OnAwake();
			obj.OnStart();
		}
		PersonalObjectPool.CreateIfRequired(gameObject, !gameObject.activeSelf);
	}

	static void DoFullInitForcePool(GameObject gameObject)
	{
		IInitialisable[] componentsInChildren = gameObject.GetComponentsInChildren<IInitialisable>(includeInactive: true);
		foreach (IInitialisable obj in componentsInChildren)
		{
			obj.OnAwake();
			obj.OnStart();
		}
		PersonalObjectPool.CreateIfRequired(gameObject, forced: true);
	}
}

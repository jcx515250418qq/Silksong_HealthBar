using System;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Hornet/Spawn Queue Group")]
public class SpawnQueueGroup : ScriptableObject
{
	private const string QUEUE_POP_FSM_EVENT = "SPAWN DEQUEUED";

	private const string QUEUE_POP_SCRIPT_MESSAGE = "OnSpawnDequeued";

	[SerializeField]
	private int groupLimit;

	[NonSerialized]
	private List<GameObject> recycleQueue;

	public void AddSpawned(GameObject obj)
	{
		if ((bool)obj)
		{
			if (recycleQueue == null)
			{
				recycleQueue = new List<GameObject>(groupLimit);
			}
			if (recycleQueue.Count >= groupLimit)
			{
				GameObject gameObject = recycleQueue[0];
				recycleQueue.RemoveAt(0);
				gameObject.SendMessage("OnSpawnDequeued", SendMessageOptions.DontRequireReceiver);
				FSMUtility.SendEventToGameObject(gameObject, "SPAWN DEQUEUED");
			}
			RecycleResetHandler.Add(obj, (Action)delegate
			{
				recycleQueue.Remove(obj);
			});
			recycleQueue.Add(obj);
		}
	}
}

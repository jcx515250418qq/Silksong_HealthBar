using System.Collections.Generic;
using UnityEngine;

public class ChildUpdateOrdered : MonoBehaviour, IUpdateBatchableLateUpdate, IUpdateBatchableUpdate
{
	public interface IUpdateOrderUpdate
	{
		void UpdateOrderUpdate();
	}

	private enum UpdateOrder
	{
		Update = 0,
		LateUpdate = 1
	}

	[SerializeField]
	private GameObject[] orderedParents;

	[SerializeField]
	private UpdateOrder updateOrder;

	private UpdateBatcher updateBatcher;

	private List<IUpdateOrderUpdate> childrenOrdered;

	public bool ShouldUpdate => true;

	private void Awake()
	{
		childrenOrdered = new List<IUpdateOrderUpdate>();
		GameObject[] array = orderedParents;
		for (int i = 0; i < array.Length; i++)
		{
			IUpdateOrderUpdate[] componentsInChildren = array[i].GetComponentsInChildren<IUpdateOrderUpdate>(includeInactive: true);
			childrenOrdered.AddRange(componentsInChildren);
		}
	}

	private void OnEnable()
	{
		updateBatcher = GameManager.instance.GetComponent<UpdateBatcher>();
		updateBatcher.Add(this);
	}

	private void OnDisable()
	{
		if ((bool)updateBatcher)
		{
			updateBatcher.Remove(this);
			updateBatcher = null;
		}
	}

	public void BatchedUpdate()
	{
		if (updateOrder == UpdateOrder.Update)
		{
			DoUpdate();
		}
	}

	public void BatchedLateUpdate()
	{
		if (updateOrder == UpdateOrder.LateUpdate)
		{
			DoUpdate();
		}
	}

	private void DoUpdate()
	{
		foreach (IUpdateOrderUpdate item in childrenOrdered)
		{
			item.UpdateOrderUpdate();
		}
	}
}

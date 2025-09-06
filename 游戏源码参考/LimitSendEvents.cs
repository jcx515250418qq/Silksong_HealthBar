using System.Collections.Generic;
using UnityEngine;

public class LimitSendEvents : MonoBehaviour
{
	public Collider2D monitorCollider;

	private bool? previousColliderState;

	private List<GameObject> sentList = new List<GameObject>();

	private void OnEnable()
	{
		sentList.Clear();
	}

	private void Update()
	{
		if ((bool)monitorCollider)
		{
			if (monitorCollider.enabled == previousColliderState)
			{
				return;
			}
			previousColliderState = monitorCollider.enabled;
		}
		if (sentList.Count > 0)
		{
			sentList.Clear();
		}
	}

	public bool Add(GameObject obj)
	{
		if (!sentList.Contains(obj))
		{
			sentList.Add(obj);
			return true;
		}
		return false;
	}
}

using System;
using UnityEngine;

public class OnDestroyEventAnnouncer : MonoBehaviour
{
	public event Action<OnDestroyEventAnnouncer> OnDestroyEvent;

	private void OnDestroy()
	{
		this.OnDestroyEvent?.Invoke(this);
		this.OnDestroyEvent = null;
	}
}

using UnityEngine;

public sealed class TriggerStaySubEvent : TriggerSubEvent
{
	public event CollisionEvent OnTriggerStayed;

	private void OnTriggerStay2D(Collider2D other)
	{
		this.OnTriggerStayed?.Invoke(other);
	}
}

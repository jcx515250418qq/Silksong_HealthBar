using UnityEngine;

public abstract class TriggerSubEvent : MonoBehaviour
{
	public delegate void CollisionEvent(Collider2D collider);
}
